using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "v1.0";
    private string userId = "devPlayer";

    public TMP_InputField userIdText;
    public TMP_InputField roomNameText;
    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();
    public GameObject roomPrefeb;
    public Transform scrollContent;
    public TMP_Dropdown gameTypeDropdown;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        Debug.Log("00.포톤매니저 시작");
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(0, 100):00}");
        userIdText.text = userId;
        PhotonNetwork.NickName = userId;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("01.포톤서버에 접속");
        PhotonNetwork.JoinLobby();
    }

    public void OnExitBut()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("start");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("02. 로비에 접속");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("02. 랜덤 룸 접속 실패");

        // 룸 속성 설정
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 5;

        int gameType = gameTypeDropdown.value + 1; // Dropdown에서 선택된 게임 타입 (1, 2, 3)

        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["gameType"] = gameType;
        ro.CustomRoomProperties = customProperties;

        roomNameText.text = $"Room_{Random.Range(1, 100):000}";

        PhotonNetwork.CreateRoom(roomNameText.text, ro);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("03. 방 생성 완료");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("04.방 입장 완료");

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameType", out object gameType))
        {
            Debug.Log($"Game Type: {gameType}");
            LoadGameScene((int)gameType);
        }
    }

    private void LoadGameScene(int gameType)
    {
        switch (gameType)
        {
            case 1:
                PhotonNetwork.LoadLevel("MainGame1");
                break;
            case 2:
                PhotonNetwork.LoadLevel("MainGame2");
                break;
            case 3:
                PhotonNetwork.LoadLevel("MainGame3");
                break;
            default:
                Debug.LogError("Unknown game type");
                break;
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;
        Debug.Log("05.방 목록 생성 시도중");

        foreach (var room in roomList)
        {
            if (room.RemovedFromList == true)
            {
                roomDict.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(room.Name);
                Debug.Log("06.삭제완료");
            }
            else
            {
                if (roomDict.ContainsKey(room.Name) == false)
                {
                    GameObject _room = Instantiate(roomPrefeb, scrollContent);
                    _room.GetComponent<RoomData>().RoomInfo = room;
                    roomDict.Add(room.Name, _room);
                    Debug.Log("06.생성완료");
                }
                else
                {
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = room;
                    Debug.Log("06.완료");
                }
            }
        }
    }

    #region UI_BUTTON_CALLBACK
    public void OnRandomButton()
    {
        if (string.IsNullOrEmpty(userIdText.text))
        {
            userId = $"USER_{Random.Range(0, 100):00}";
            userIdText.text = userId;
        }
        PlayerPrefs.SetString("USER_ID", userIdText.text);
        PhotonNetwork.NickName = userIdText.text;
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoomClick()
    {
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 5;

        if (string.IsNullOrEmpty(roomNameText.text))
        {
            roomNameText.text = $"Room_{Random.Range(1, 100):000}";
        }

        int gameType = gameTypeDropdown.value + 1; // Dropdown에서 선택된 게임 타입 (1, 2, 3)

        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["gameType"] = gameType;
        ro.CustomRoomProperties = customProperties;

        PhotonNetwork.CreateRoom(roomNameText.text, ro);
    }
    #endregion
}
