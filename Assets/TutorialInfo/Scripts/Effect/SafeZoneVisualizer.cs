using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SafeZoneVisualizer : MonoBehaviour
{
    public Transform safeZoneCenter;
    public float radius = 30f;
    public int segments = 60;
    public float lineHeight = 0.1f;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = segments;

        DrawCircle();
    }

    void DrawCircle()
    {
        Vector3[] points = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            float angle = ((float)i / segments) * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            points[i] = new Vector3(safeZoneCenter.position.x + x, lineHeight, safeZoneCenter.position.z + z);
        }

        lineRenderer.SetPositions(points);
    }
}
