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
    //�ݶ��̴��� �÷��̾� �±׸� ���� ������Ʈ�� ������ ���
    {
     gameObject.GetComponent<BoxCollider>().enabled = false;
          //�÷��̾ ������ �ݶ��̴��� ��Ȱ��ȭ. �ݶ��̴��� Ʈ���� �����Ǿ�����
           gameobject2.gameObject.SetActive(false);
           //�ȳ��ڽ� ��Ȱ��ȭ
           
          //��� ȣ��, �����ϸ� ui�󿡼� "�׵��� �ɴϴ�.... ��Ⱑ �ö����� ��Ƴ�������!" ���
          gameobject1.GetComponent<Timer>().enabled = true;
          //�������� ���� Ÿ�̸� ����.
      
    }
}
}
