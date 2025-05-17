using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceScript : MonoBehaviour
{
    static Rigidbody rb;
    public static Vector3 diceVelocity;

    private int[] angles = { 0, 90, 180, 270, 360 };

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        diceVelocity = rb.velocity; // 움직임이 있는지 확인하기 위한 변수
    }

    public void DiceRoll()
    {
        if (diceVelocity != Vector3.zero)
        {
            Debug.Log("주사위가 이미 굴러가고 있습니다. 다시 굴릴 수 없습니다.");
            return;
        }
            // 회전 랜덤 변수
            float dirX = Random.Range(0, 3000);
        float dirY = Random.Range(0, 3000);
        float dirZ = Random.Range(0, 3000);

        // 초기 회전값 (랜덤 부여)
        Quaternion currentRotation = transform.localRotation;
        int randomIndex_x = Random.Range(0, angles.Length);
        int randomIndex_z = Random.Range(0, angles.Length);

        // 초기 위치와 회전 초기화
        transform.localPosition = new Vector3(0, 1, 0); // Y축을 약간 올려서 지면에 겹치지 않게 함
        transform.localRotation = Quaternion.Euler(angles[randomIndex_x], currentRotation.eulerAngles.y, angles[randomIndex_z]);

        // 윗 방향으로 힘을 가해 공중에 띄우면서, 랜덤한 방향으로 회전
        float ForceRand = Random.Range(700, 1000);
        rb.AddForce(Vector3.up * ForceRand);
        rb.AddTorque(new Vector3(dirX, dirY, dirZ), ForceMode.VelocityChange);
    }
}