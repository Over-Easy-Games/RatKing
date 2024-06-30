using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PointDistributor : MonoBehaviour
{

    private MeshFilter _meshFilter;
    private Mesh _mesh;

    [SerializeField] private int numberOfPoints = 100;
    [SerializeField] private float debugSizeValue = 1.0f;
    
    struct Point
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    private List<Point> _points;

    public void Generate()
    {
        _points = new List<Point>();
        _points.Clear();
        if (TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
            _meshFilter = meshFilter;
        _mesh = _meshFilter.mesh;
        
        
        Vector3[] vertices = _mesh.vertices;
        int[] triangles = _mesh.triangles;

        foreach (var vert in vertices)
        {
            Point newPoint = new Point();
            newPoint.position = vert + _meshFilter.transform.position;
            newPoint.rotation = _meshFilter.transform.rotation;
            newPoint.scale = _meshFilter.transform.localScale;
            
            _points.Add(newPoint);
        }
        
    }

    private void OnValidate()
    {
        Generate();
    }

    private void OnDrawGizmos()
    {
        if (_points == null)
            return;
        
        foreach (var point in _points)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(point.position, point.scale.x * debugSizeValue);
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
        if(GUILayout.Button("Generate"))
        {
            myScript.Generate();
        }
    }
}
