using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomData : MonoBehaviour
{
    private TMP_Text RoomInfoText;
    private RoomInfo _roomInfo;
    public TMP_InputField userIdText;

    public RoomInfo RoomInfo
    {
        get
        {
            return _roomInfo;
        }
        set
        {
            _roomInfo = value;
            string gameType = "Unknown";

            if (_roomInfo.CustomProperties.ContainsKey("gameType"))
            {
                gameType = _roomInfo.CustomProperties["gameType"].ToString();
            }

            RoomInfoText.text = $"{_roomInfo.Name} ({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers}) - Game Type: {gameType}";
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnEnterRoom(_roomInfo.Name));
        }
    }

    void Awake()
    {
        Debug.Log("방호출성공함!!! ");
        RoomInfoText = GetComponentInChildren<TMP_Text>();
        userIdText = GameObject.Find("InputField (TMP)-Nickname").GetComponent<TMP_InputField>();
    }

    void OnEnterRoom(string roomName)
    {
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 2;
        PhotonNetwork.NickName = userIdText.text;
        PhotonNetwork.JoinOrCreateRoom(roomName, ro, TypedLobby.Default);
    }
}