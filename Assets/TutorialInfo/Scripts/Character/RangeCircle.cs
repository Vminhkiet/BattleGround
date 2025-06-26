using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeCircle : MonoBehaviour
{
    public float radius = 10f;
    public int segments = 60;
    public LineRenderer line;

    void Start()
    {
        if (line == null)
            line = GetComponent<LineRenderer>();

        DrawCircle();
    }

    void DrawCircle()
    {
        line.positionCount = segments + 1;
        line.loop = true;
        line.useWorldSpace = false;

        float angleStep = 360f / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleStep;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            line.SetPosition(i, new Vector3(x, 0f, z));
        }
    }
}
