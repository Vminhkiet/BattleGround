using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SafeZoneVisualizer : MonoBehaviour
{
    public SafeZoneManager safeZoneManager;
    private float radius;
    public int segments = 60;
    public float lineHeight = 0.1f;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        safeZoneManager=GetComponent<SafeZoneManager>();
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = segments;      
    }

    private void Update()
    {
        radius=safeZoneManager.safeZoneRadius;
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
            points[i] = new Vector3(transform.position.x + x, lineHeight, transform.position.z + z);
        }

        lineRenderer.SetPositions(points);
    }
}
