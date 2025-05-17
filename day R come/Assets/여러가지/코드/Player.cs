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
        { Mlight.GetComponent<Light>().enabled=true; } // �������� �ʴ� ���� ã�� 
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
            // ����䰡 ���� �����ϴ��� Ȯ��
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
            if (!isJump) // isJump bool ������, ���� ������ ���θ� ��Ÿ���ϴ�.
            {
                if (gameObject.CompareTag("Player")) { anim.SetTrigger("doDodge"); }
                speed *= 2;
                rigid.AddForce(Vector3.up * 30, ForceMode.Impulse);


                isJump = true; // ���� ������ �÷��� ����
                speed /= 2;
            }
        }

    }

    IEnumerator Activate()
    {

        yield return new WaitForSeconds(2.0f); // ���ϴ� �ð����� ����
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
        // �߼Ҹ� ��� �ڵ�
        if(!audio.isPlaying){
        audio.clip = run;
        audio.Play();}  
        // �߼Ҹ��� �ٸ� �÷��̾�� ����
         if (gameObject.CompareTag("Player")){
        photonview.RPC("RPC_PlayFootstepSound", RpcTarget.Others);}
    }

    // RPC �Լ�: �߼Ҹ��� �ٸ� �÷��̾�� �����ϱ� ���� �Լ�
    [PunRPC]
    private void RPC_PlayFootstepSound()
    {
        if(!audio.isPlaying){
        audio.clip = run;
        audio.Play();}
    }



    public void Jump()
    {

        if (jDown && !isJump) // isJump bool ������, ���� ������ ���θ� ��Ÿ���ϴ�.
        {
            audio.clip=jump;
            audio.Play();
             if (gameObject.CompareTag("Player")){ photonview.RPC("RPC_PlayjumpSound", RpcTarget.Others);}
            
            if (gameObject.CompareTag("Player")) { anim.SetTrigger("doDodge"); }
            speed *= 2;
            rigid.AddForce(Vector3.up * 30, ForceMode.Impulse);



            isJump = true; // ���� ������ �÷��� ����
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
            // �÷��̾��� ��ǥ�� 0, 10, 0���� �̵�
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
        Debug.Log("�浹��");
         int targetViewID = collision.gameObject.GetPhotonView().ViewID;
        ApplyRadioCollision(targetViewID);
    }

    }

    //�̺κ� ���� ó�� ���� ��Ƽȭ
    /* void OnTriggerStay(Collider other)
     {
         if (other.tag == "Weapon") { nearObject = other.gameObject; }
         Debug.Log("������");
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

        // ��� �÷��̾�� RPC ȣ��
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
    //������� ó������
    void Leave()
    {
        Debug.Log("���� ���� �κ�� ���ư��ϴ�.");


        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Lobby");

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // �ٸ� �÷��̾�� �ִϸ��̼� ���¸� �����ϴ�.
            stream.SendNext(isRun);
        }
        else
        {
            // �ٸ� �÷��̾�κ��� �ִϸ��̼� ���¸� �޽��ϴ�.
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

        // ������ Ŭ���̾�Ʈ���� �ı� ����� �����ϴ�.
       
    }
}
[PunRPC]
void DestroyObject(int viewID)
{
    PhotonView targetPhotonView = PhotonView.Find(viewID);

    
        // ������ Ŭ���̾�Ʈ�� ������ ������Ʈ�� �ı�
        PhotonNetwork.Destroy(targetPhotonView.gameObject);
}
    [PunRPC]
    void SyncTimer(float updatedTime)
    {
        timer.currentTime = updatedTime;
    }
}