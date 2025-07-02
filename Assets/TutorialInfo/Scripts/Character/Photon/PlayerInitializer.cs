using UnityEngine;
using Photon.Pun;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField] private Transform[] playerSpawns;
    private string playerPrefabPath;

    public void SetPlayerSpawns(Transform[] spawns)
    {
        this.playerSpawns = spawns;
    }

    public void SetPlayerPrefabPath(string path)
    {
        this.playerPrefabPath = path;
    }

    public void SpawnAndInitializePlayer()
    {
        if (string.IsNullOrEmpty(playerPrefabPath))
        {
            Debug.LogError("PlayerInitializer: Player Prefab Path is not set. Cannot spawn player.", this);
            return;
        }

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogError("PlayerInitializer: Photon is not connected and ready. Cannot spawn player in online mode.", this);
            return;
        }

        if (PhotonNetwork.LocalPlayer == null)
        {
            Debug.LogError("PlayerInitializer: PhotonNetwork.LocalPlayer is null. Cannot determine spawn point. Aborting.", this);
            return;
        }

        Vector3 spawnPosition = Vector3.zero;
        if (playerSpawns != null && playerSpawns.Length > 0)
        {
            int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            if (spawnIndex >= 0 && spawnIndex >= playerSpawns.Length)
            {
                Debug.LogWarning($"PlayerInitializer: Spawn index {spawnIndex} is out of bounds for {playerSpawns.Length} spawn points. Choosing a random available spawn point.", this);
                spawnPosition = playerSpawns[Random.Range(0, playerSpawns.Length)].position;
            }
            else
            {
                spawnPosition = playerSpawns[spawnIndex].position;
                Debug.Log($"PlayerInitializer: Using designated spawn point for ActorNumber {PhotonNetwork.LocalPlayer.ActorNumber} at {spawnPosition}");
            }
        }
        else
        {
            Debug.LogWarning("PlayerInitializer: No spawn points assigned. Spawning at (0,0,0).", this);
        }

        Debug.Log($"PlayerInitializer: Spawning via PhotonNetwork.Instantiate: {playerPrefabPath} at {spawnPosition}");

        GameObject playerGameObject = PhotonNetwork.Instantiate(playerPrefabPath, spawnPosition, Quaternion.identity);

        if (playerGameObject == null)
        {
            Debug.LogError("PlayerInitializer: PhotonNetwork.Instantiate returned null! Player prefab might be missing or path is incorrect.", this);
            return;
        }

        PhotonView playerPhotonView = playerGameObject.GetComponent<PhotonView>();

        if (playerPhotonView == null)
        {
            Debug.LogError("PlayerInitializer: Spawned player prefab is missing PhotonView component! This is critical for online mode.", playerGameObject);
            return;
        }

        PhotonTransformAdapter photonTransformAdapter = playerGameObject.GetComponentInChildren<PhotonTransformAdapter>();
        if (photonTransformAdapter == null)
        {
            Debug.LogError("PlayerInitializer: PhotonTransformAdapter not found on playerPrefab! Please add it to the Prefab.", playerGameObject);
            return;
        }


        APlayerInputHandler inputHandler = playerGameObject.GetComponent<APlayerInputHandler>();
        IEffectAttackManager actualEffectManager = playerGameObject.GetComponent<IEffectAttackManager>();
        ICharacterSkill characterSkill = playerGameObject.GetComponentInChildren<ICharacterSkill>();
        IMovable movementComponent = playerGameObject.GetComponent<IMovable>();
        PlayerStats playerStats = playerGameObject.GetComponentInChildren<PlayerStats>();
        CharacterMeshRotation cRotation = playerGameObject.GetComponentInChildren<CharacterMeshRotation>();
        INetworkOwnership networkOwnershipInstance = new PhotonOwnershipAdapter(playerPhotonView);
        INetworkTransform networkTransform = photonTransformAdapter;
        UltiChargeManager _ultiChargeManager = playerGameObject.GetComponent<UltiChargeManager>();
        ResetAnimationEvent resetAnimationEvent = playerGameObject.GetComponentInChildren<ResetAnimationEvent>();

        IEffectPlayer effectPlayerInstance;

        bool activateInputHandler = false;

        if (playerPhotonView.IsMine)
        {
            TopDownCameraFollow topdown = Camera.main.GetComponent<TopDownCameraFollow>();
            MinimapController minimapController = GameObject.FindAnyObjectByType<MinimapController>();
            SafeZoneManager safezone = GameObject.FindAnyObjectByType<SafeZoneManager>();
            topdown.SetTransform(playerGameObject.transform);
            minimapController.SetPlayerTransform(playerGameObject.transform);
            safezone.SetPlayerTransform(playerGameObject.transform);
            gameObject.tag = "Player";

            activateInputHandler = true;
            effectPlayerInstance = new PhotonEffectManagerAdapter(actualEffectManager, playerPhotonView);

        }
        else
        {
            gameObject.tag = "Enemy";

            activateInputHandler = false;
            effectPlayerInstance = new NoOpEffectPlayer();

        }

        if (inputHandler != null)
        {
            if (activateInputHandler)
            {
                inputHandler.Initialize(
                    effectPlayerInstance,
                    characterSkill,
                    movementComponent,
                    playerStats,
                    cRotation,
                    networkOwnershipInstance,
                    photonTransformAdapter,
                    _ultiChargeManager,
                    resetAnimationEvent
                );
                inputHandler.enabled = true;
                Debug.Log("PlayerInitializer: InputHandler activated for local player.");
            }
            else
            {
                inputHandler.enabled = false;
                Debug.Log("PlayerInitializer: InputHandler deactivated for remote player.");
            }
        }
        else
        {
            Debug.LogError("PlayerInitializer: APlayerInputHandler not found on player prefab! Player input will not work.", playerGameObject);
        }
    }
    public void SetSpawnPoints(Transform[] spawns)
    {
        this.playerSpawns = spawns;
    }
    public class NoOpEffectPlayer : IEffectPlayer
    {
        public void PlayNormalAttackEffect(int phase, Vector2 direction) { }
        public void PlayUltiEffect(Vector3 position) { }
        public void PlaySpellEffect(Vector2 direction) { }
        public void PlayUltiEffect(Vector2 direction) { }
        public void StopAllEffects() { }
    }
}
