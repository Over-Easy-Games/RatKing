using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// [ExecuteInEditMode]
public class PointDistributor : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private Mesh _mesh;

    [SerializeField] private int totalPoints = 100;
    [SerializeField] private Mesh meshToInstance; 
    [SerializeField] private Material material;
    [SerializeField] private float debugSizeValue = 0.2f;
    [SerializeField] private float debugSpeed = 0.01f;
    [SerializeField] private bool drawGizmo = false;
    [SerializeField] private bool drawMeshes = true;

    private Matrix4x4[] matrices;
    
    struct Vertex
    {
        public Vertex(Vector3 _pos, Vector3 _normal)
        {
            position = _pos;
            normal = _normal;
        }
        
        public Vector3 position;
        public Vector3 normal;
    }
    
    struct Triangle
    {
        public Triangle(Vertex v0, Vertex v1, Vertex v2)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
        }
        
        public Vertex v0;
        public Vertex v1;
        public Vertex v2;
        
        public Vertex SampleRandomPointOnTriangle()
        {
            float sqrtR1 = Mathf.Sqrt(UnityEngine.Random.value);
            float r2 = UnityEngine.Random.value;

            float a = 1 - sqrtR1;
            float b = sqrtR1 * (1 - r2);
            float c = sqrtR1 * r2;

            Vector3 pointInsideTriangle = a * v0.position + b * v1.position + c * v2.position;
            Vector3 normalInsideTriangle = a * v0.normal + b * v1.normal + c * v2.normal;

            return new Vertex(pointInsideTriangle, normalInsideTriangle);
        }
    }

    private List<Matrix4x4> _points = new List<Matrix4x4>();
    
    private Matrix4x4[] _targets;

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        _points.Clear();

        if (TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
            _meshFilter = meshFilter;

        _mesh = _meshFilter.sharedMesh;
        Vector3[] vertices = _mesh.vertices;
        Vector3[] normals = _mesh.normals;
        int[] triangles = _mesh.triangles;

        
        float totalArea = 0f;
        List<float> areas = new List<float>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];
            float area = Vector3.Cross(v1 - v0, v2 - v0).magnitude * 0.5f;
            areas.Add(area);
            totalArea += area;
        }

        List<float> cumulativeAreas = new List<float>();
        float cumulativeArea = 0f;
        foreach (var area in areas)
        {
            cumulativeArea += area;
            cumulativeAreas.Add(cumulativeArea);
        }

        System.Random rand = new System.Random();
        for (int i = 0; i < totalPoints; i++)
        {
            float r = (float)rand.NextDouble() * totalArea;
            int triangleIndex = FindTriangleIndex(cumulativeAreas, r);
            Vector3 v0 = vertices[triangles[triangleIndex * 3]];
            Vector3 v1 = vertices[triangles[triangleIndex * 3 + 1]];
            Vector3 v2 = vertices[triangles[triangleIndex * 3 + 2]];
            Vector3 n0 = normals[triangles[triangleIndex * 3]];
            Vector3 n1 = normals[triangles[triangleIndex * 3 + 1]];
            Vector3 n2 = normals[triangles[triangleIndex * 3 + 2]];

            Vector3 positionOffset = _meshFilter.transform.position;
            Vertex vertex0 = new Vertex(v0 + positionOffset, n0);
            Vertex vertex1 = new Vertex(v1 + positionOffset, n1);
            Vertex vertex2 = new Vertex(v2 + positionOffset, n2);

            Vertex randomSample = new Triangle(vertex0, vertex1, vertex2).SampleRandomPointOnTriangle();
            Quaternion pointRotation = Quaternion.LookRotation(randomSample.normal, Vector3.up);
            Vector3 pointScale = Vector3.one * debugSizeValue;

            Matrix4x4 matrix = Matrix4x4.TRS(randomSample.position, pointRotation, pointScale);
            _points.Add(matrix);
        }
        
        matrices = new Matrix4x4[_points.Count];
        _targets = new Matrix4x4[_points.Count];
        for (int i = 0; i < _points.Count; i++)
        {
            matrices[i] = _points[i];
            _targets[i] = _points[ rand.Next() % _points.Count ];
        }
        
    }

    private void Update()
    {
        UpdatePositions();
        if (meshToInstance != null && material != null && drawMeshes)
        {
            Graphics.DrawMeshInstanced(meshToInstance, 0, material, matrices);
        }
    }

    private void UpdatePositions()
    {
        for (int matrixIndex = 0; matrixIndex < _points.Count; matrixIndex++){
            Matrix4x4 currentMatrix = matrices[matrixIndex];
            
            Vector3 currentPosition = currentMatrix.GetPosition();
            Vector3 targetPosition = _targets[matrixIndex].GetPosition();

            Vector3 velocity = (targetPosition - currentPosition).normalized * debugSpeed;
            
            // Quaternion currentRotation = currentMatrix.rotation;
            Quaternion currentRotation = Quaternion.LookRotation(velocity, Vector3.up);

            currentMatrix.SetTRS( currentPosition + velocity, currentRotation, Vector3.one * debugSizeValue );
            matrices[matrixIndex] = currentMatrix;
            
            System.Random rand = new System.Random();
            
            float distanceToTarget = Vector3.Distance(currentPosition, targetPosition);
            if (distanceToTarget < 0.1f){
                _targets[matrixIndex] = _points[ rand.Next() % _points.Count ];
            }
            
        }
            
    }

    private int FindTriangleIndex(List<float> cumulativeAreas, float value)
    {
        for (int triangleIndex = 0; triangleIndex < cumulativeAreas.Count; triangleIndex++)
        {
            if (value < cumulativeAreas[triangleIndex])
            {
                return triangleIndex;
            }
        }
        return cumulativeAreas.Count - 1;
    }

    private void OnValidate()
    {
        Generate();
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmo)
            return;
        
        if (_points == null) 
            return;
    
        foreach (var instance in matrices)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(instance.GetPosition(), debugSizeValue);
        }
    
        foreach (var point in _points)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(point.GetPosition(), debugSizeValue / 2f);
        }
    }
}

[CustomEditor(typeof(PointDistributor))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PointDistributor myScript = (PointDistributor)target;
        if (GUILayout.Button("Generate"))
        {
            myScript.Generate();
        }
    }
}
