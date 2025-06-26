using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR // Chỉ biên dịch phần này trong Unity Editor
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ArcMeshGenerator : MonoBehaviour
{
    [Header("Arc Settings")]
    public float radius = 3f;
    [Range(0, 360)]
    public float angle = 90f;
    [Range(3, 60)]
    public int segments = 10;

    [Header("Material Settings")]
    public Material arcMaterial;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    // Called when the script is loaded or a value is changed in the Inspector
    void OnValidate()
    {
        SetupComponents(); // Đảm bảo components được lấy

        // --- CHỈ TẠO/CẬP NHẬT MESH TRONG ONVALIDATE ---
        // KHÔNG GÁN MATERIAL Ở ĐÂY để tránh lỗi SendMessage
        if (meshFilter == null) return; // Đảm bảo meshFilter không null

        // Quản lý sharedMesh trong Editor
        if (meshFilter.sharedMesh == null || meshFilter.sharedMesh != mesh)
        {
            if (meshFilter.sharedMesh != null)
            {
                // Chỉ hủy nếu không phải là asset từ disk
#if UNITY_EDITOR
                if (!EditorUtility.IsPersistent(meshFilter.sharedMesh))
                {
                    DestroyImmediate(meshFilter.sharedMesh); // Dùng DestroyImmediate trong Editor Mode
                }
#endif
            }
            mesh = new Mesh();
            mesh.name = "GeneratedArcMesh_Editor"; // Đặt tên khác để phân biệt
            meshFilter.sharedMesh = mesh; // Gán Mesh mới vào sharedMesh
        }
        else
        {
            mesh.Clear(); // Xóa dữ liệu cũ nếu sử dụng lại Mesh hiện có
        }
        GenerateArcMeshInternal(); // Tạo/cập nhật dữ liệu Mesh
    }

    // Called when the script instance is being loaded.
    void Awake()
    {
        SetupComponents(); // Đảm bảo components được lấy

        // --- TẠO/CẬP NHẬT MESH VÀ GÁN MATERIAL TRONG AWAKE (RUNTIME) ---
        if (meshFilter == null) return; // Đảm bảo meshFilter không null

        // Quản lý mesh cho Runtime
        if (mesh == null || meshFilter.mesh != mesh || meshFilter.mesh == null)
        {
            if (meshFilter.mesh != null && !Application.isEditor) // Chỉ hủy nếu không trong Editor và không phải asset
            {
                Destroy(meshFilter.mesh); // Dùng Destroy trong Play Mode
            }
            mesh = new Mesh();
            mesh.name = "GeneratedArcMesh_Runtime"; // Đặt tên khác để phân biệt
            meshFilter.mesh = mesh; // Gán Mesh mới vào mesh
        }
        else
        {
            mesh.Clear(); // Xóa dữ liệu cũ nếu sử dụng lại Mesh hiện có
        }
        GenerateArcMeshInternal(); // Tạo/cập nhật dữ liệu Mesh

        // Gán Material chỉ ở Awake (Runtime)
        if (meshRenderer != null && arcMaterial != null)
        {
            meshRenderer.material = arcMaterial; // Gán material instance trong Play Mode
        }
        else if (meshRenderer == null)
        {
            Debug.LogError("MeshRenderer not found for material assignment in Awake!", this);
        }
        else // arcMaterial == null
        {
            Debug.LogWarning("Arc Material is not assigned to ArcMeshGenerator! Please assign it in the Inspector.", this);
        }
    }

    // Hàm để lấy tham chiếu đến MeshFilter và MeshRenderer
    private void SetupComponents()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }
        }
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
            }
        }
    }

    // Hàm này tạo/cập nhật dữ liệu Mesh, không gán MeshFilter
    private void GenerateArcMeshInternal()
    {
        if (mesh == null)
        {
            Debug.LogError("Mesh is null. Cannot generate mesh data. This should not happen.", this);
            return;
        }

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        vertices.Add(Vector3.zero);
        uvs.Add(new Vector2(0.5f, 0.5f));

        float startAngleRad = (-angle / 2f) * Mathf.Deg2Rad;
        float angleIncrementRad = (angle / segments) * Mathf.Deg2Rad;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngleRad = startAngleRad + (float)i * angleIncrementRad;
            float x = radius * Mathf.Sin(currentAngleRad);
            float z = radius * Mathf.Cos(currentAngleRad);

            vertices.Add(new Vector3(x, 0, z));
            uvs.Add(new Vector2(x / (2 * radius) + 0.5f, z / (2 * radius) + 0.5f));
        }

        for (int i = 0; i < segments; i++)
        {
            triangles.Add(0);
            triangles.Add(i + 2);
            triangles.Add(i + 1);
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    // Để hình dung vùng tấn công trong Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (meshFilter == null || meshFilter.sharedMesh == null || transform == null) return;

        // Sử dụng meshFilter.sharedMesh cho Gizmos trong Editor
        Mesh gizmoMesh = meshFilter.sharedMesh;
        if (gizmoMesh == null) return;

        Vector3 center = transform.position;
        Vector3 startDir = Quaternion.Euler(0, -angle / 2, 0) * transform.forward;
        Vector3 endDir = Quaternion.Euler(0, angle / 2, 0) * transform.forward;

        Gizmos.DrawLine(center, center + startDir * radius);
        Gizmos.DrawLine(center, center + endDir * radius);

        Vector3 previousPoint = center + Quaternion.Euler(0, -angle / 2, 0) * transform.forward * radius;
        for (int i = 1; i <= segments; i++)
        {
            float currentSegmentAngle = -angle / 2 + (float)i / segments * angle;
            Vector3 currentPoint = center + Quaternion.Euler(0, currentSegmentAngle, 0) * transform.forward * radius;
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }

    // Clean up mesh when object is destroyed to prevent memory leaks in Editor
    void OnDestroy()
    {
        // Chỉ hủy mesh nếu nó là mesh chúng ta tạo và đang được gán cho MeshFilter
        // Hoặc nếu nó là mesh của chúng ta nhưng không còn gán nữa
        if (meshFilter != null && meshFilter.sharedMesh == mesh)
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                if (!EditorUtility.IsPersistent(meshFilter.sharedMesh))
                {
                    DestroyImmediate(meshFilter.sharedMesh);
                }
#endif
            }
            else
            {
                Destroy(meshFilter.sharedMesh);
            }
        }
        else if (mesh != null) // Trường hợp mesh đã không còn gắn vào MeshFilter nữa
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                if (!EditorUtility.IsPersistent(mesh))
                {
                    DestroyImmediate(mesh);
                }
#endif
            }
            else
            {
                Destroy(mesh);
            }
        }
        mesh = null; // Đảm bảo biến mesh được đặt về null
    }
}