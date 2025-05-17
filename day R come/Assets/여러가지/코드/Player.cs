using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{
    //private Transform canvasTransform;

    private PhotonView photonview;
    public Button jbutton;
    public Button abutton;
    public Button ebutton;
    public float speed;
    float hAxis;
    float vAxis;
    public VariableJoystick variableJoystick;
    Vector3 moveVec;
    Rigidbody rigid;
    Animator anim;

        AudioSource audio;


    GameObject nearObject;
    bool wDown;
    bool jDown;

    bool isJump;
    public bool isAttack;
    public GameObject[] weapons;
    public bool[] hasweapons;
    public int lookSensitivity = 5;
    private bool isRun = false;
    public bool isDead = false;
    public Collider Attackarea;
    public Text mainText;
    public GameObject Mlight;
    public Timer timer;
     public GameObject camera ; 
    public AudioClip run;
        public AudioClip jump;
        public AudioClip HeartBit;
        Vector3 inputDirection;




    // Start is called before the first frame update
    IEnumerator Start()
    {
        

        photonview = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        audio=GetComponent<AudioSource>();
        yield return new WaitForSeconds(0.5f);
        mainText = GameObject.Find("mainText").GetComponent<Text>();
        timer=GameObject.Find("Timer").GetComponent<Timer>();
        jbutton = GameObject.Find("JUMP").gameObject.GetComponent<Button>();
        jbutton.onClick.AddListener(JumpButtonClick);
       Mlight=GameObject.Find("DLight");
        if (gameObject.CompareTag("Monster"))
        { Mlight.GetComponent<Light>().enabled=true; } // 공유하지 않는 변수 찾기 
        else{Mlight.GetComponent<Light>().enabled=false;}

        abutton = GameObject.Find("Attack").gameObject.GetComponent<Button>();
        abutton.onClick.AddListener(Attack);
        ebutton = GameObject.Find("Exit").gameObject.GetComponent<Button>();
        ebutton.onClick.AddListener(Leave);
        variableJoystick = FindObjectOfType<VariableJoystick>();
        if (photonview.IsMine)
        {
            if (gameObject.CompareTag("Player"))
            {
                Camera.main.GetComponent<Follow>().target = transform.Find("CamPivot").transform;
            }
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }

    }
    // Update is called once per frame
    void Update()
    {

        if (photonview.IsMine)
        {


            escape();
            GetInput();
            TurnAndMove();
            Jump();
            
        }
        if (gameObject.CompareTag("Monster"))
        {
            // 포톤뷰가 내가 조종하는지 확인
            if (!photonview.IsMine)
            { 
                   AudioListener audioListener = camera.GetComponent<AudioListener>();
                    audioListener.enabled = false;
                
            }else{ photonview.RPC("RPC_Hearthbit", RpcTarget.Others);}
        }
    }
    [PunRPC]
    private void RPC_Hearthbit()
    {
        if(!audio.isPlaying){
        audio.clip = HeartBit;
        audio.Play();}
    }
    void JumpButtonClick()
    {
        if (photonview.IsMine)
        {
            if (!isJump) // isJump bool 변수로, 점프 중인지 여부를 나타냅니다.
            {
                if (gameObject.CompareTag("Player")) { anim.SetTrigger("doDodge"); }
                speed *= 2;
                rigid.AddForce(Vector3.up * 30, ForceMode.Impulse);


                isJump = true; // 점프 중으로 플래그 설정
                speed /= 2;
            }
        }

    }

    IEnumerator Activate()
    {

        yield return new WaitForSeconds(2.0f); // 원하는 시간으로 수정
        Attackarea.enabled = false;
        isAttack = false;
    }

    void Attack()
    {
        if (photonview.IsMine)
        {
            if (gameObject.CompareTag("Monster") && !isAttack)
            {
                isAttack = true;
                anim.SetTrigger("doAttack");
                Attackarea.enabled = true;
                StartCoroutine(Activate());

            }
        }
    }
    void Cammake(){}
    void GetInput()
    {

        GameObject.Find("controller").SetActive(true);
        hAxis = variableJoystick.Horizontal;
        vAxis = variableJoystick.Vertical;

        jDown = Input.GetButtonDown("Jump");

    }

    void TurnAndMove()
    {
        /*if (Input.GetMouseButton(0) && Input.mousePosition.x > Screen.width / 2)
        {
           
          
            float _yRotation = Input.GetAxisRaw("Mouse X");
            Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
            rigid.MoveRotation(rigid.rotation * Quaternion.Euler(_characterRotationY));
        }*/
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.phase == TouchPhase.Moved && touch.position.x > Screen.width / 2)
            {
                float yRotation = touch.deltaPosition.x;
                Vector3 characterRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;
                rigid.MoveRotation(rigid.rotation * Quaternion.Euler(characterRotationY));
            }
        }

        Vector3 forward = transform.forward;
        inputDirection = forward * vAxis + transform.right * hAxis;
        transform.position += inputDirection * speed * Time.deltaTime;

        anim.SetBool("isRun", inputDirection != Vector3.zero);
        if(inputDirection != Vector3.zero){
            PlayFootstepSound();
        }else{audio.Stop();}
    }
    private void PlayFootstepSound()
    {
        // 발소리 재생 코드
        if(!audio.isPlaying){
        audio.clip = run;
        audio.Play();}  
        // 발소리를 다른 플레이어에게 전송
         if (gameObject.CompareTag("Player")){
        photonview.RPC("RPC_PlayFootstepSound", RpcTarget.Others);}
    }

    // RPC 함수: 발소리를 다른 플레이어에게 전송하기 위한 함수
    [PunRPC]
    private void RPC_PlayFootstepSound()
    {
        if(!audio.isPlaying){
        audio.clip = run;
        audio.Play();}
    }



    public void Jump()
    {

        if (jDown && !isJump) // isJump bool 변수로, 점프 중인지 여부를 나타냅니다.
        {
            audio.clip=jump;
            audio.Play();
             if (gameObject.CompareTag("Player")){ photonview.RPC("RPC_PlayjumpSound", RpcTarget.Others);}
            
            if (gameObject.CompareTag("Player")) { anim.SetTrigger("doDodge"); }
            speed *= 2;
            rigid.AddForce(Vector3.up * 30, ForceMode.Impulse);



            isJump = true; // 점프 중으로 플래그 설정
            speed /= 2;

        }


    }
        [PunRPC]
    private void RPC_PlayjumpSound()
    {
        if(!audio.isPlaying){
        audio.clip = jump;
        audio.Play();}
    }
    void escape()
    {
        if (transform.position.y < -10f)
        {
            // 플레이어의 좌표를 0, 10, 0으로 이동
            transform.position = new Vector3(0f, 10f, 0f);
        }
    }



    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;


        }
      if (collision.gameObject.tag == "radio"&&gameObject.tag == "Player")
    {
        Debug.Log("충돌함");
         int targetViewID = collision.gameObject.GetPhotonView().ViewID;
        ApplyRadioCollision(targetViewID);
    }

    }

    //이부분 따로 처리 안함 멀티화
    /* void OnTriggerStay(Collider other)
     {
         if (other.tag == "Weapon") { nearObject = other.gameObject; }
         Debug.Log("접촉함");
         Debug.Log(nearObject.name);
     }
     void OnTriggerExit(Collider other)
     {
         if (other.tag == "Weapon") { nearObject = null; }
     }*/
