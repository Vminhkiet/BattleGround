using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Player Info")]
    public string playerInfo = "Database/InfoPlayer";
    [Header("Database Link Character")]
    private string linkCharacter = "Database/LinkCharacter";

    private string linkPlayer;

    [Header("Player Spawning")]
    public Transform[] playerSpawns;

    [Header("AI/Enemy Spawning")]
    [Tooltip("Danh sách các đường dẫn Prefab của tướng AI/đối thủ.")]
    public string[] enemyCharacterPrefabPaths;
    [Tooltip("Số lượng tướng AI/đối thủ muốn spawn.")]
    [Range(0, 10)]
    public int numberOfEnemiesToSpawn = 1;
    [Tooltip("Vị trí spawn ngẫu nhiên cho tướng AI/đối thủ.")]
    public Transform[] enemySpawnPoints;

    [Header("Player Initializer")]
    public PlayerInitializer playerInitializer;

    [Header("Game State")]
    [Tooltip("Tên Scene Lobby/Menu để quay về khi trận đấu kết thúc hoặc rời phòng.")]
    public string lobbySceneName = "Main";

    private bool hasSpawnedLocalPlayer = false;
    private bool hasMasterClientSpawnedEnemies = false;

    private void Awake()
    {
        if (playerInitializer == null)
        {
            playerInitializer = FindObjectOfType<PlayerInitializer>();
            if (playerInitializer == null)
            {
                Debug.LogError("GameManager: PlayerInitializer component not found. Disabling GameManager.", this);
                this.enabled = false;
                return;
            }
        }

        InfoPlayerDatabase info = Resources.Load<InfoPlayerDatabase>(playerInfo);
        CharacterPrefabCollection collection = Resources.Load<CharacterPrefabCollection>(linkCharacter);
        if (info == null || collection == null)
        {
            LeaveRoomAndGoToLobby();
            return;
        }

        linkPlayer = collection.GetPrefabPath(info.getCharacterType());

        if (string.IsNullOrEmpty(linkPlayer))
        {
            LeaveRoomAndGoToLobby();
            return;
        }

        playerInitializer.SetPlayerPrefabPath(linkPlayer);
        playerInitializer.SetPlayerSpawns(playerSpawns);

        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom && !hasSpawnedLocalPlayer)
        {
            Debug.Log("GameManager.Awake(): Photon already connected and in room. Spawning local player.");
            playerInitializer.SpawnAndInitializePlayer();
            hasSpawnedLocalPlayer = true;
        }
    }

    public override void OnJoinedRoom()
    {
        if (!hasSpawnedLocalPlayer && PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("GameManager.OnJoinedRoom() được gọi. Đang yêu cầu PlayerInitializer SpawnAndInitializePlayer.");
            playerInitializer.SpawnAndInitializePlayer();
            hasSpawnedLocalPlayer = true;
        }
        else if (hasSpawnedLocalPlayer)
        {
            Debug.Log("GameManager.OnJoinedRoom() được gọi nhưng người chơi đã được spawn rồi. Bỏ qua.");
        }
        else
        {
            Debug.LogWarning("GameManager.OnJoinedRoom() được gọi nhưng Photon chưa sẵn sàng để spawn. Sẽ chờ.");
        }

        if (PhotonNetwork.IsMasterClient && !hasMasterClientSpawnedEnemies)
        {
            Debug.Log("GameManager.OnJoinedRoom(): Master Client is spawning enemies.");
            SpawnEnemies();
            hasMasterClientSpawnedEnemies = true;
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("GameManager.OnJoinedRoom(): Not Master Client. Waiting for enemies to be spawned by Master.");
        }
    }

    private void SpawnEnemies()
    {
        if (enemyCharacterPrefabPaths == null || enemyCharacterPrefabPaths.Length == 0)
        {
            Debug.LogWarning("GameManager: No enemy character prefabs defined to spawn.", this);
            return;
        }

        if (enemySpawnPoints == null || enemySpawnPoints.Length == 0)
        {
            Debug.LogError("GameManager: No enemy spawn points assigned! Cannot spawn enemies.", this);
            return;
        }

        HashSet<int> usedSpawnIndices = new HashSet<int>();
        System.Random rnd = new System.Random(PhotonNetwork.CurrentRoom.Name.GetHashCode());

        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            if (enemySpawnPoints.Length == usedSpawnIndices.Count)
            {
                Debug.LogWarning("GameManager: Ran out of unique enemy spawn points. Spawning fewer enemies than requested.", this);
                break;
            }

            int randomSpawnIndex;
            do
            {
                randomSpawnIndex = rnd.Next(0, enemySpawnPoints.Length);
            } while (usedSpawnIndices.Contains(randomSpawnIndex));

            usedSpawnIndices.Add(randomSpawnIndex);

            // Use enemySpawnPoints for enemy positions
            Vector3 spawnPos = enemySpawnPoints[randomSpawnIndex].position;
            Quaternion spawnRot = Quaternion.identity;

            string randomEnemyPrefabPath = enemyCharacterPrefabPaths[rnd.Next(0, enemyCharacterPrefabPaths.Length)];

            Debug.Log($"GameManager: Spawning enemy {randomEnemyPrefabPath} at {spawnPos}");
            PhotonNetwork.Instantiate(randomEnemyPrefabPath, spawnPos, spawnRot);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} đã vào phòng. Hiện có {PhotonNetwork.CurrentRoom.PlayerCount} người.");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} đã rời phòng. Hiện còn {PhotonNetwork.CurrentRoom.PlayerCount} người.");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Bạn đã rời khỏi phòng. Đang quay về Scene Lobby.");
        hasSpawnedLocalPlayer = false;
        hasMasterClientSpawnedEnemies = false;
        LeaveRoomAndGoToLobby();
    }

    public void LeaveMatch()
    {
        Debug.Log("Đang yêu cầu PhotonNetwork rời phòng...");
        PhotonNetwork.LeaveRoom();
    }

    private void LeaveRoomAndGoToLobby()
    {
        SceneManager.LoadScene(lobbySceneName);
    }
}