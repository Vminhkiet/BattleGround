using System;
using System.Collections.Generic;
using UnityEngine;

public class SafeZoneManager : MonoBehaviour
{
    [Header("Safe Zone Settings")]
    public Transform safeZoneCenter;
    public float safeZoneRadius = 30f;
    public float safeZoneDeactivateThreshold = 5f;

    [Header("Player Settings")]
    public Transform player;
    public Camera playerCamera;

    [Header("Emitter Settings")]
    public GameObject emitterPrefab;
    public int emitterCount = 9;
    public float emitterEdgeDistance = 1f;
    public float emitterOuterOffset = 10f;
    public LayerMask visibilityLayer;
    public Vector2 emitterGridSpacing = new Vector2(0.3f, 0.3f);
    public Vector2 emitterGridOffset = new Vector2(0f, 0f);

    private GameObject[] emitters;
    private Plane[] cameraFrustum;

    void Start()
    {
        emitters = new GameObject[emitterCount];
        for (int i = 0; i < emitterCount; i++)
        {
            emitters[i] = Instantiate(emitterPrefab, transform);
            emitters[i].SetActive(false);
        }
    }

    void Update()
    {
        float distanceFromSafeZone = Vector3.Distance(player.position, safeZoneCenter.position);
        bool playerInsideSafeZone = distanceFromSafeZone < safeZoneRadius;
        bool safeZoneEdgeVisible = IsSafeZoneEdgeInView();

        if (playerInsideSafeZone && !safeZoneEdgeVisible && distanceFromSafeZone < safeZoneRadius - safeZoneDeactivateThreshold)
        {
            DisableAllEmitters();
        }
        else if (!playerInsideSafeZone && !safeZoneEdgeVisible)
        {
            PlaceEmittersAroundPlayer();
        }
        else // safe zone edge is in view
        {
            PlaceEmittersAlongSafeZoneEdge();
        }
    }

    bool IsSafeZoneEdgeInView()
    {
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        Vector3 samplePoint = safeZoneCenter.position + (player.position - safeZoneCenter.position).normalized * safeZoneRadius;
        Bounds testBounds = new Bounds(samplePoint, Vector3.one * 2f);
        return GeometryUtility.TestPlanesAABB(cameraFrustum, testBounds);
    }

    void DisableAllEmitters()
    {
        foreach (var e in emitters)
        {
            e.SetActive(false);
        }
    }

    void PlaceEmittersAroundPlayer()
    {
        float y = emitterPrefab.transform.position.y;
        int gridSize = 3;

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                int index = row * gridSize + col;
                if (index >= emitters.Length) break;

                float x = emitterGridSpacing.x * (col - 1) + emitterGridOffset.x;
                float z = emitterGridSpacing.y * (row - 1) + emitterGridOffset.y;
                Vector3 viewportPos = new Vector3(0.5f + x, 0.5f + z, playerCamera.nearClipPlane + 1f);
                Vector3 worldPos = playerCamera.ViewportToWorldPoint(viewportPos);

                emitters[index].transform.position = new Vector3(worldPos.x, y, worldPos.z);
                emitters[index].SetActive(true);
            }
        }
    }

    void PlaceEmittersAlongSafeZoneEdge()
    {
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        float y = emitterPrefab.transform.position.y;
        int emitterIndex = 0;

        // Place emitters in pairs: one on edge, one offset outward
        for (int i = 0; i < 360; i += 10)
        {
            if (emitterIndex >= emitterCount - 1) break;

            Vector3 dir = Quaternion.Euler(0, i, 0) * Vector3.forward;

            Vector3 edgePoint = safeZoneCenter.position + dir * (safeZoneRadius + emitterEdgeDistance);
            Vector3 outerPoint = safeZoneCenter.position + dir * (safeZoneRadius + emitterEdgeDistance + emitterOuterOffset);

            Bounds b = new Bounds(edgePoint, Vector3.one * 2f);
            if (GeometryUtility.TestPlanesAABB(cameraFrustum, b))
            {
                emitters[emitterIndex].transform.position = new Vector3(edgePoint.x, y, edgePoint.z);
                emitters[emitterIndex].SetActive(true);
                emitterIndex++;

                emitters[emitterIndex].transform.position = new Vector3(outerPoint.x, y, outerPoint.z);
                emitters[emitterIndex].SetActive(true);
                emitterIndex++;
            }
        }

        for (int i = emitterIndex; i < emitters.Length; i++)
        {
            emitters[i].SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        if (safeZoneCenter != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(safeZoneCenter.position, safeZoneRadius);
        }
    }
}
