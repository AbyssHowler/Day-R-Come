using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance = null;
    public Transform[] spawnPoints;
    public Button masterButton;
    private GameObject playerPrefab;
    private List<Photon.Realtime.Player> spawnedPlayers = new List<Photon.Realtime.Player>();
    public Text mainText;
    public bool Started= false;
    public GameObject gameover;
     public Timer timer;

    void Start()
    {
        timer=GameObject.Find("Timer").GetComponent<Timer>();
        mainText = GameObject.Find("mainText").GetComponent<Text>();
        StartCoroutine(CreatePlayer());

        if (PhotonNetwork.IsMasterClient)
        {
            masterButton.gameObject.SetActive(true);
        }
        else
        {
            masterButton.gameObject.SetActive(false);
        }
    }
    void Update(){
        // "Player" 태그를 가진 모든 오브젝트를 배열로 가져옴
         StartCoroutine(delaygameover());
    }

    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Lobby");
    }

    public void Startrandom()
    {
        Started=true;
        if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient)
        {
            return;
        }

        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        Photon.Realtime.Player randomPlayer = GetRandomPlayer(players);

        if (randomPlayer != null)
        {
            string message = "당신은 선택받았습니다!";
            photonView.RPC("SendRandomPlayerMessage", RpcTarget.All, randomPlayer, message);
            photonView.RPC("SwitchPlayerToMonster", RpcTarget.All, randomPlayer);
        }
        masterButton.gameObject.SetActive(false);
       
    }

    Photon.Realtime.Player GetRandomPlayer(Photon.Realtime.Player[] players)
    {
        if (players.Length > 0)
        {
            int randomIndex = Random.Range(0, players.Length);
            return players[randomIndex];
        }
        else
        {
            return null;
        }
    }

   [PunRPC]
void SendRandomPlayerMessage(Photon.Realtime.Player player, string message)
{
    if (spawnedPlayers.Contains(player))
    {
        mainText.text = message + "\n다른 플레이어를 모두 죽이세요!";
        // Send message logic
    }
     if (!spawnedPlayers.Contains(player))
    {
        mainText.text = "살인자를 피해 라디오를 찾으세요!";
        // Send message logic
    }
    
      StartCoroutine(GameTimer(5.0f)); 
}

    [PunRPC]
    void SwitchPlayerToMonster(Photon.Realtime.Player selectedPlayer)
    {
        // 선택된 플레이어만 몬스터로 전환
        if (spawnedPlayers.Contains(selectedPlayer))
        {
            int playerIndex = spawnedPlayers.IndexOf(selectedPlayer);
            if (playerIndex < spawnPoints.Length)
            {
                Vector3 posit = spawnPoints[playerIndex + 1].position; // 혹은 다른 로직에 따라 수정
                PhotonNetwork.Destroy(playerPrefab);
                playerPrefab = PhotonNetwork.Instantiate("Enemy B", posit, Quaternion.identity, 0);
            }
        }
    }
    IEnumerator GameTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

       timer.enabled=true;

        //1분 30초 버티면 플레이어 승 못버팀 술래승 그 뭐냐 타이머 실행하기 여따
    }
    IEnumerator delaygameover(){
        yield return new WaitForSeconds(3f);

         GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // 만약 "Player" 태그를 가진 오브젝트가 0개이면
        if (players.Length == 0)
        {
            // 게임 오버 함수 호출
            monsterwin();
        }
    }
    [PunRPC]
    void monsterwin(){
        timer.enabled=false;
        mainText.text = "생존자들이 모두 잡혔습니다... \n몬스터 승리!";
        gameover.SetActive(true);
        
    }

    IEnumerator CreatePlayer()
    {
        yield return new WaitForSeconds(1.0f);
        spawnPoints = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        Vector3 posit = spawnPoints[PhotonNetwork.CurrentRoom.PlayerCount].position;

        playerPrefab = PhotonNetwork.Instantiate("Player", posit, Quaternion.identity, 0);
        spawnedPlayers.Add(PhotonNetwork.LocalPlayer);
        Debug.Log("캐릭터 스폰됨");
    }
}