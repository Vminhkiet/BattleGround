using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakingManager : MonoBehaviourPunCallbacks
{
    [Tooltip("S? l??ng ng??i ch?i t?i thi?u ?? b?t ??u game.")]
    public byte minPlayersToStartGame = 2;
    private bool isInRoomAndWaiting;
    [Header("Network Settings")]
    [Tooltip("Phiên b?n game c?a b?n. Quan tr?ng ?? phân tách các b?n build khác nhau.")]
    public string gameVersion = "1.0";
    [Tooltip("S? l??ng ng??i ch?i t?i ?a trong m?t phòng Battle Royale.")]
    public byte maxPlayersPerRoom = 20;
    [Tooltip("Tên Scene c?a b?n ?? Battle Royale s? ???c t?i khi vào phòng.")]
    public string gameSceneName = "Testscene";

    [Header("UI Elements")]
    [Tooltip("Nút 'Play' ho?c 'Tìm tr?n' mà ng??i ch?i s? nh?n.")]
    public Button playButton;
    [Tooltip("Nút 'H?y tìm tr?n' mà ng??i ch?i s? nh?n ?? ng?t k?t n?i ho?c r?i phòng. Nên ??t nút này LÀ CON C?A LOADING PANEL.")]
    public Button cancelMatchmakingButton;
    [Tooltip("Panel UI s? hi?n th? khi ?ang trong quá trình tìm tr?n/k?t n?i.")]
    public GameObject loadingPanel;
    private bool isConnecting;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }
        if (cancelMatchmakingButton != null)
        {
            cancelMatchmakingButton.onClick.AddListener(OnCancelMatchmakingClicked);
            cancelMatchmakingButton.interactable = false;
        }

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }


    public void OnPlayButtonClicked()
    {
        if (playButton != null) playButton.interactable = false;
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        if (cancelMatchmakingButton != null)
        {
            cancelMatchmakingButton.interactable = true;
        }

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("?ã k?t n?i ??n Photon. ?ang tham gia phòng ng?u nhiên...");
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            Debug.Log("Ch?a k?t n?i. ?ang k?t n?i ??n Photon Cloud...");
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public void OnCancelMatchmakingClicked()
    {
        Debug.Log("Nút 'H?y tìm tr?n' ?ã ???c nh?n.");

        if (cancelMatchmakingButton != null)
        {
            cancelMatchmakingButton.interactable = false;
        }
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
        if (playButton != null) playButton.interactable = true;

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("?ang r?i phòng...");
            PhotonNetwork.LeaveRoom();
        }
        else if (PhotonNetwork.IsConnected)
        {
            Debug.Log("?ang ng?t k?t n?i kh?i Photon Cloud...");
            PhotonNetwork.Disconnect();
        }
        else
        {
            Debug.Log("?ang ng?t quá trình k?t n?i ban ??u...");
            PhotonNetwork.Disconnect();
        }

        isConnecting = false;
        isInRoomAndWaiting = false;
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("?ã k?t n?i ??n Master Server.");
        if (isConnecting)
        {
            PhotonNetwork.JoinLobby();
            isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"M?t k?t n?i: {cause}");
        if (playButton != null) playButton.interactable = true;
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
        if (cancelMatchmakingButton != null)
        {
            cancelMatchmakingButton.interactable = false;
        }
        isConnecting = false;
        isInRoomAndWaiting = false;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("?ã tham gia Lobby.");

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Không tìm th?y phòng ng?u nhiên: {message}. ?ang t?o phòng m?i...");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayersPerRoom;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;


        string roomName = "BR_Room_" + Random.Range(1000, 9999);
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"?ã t?o phòng: {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"?ã tham gia phòng: {PhotonNetwork.CurrentRoom.Name}. Ng??i ch?i: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");

        if (playButton != null) playButton.interactable = true;

        CheckAndStartGame();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} ?ã vào phòng. T?ng ng??i ch?i: {PhotonNetwork.CurrentRoom.PlayerCount}");
        CheckAndStartGame();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} ?ã r?i phòng. T?ng ng??i ch?i: {PhotonNetwork.CurrentRoom.PlayerCount}");
    }

    private void CheckAndStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount >= minPlayersToStartGame)
            {
                Debug.Log($"Master Client: ?? ng??i ch?i ({PhotonNetwork.CurrentRoom.PlayerCount}/{minPlayersToStartGame}). ?ang t?i Scene Battle Royale...");
                PhotonNetwork.LoadLevel(gameSceneName);
                if (loadingPanel != null)
                {
                    loadingPanel.SetActive(false);
                }
                if (cancelMatchmakingButton != null)
                {
                    cancelMatchmakingButton.interactable = false;
                }
                isInRoomAndWaiting = false;
            }
            else
            {
                Debug.Log($"Master Client: ?ang ch? thêm ng??i ch?i. Hi?n t?i có {PhotonNetwork.CurrentRoom.PlayerCount}/{minPlayersToStartGame} ng??i.");
                isInRoomAndWaiting = true;
            }
        }
        else
        {
            Debug.Log($"Client: ?ã vào phòng và ?ang ch? Master Client t?i scene. Hi?n t?i có {PhotonNetwork.CurrentRoom.PlayerCount}/{minPlayersToStartGame} ng??i.");
            isInRoomAndWaiting = true;
        }
    }
}