void OnTriggerEnter(Collider other)
{
     if (other.CompareTag("Player"))
    {
        PhotonView otherPhotonView = other.GetComponent<PhotonView>();

        // 상대 플레이어에게 RPC 호출
        photonview.RPC("DestroyObject", otherPhotonView.Owner, otherPhotonView.ViewID);
    }
       
}

    void Interation()
    {
        if (nearObject != null && !isJump)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasweapons[weaponIndex] = true;
                Destroy(nearObject);
            }
        }
    }
    //여기까지 처리안함
    void Leave()
    {
        Debug.Log("방을 나가 로비로 돌아갑니다.");


        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Lobby");

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 다른 플레이어에게 애니메이션 상태를 보냅니다.
            stream.SendNext(isRun);
        }
        else
        {
            // 다른 플레이어로부터 애니메이션 상태를 받습니다.
            isRun = (bool)stream.ReceiveNext();
            anim.SetBool("isRun", isRun);
        }
    }
  

    [PunRPC]
  void ApplyRadioCollision(int targetViewID)
{
    if (photonview.IsMine)
    {
        timer.currentTime -= 10;
        photonview.RPC("SyncTimer", RpcTarget.AllBuffered, timer.currentTime);
        PhotonView targetPhotonView = PhotonView.Find(targetViewID);    
        photonview.RPC("DestroyObject", RpcTarget.MasterClient, targetViewID);

        // 마스터 클라이언트에게 파괴 명령을 보냅니다.
       
    }
}
[PunRPC]
void DestroyObject(int viewID)
{
    PhotonView targetPhotonView = PhotonView.Find(viewID);

    
        // 마스터 클라이언트가 소유한 오브젝트를 파괴
        PhotonNetwork.Destroy(targetPhotonView.gameObject);
}
    [PunRPC]
    void SyncTimer(float updatedTime)
    {
        timer.currentTime = updatedTime;
    }
}