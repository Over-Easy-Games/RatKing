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

    private Matrix4x4[] _instanceMatrices;
    private Matrix4x4[] _possibleTargets;
    private int[] _instanceTargetIndices;

    private List<Matrix4x4> _generatedSurfacePoints = new List<Matrix4x4>();

    private bool[,] _meshVertexAdjacency;
    
    struct Vertex
    {
        public Vertex(Vector3 position, Vector3 normal)
        {
            this.position = position;
            this.normal = normal;
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

    private void Start()
    {
        Generate();
    }

    private void GenerateMeshVertexAdjacency(Mesh mesh)
    {
        _meshVertexAdjacency = new bool[ mesh.vertexCount, mesh.vertexCount ];

        for (int triangleIndex = 0; triangleIndex < mesh.triangles.Length; triangleIndex += 3){
            
            if (triangleIndex + 2 >= mesh.triangles.Length || triangleIndex + 1 >= mesh.triangles.Length)
                continue;

            int[] triangles = mesh.triangles;
            
            _meshVertexAdjacency[ triangles[triangleIndex], triangles[triangleIndex + 1] ] = true;
            _meshVertexAdjacency[ triangles[triangleIndex + 1], triangles[triangleIndex + 0] ] = true;
            
            _meshVertexAdjacency[ triangles[triangleIndex + 1], triangles[triangleIndex + 2] ] = true;
            _meshVertexAdjacency[ triangles[triangleIndex + 2], triangles[triangleIndex + 1] ] = true;
            
            _meshVertexAdjacency[ triangles[triangleIndex], triangles[triangleIndex + 2] ] = true;
            _meshVertexAdjacency[ triangles[triangleIndex + 2], triangles[triangleIndex] ] = true;
        }
    }
    
    public void Generate()
    {
        _generatedSurfacePoints.Clear();

        if (TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
            _meshFilter = meshFilter;

        _mesh = _meshFilter.sharedMesh;
        Vector3[] vertices = _mesh.vertices;
        Vector3[] normals = _mesh.normals;
        int[] triangles = _mesh.triangles;

        GenerateMeshVertexAdjacency(_mesh);
        
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
            _generatedSurfacePoints.Add(matrix);
        }

        bool useGeneratedPoint = false;
        switch(useGeneratedPoint) 
        {
            case true:
                _possibleTargets = new Matrix4x4[_generatedSurfacePoints.Count];
                for (int i = 0; i < _possibleTargets.Length; i++){
                    _possibleTargets[i] = _generatedSurfacePoints[i];
                }
                break;
            case false:
                _possibleTargets = new Matrix4x4[_mesh.vertexCount];
                for (int i = 0; i < _possibleTargets.Length; i++){
                    _possibleTargets[i] = Matrix4x4.TRS(  _meshFilter.transform.position + _mesh.vertices[i], Quaternion.identity, Vector3.one );
                }
                break;
        }
        

        
        _instanceMatrices = new Matrix4x4[_generatedSurfacePoints.Count];
        _instanceTargetIndices = new int[_generatedSurfacePoints.Count];
        for (int i = 0; i < _instanceMatrices.Length; i++)
        {
            _instanceMatrices[i] = _generatedSurfacePoints[i];
            _instanceTargetIndices[i] = rand.Next() % _possibleTargets.Length;
        }

    }

    private void Update()
    {
        UpdatePositions();
        if (meshToInstance != null && material != null && drawMeshes)
        {
            Graphics.DrawMeshInstanced(meshToInstance, 0, material, _instanceMatrices);
        }
    }

    private void UpdatePositions()
    {
        for (int matrixIndex = 0; matrixIndex < _generatedSurfacePoints.Count; matrixIndex++){
            Matrix4x4 currentMatrix = _instanceMatrices[matrixIndex];
            
            Vector3 currentPosition = currentMatrix.GetPosition();
            Vector3 targetPosition =  _possibleTargets[_instanceTargetIndices[matrixIndex]].GetPosition();

            Vector3 velocity = (targetPosition - currentPosition).normalized * debugSpeed;
            
            // Quaternion currentRotation = currentMatrix.rotation;
            Quaternion currentRotation = Quaternion.LookRotation(velocity, Vector3.up);

            currentMatrix.SetTRS( currentPosition + velocity, currentRotation, Vector3.one * debugSizeValue );
            _instanceMatrices[matrixIndex] = currentMatrix;
            
            System.Random rand = new System.Random();
            float distanceToTarget = Vector3.Distance(currentPosition, targetPosition);
            if (distanceToTarget < 0.1f){
                // _instanceTargetIndices[matrixIndex] = rand.Next() % _possibleTargets.Length;

                int currentTargetIndex = _instanceTargetIndices[matrixIndex];
                int newTargetIndex = -1;

                if (HasAdjacentVertices(currentTargetIndex)){
                    while (newTargetIndex == -1){
                        int possibleNewTargetIndex = rand.Next() % _possibleTargets.Length;
                        if (_meshVertexAdjacency[currentTargetIndex, possibleNewTargetIndex])
                            newTargetIndex = possibleNewTargetIndex;
                    }
                }
                
                _instanceTargetIndices[matrixIndex] = newTargetIndex;
            }
            
        }
            
    }

    private bool HasAdjacentVertices(int srcIndex)
    {
        for (int columnIndex = 0; columnIndex < _meshVertexAdjacency.GetLength(0); columnIndex++){
            if (_meshVertexAdjacency[srcIndex, columnIndex])
                return true;
        }
        return false;
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
        
        if (_generatedSurfacePoints == null) 
            return;
    
        foreach (var instance in _instanceMatrices)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(instance.GetPosition(), debugSizeValue);
        }
    
        foreach (var point in _generatedSurfacePoints)
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
