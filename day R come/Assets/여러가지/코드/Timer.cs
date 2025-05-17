using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;
    public float countdownTime = 180.0f; // 3분을 초로 표현한 값
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

            // 시간을 분과 초로 분할하여 표시
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);

            // 시간을 00:00 형식으로 표시
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            // 시간이 다 떨어지면 원하는 동작을 수행하거나 타이머를 멈춥니다.
            timerText.text = "게임 종료! 생존자들이 탈출했습니다!";
            // 여기서 다른 동작을 수행하거나 타이머를 멈출 수 있습니다.
            gameobject1.gameObject.SetActive(true);
        }
    }
}
