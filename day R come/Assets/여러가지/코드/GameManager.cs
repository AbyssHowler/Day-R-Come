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
        // "Player" �±׸� ���� ��� ������Ʈ�� �迭�� ������
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
            string message = "����� ���ù޾ҽ��ϴ�!";
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
        mainText.text = message + "\n�ٸ� �÷��̾ ��� ���̼���!";
        // Send message logic
    }
     if (!spawnedPlayers.Contains(player))
    {
        mainText.text = "�����ڸ� ���� ������ ã������!";
        // Send message logic
    }
    
      StartCoroutine(GameTimer(5.0f)); 
}

    [PunRPC]
    void SwitchPlayerToMonster(Photon.Realtime.Player selectedPlayer)
    {
        // ���õ� �÷��̾ ���ͷ� ��ȯ
        if (spawnedPlayers.Contains(selectedPlayer))
        {
            int playerIndex = spawnedPlayers.IndexOf(selectedPlayer);
            if (playerIndex < spawnPoints.Length)
            {
                Vector3 posit = spawnPoints[playerIndex + 1].position; // Ȥ�� �ٸ� ������ ���� ����
                PhotonNetwork.Destroy(playerPrefab);
                playerPrefab = PhotonNetwork.Instantiate("Enemy B", posit, Quaternion.identity, 0);
            }
        }
    }
    IEnumerator GameTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

       timer.enabled=true;

        //1�� 30�� ��Ƽ�� �÷��̾� �� ������ ������ �� ���� Ÿ�̸� �����ϱ� ����
    }
    IEnumerator delaygameover(){
        yield return new WaitForSeconds(3f);

         GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // ���� "Player" �±׸� ���� ������Ʈ�� 0���̸�
        if (players.Length == 0)
        {
            // ���� ���� �Լ� ȣ��
            monsterwin();
        }
    }
    [PunRPC]
    void monsterwin(){
        timer.enabled=false;
        mainText.text = "�����ڵ��� ��� �������ϴ�... \n���� �¸�!";
        gameover.SetActive(true);
        
    }

    IEnumerator CreatePlayer()
    {
        yield return new WaitForSeconds(1.0f);
        spawnPoints = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        Vector3 posit = spawnPoints[PhotonNetwork.CurrentRoom.PlayerCount].position;

        playerPrefab = PhotonNetwork.Instantiate("Player", posit, Quaternion.identity, 0);
        spawnedPlayers.Add(PhotonNetwork.LocalPlayer);
        Debug.Log("ĳ���� ������");
    }
}