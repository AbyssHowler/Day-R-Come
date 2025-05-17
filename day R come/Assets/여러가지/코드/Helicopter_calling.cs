using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter_calling : MonoBehaviour
{
    public GameObject gameobject1;
     public GameObject gameobject2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    //콜라이더에 플레이어 태그를 가진 오브젝트가 감지된 경우
    {
     gameObject.GetComponent<BoxCollider>().enabled = false;
          //플레이어를 감지할 콜라이더를 비활성화. 콜라이더는 트리거 설정되어있음
           gameobject2.gameObject.SetActive(false);
           //안내박스 비활성화
           
          //헬기 호출, 가능하면 ui상에서 "그들이 옵니다.... 헬기가 올때까지 살아남으세요!" 출력
          gameobject1.GetComponent<Timer>().enabled = true;
          //좀비스포너 가동 타이머 가동.
      
    }
}
}
