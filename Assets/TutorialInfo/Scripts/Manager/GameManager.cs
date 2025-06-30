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

    [Header("Game State")]
    [Tooltip("Tên Scene Lobby/Menu để quay về khi trận đấu kết thúc hoặc rời phòng.")]
    public string lobbySceneName = "Main";

    private bool hasSpawnedPlayer = false;
    private void Awake()
    {
        InfoPlayerDatabase info = Resources.Load<InfoPlayerDatabase>(playerInfo);
        CharacterPrefabCollection collection = Resources.Load<CharacterPrefabCollection>(linkCharacter);
        linkPlayer = collection.GetPrefabPath(info.getCharacterType());

        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogError("Không thể spawn Player: Chưa kết nối hoặc chưa sẵn sàng trong phòng Photon.");
            LeaveRoomAndGoToLobby();
            return;
        }

        if (string.IsNullOrEmpty(linkPlayer))
        {
            Debug.LogError("playerPrefabToSpawn chưa được xác định. Không thể spawn.");
            LeaveRoomAndGoToLobby();
            return;
        }

        Vector3 spawnPosition = Vector3.zero;
        if (playerSpawns != null && playerSpawns.Length > 0)
        {
            int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            if (spawnIndex >= 0 && spawnIndex < playerSpawns.Length)
            {
                spawnPosition = playerSpawns[spawnIndex].position;
            }
            else
            {
                Debug.LogWarning($"Spawn index {spawnIndex} is out of bounds for {playerSpawns.Length} spawn points. Choosing a random spawn point.");
                spawnPosition = playerSpawns[Random.Range(0, playerSpawns.Length)].position;
            }
        }
        else
        {
            Debug.LogWarning("Không có điểm spawn nào được gán cho Player. Spawn tại gốc tọa độ (0,0,0).");
        }

        GameObject myPlayerGameObject = PhotonNetwork.Instantiate(linkPlayer, spawnPosition, Quaternion.identity);

        Debug.Log($"Đã spawn Player: {myPlayerGameObject.name} cho {PhotonNetwork.LocalPlayer.NickName} tại vị trí {spawnPosition}");
        PhotonView pv = myPlayerGameObject.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            Camera playerCamera = myPlayerGameObject.GetComponentInChildren<Camera>(true);
            if (playerCamera != null)
            {
                playerCamera.gameObject.SetActive(true);
                if (Camera.main != null && Camera.main != playerCamera)
                {
                    Camera.main.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            Camera playerCamera = myPlayerGameObject.GetComponentInChildren<Camera>(true);
            if (playerCamera != null)
            {
                playerCamera.gameObject.SetActive(false);
            }
        }
    }

    public override void OnJoinedRoom()
    {

        if (!hasSpawnedPlayer && PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("GameManager.OnJoinedRoom() được gọi. Đang cố gắng SpawnPlayer.");
            SpawnPlayer();
            hasSpawnedPlayer = true;
        }
        else if (hasSpawnedPlayer)
        {
            Debug.Log("GameManager.OnJoinedRoom() được gọi nhưng người chơi đã được spawn rồi. Bỏ qua.");
        }
        else 
        {
            Debug.LogWarning("GameManager.OnJoinedRoom() được gọi nhưng Photon chưa sẵn sàng để spawn. Sẽ chờ.");
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
        Debug.Log("Bạn đã rời khỏi phòng.");
        LeaveRoomAndGoToLobby();
    }

    public void LeaveMatch()
    {
        Debug.Log("Đang rời trận đấu...");
        PhotonNetwork.LeaveRoom();
    }

    private void LeaveRoomAndGoToLobby()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(lobbySceneName);
        }
        else
        {
            SceneManager.LoadScene(lobbySceneName);
        }
    }
}
