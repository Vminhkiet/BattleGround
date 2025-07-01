using System;
using System.Collections.Generic;
using UnityEngine;

public class SafeZoneManager : MonoBehaviour
{
    [Header("Safe Zone Settings")]
    public Transform safeZoneCenter;
    public float safeZoneRadius = 30f;
    public float safeZoneDeactivateThreshold = 5f;
    public float safeZoneViewOffset = 15f;

    [Header("Player Settings")]
    private Transform player;
    private Camera playerCamera;

    [Header("Emitter Settings")]
    public GameObject emitterPrefab;
    public int emitterCount = 9;
    public float emitterEdgeDistance = 1f;
    public float emitterOuterOffset = 10f;
    public LayerMask visibilityLayer;
    public Vector2 emitterGridSpacing = new Vector2(0.3f, 0.3f);
    public Vector2 emitterGridOffset = new Vector2(0f, 0f);
    public float emiiterHeight = 13;
    public int checkPoints = 12;
    public float edgeEmitterAngleSpacing = 10f;

    private GameObject[] emitters;
    private Plane[] cameraFrustum;
    private bool isPlayingSound;
    private AudioSource audioSource;

    public void SetPlayerTransform(Transform transform) {
        this.player = transform;
    }

    void Start()
    {
        playerCamera = Camera.main;
        isPlayingSound =false;
        emitters = new GameObject[emitterCount];
        audioSource = GetComponent<AudioSource>();
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
            SetUpSound(false);
            DisableAllEmitters();
        }
        else if (!playerInsideSafeZone && !safeZoneEdgeVisible)
        {
            SetUpSound(true);
            PlaceEmittersAroundPlayer();
        }
        else
        {
            SetUpSound(true);
            PlaceEmittersAlongSafeZoneEdge();
        }
    }

    void SetUpSound(bool play)
    {
        if (play)
        {
            if (!isPlayingSound)
            {
                Debug.Log("Play");
                audioSource.Play();
                isPlayingSound = true;
            }
        }
        else
        {
            if (isPlayingSound)
            {
                Debug.Log("Stop");
                audioSource.Stop();
                isPlayingSound = false;
            }
        }
    }

    bool IsSafeZoneEdgeInView()
    {
        Vector3 playerToCenter = (safeZoneCenter.position - player.position).normalized;
        Vector3 edgePoint = safeZoneCenter.position - playerToCenter * safeZoneRadius;

        Vector3 viewportPoint = playerCamera.WorldToViewportPoint(edgePoint);

        bool inFrontOfCamera = viewportPoint.z > 0;
        bool inScreenBounds = viewportPoint.x >= 0f && viewportPoint.x <= 1f &&
                              viewportPoint.y >= 0f && viewportPoint.y <= 1f;

        float screenDistance = Vector3.Distance(player.position, edgePoint);

        return inFrontOfCamera && inScreenBounds && screenDistance < safeZoneViewOffset;
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

                emitters[index].transform.position = new Vector3(worldPos.x, emiiterHeight, worldPos.z);
                emitters[index].SetActive(true);
            }
        }
    }

    void PlaceEmittersAlongSafeZoneEdge()
    {
        if (emitters.Length < 8)
        {
            Debug.LogWarning("Bạn cần ít nhất 8 emitter cho ma trận 2x4.");
            return;
        }

        Vector3 playerToCenter = (safeZoneCenter.position - player.position).normalized;
        float baseAngle = Mathf.Atan2(playerToCenter.x, playerToCenter.z) * Mathf.Rad2Deg;

        int perRow = 4;
        float startAngle = baseAngle - edgeEmitterAngleSpacing * (perRow - 1) / 2f;

        int emitterIndex = 0;

        for (int row = 0; row < 2; row++)
        {
            float radiusOffset = (row == 0) ? emitterEdgeDistance : emitterEdgeDistance + emitterOuterOffset;

            for (int i = 0; i < perRow; i++)
            {
                float angle = startAngle + i * edgeEmitterAngleSpacing;
                Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                Vector3 pos = safeZoneCenter.position + dir * (safeZoneRadius + radiusOffset);

                emitters[emitterIndex].transform.position = new Vector3(pos.x, emiiterHeight, pos.z);
                emitters[emitterIndex].SetActive(true);
                emitterIndex++;
            }
        }

        // Disable any unused emitters beyond the 8 used
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
