using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;
    public float countdownTime = 180.0f; // 3���� �ʷ� ǥ���� ��
    public GameObject gameobject1;

    public float currentTime;

    private void Start()
    {
        currentTime = countdownTime;
    }

    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            // �ð��� �а� �ʷ� �����Ͽ� ǥ��
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);

            // �ð��� 00:00 �������� ǥ��
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            // �ð��� �� �������� ���ϴ� ������ �����ϰų� Ÿ�̸Ӹ� ����ϴ�.
            timerText.text = "���� ����! �����ڵ��� Ż���߽��ϴ�!";
            // ���⼭ �ٸ� ������ �����ϰų� Ÿ�̸Ӹ� ���� �� �ֽ��ϴ�.
            gameobject1.gameObject.SetActive(true);
        }
    }
}
