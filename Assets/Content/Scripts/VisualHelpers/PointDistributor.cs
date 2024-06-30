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

    [SerializeField] private int totalPoints = 1000;
    [SerializeField] private float debugSizeValue = 1.0f;
    [SerializeField] private Mesh meshToInstance; 
    [SerializeField] private Material material;

    private Matrix4x4[] matrices;
    
    struct TriangleSample
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector3 tangent;
    }
    
    struct Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector3 tangent;
    }

    private List<Matrix4x4> _points = new List<Matrix4x4>();

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

            Vector3 pointPosition = SamplePointOnTriangle(v0, v1, v2) + _meshFilter.transform.position;
            Quaternion pointRotation = Quaternion.identity;
            Vector3 pointScale = Vector3.one * debugSizeValue;

            Matrix4x4 matrix = Matrix4x4.TRS(pointPosition, pointRotation, pointScale);
            _points.Add(matrix);
        }
        
        matrices = new Matrix4x4[_points.Count];
        for (int i = 0; i < _points.Count; i++)
        {
            matrices[i] = _points[i];
        }
    }

    private void Update()
    {
        if (meshToInstance != null && material != null)
        {
            Graphics.DrawMeshInstanced(meshToInstance, 0, material, matrices);
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

    private Vector3 SamplePointOnTriangle(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        float sqrtR1 = Mathf.Sqrt(UnityEngine.Random.value);
        float r2 = UnityEngine.Random.value;

        float a = 1 - sqrtR1;
        float b = sqrtR1 * (1 - r2);
        float c = sqrtR1 * r2;

        Vector3 pointInsideTriangle = a * v0 + b * v1 + c * v2;

        return pointInsideTriangle;
    }

    private void OnValidate()
    {
        Generate();
    }

    // private void OnDrawGizmos()
    // {
    //     if (_points == null) 
    //         return;
    //
    //     foreach (var point in _points)
    //     {
    //         Gizmos.color = Color.yellow;
    //         Gizmos.DrawSphere(point.position, point.scale.x * debugSizeValue);
    //     }
    // }
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
