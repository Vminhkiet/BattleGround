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
        radius = Mathf.Lerp(radius, safeZoneManager.safeZoneRadius, Time.deltaTime * 3f);
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
            Vector3 center = safeZoneManager.safeZoneCenter != null ? safeZoneManager.safeZoneCenter.position : transform.position;
            points[i] = new Vector3(center.x + x, lineHeight, center.z + z);
        }

        lineRenderer.SetPositions(points);
    }
}
