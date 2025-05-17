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
        diceVelocity = rb.velocity; // �������� �ִ��� Ȯ���ϱ� ���� ����
    }

    public void DiceRoll()
    {
        if (diceVelocity != Vector3.zero)
        {
            Debug.Log("�ֻ����� �̹� �������� �ֽ��ϴ�. �ٽ� ���� �� �����ϴ�.");
            return;
        }
            // ȸ�� ���� ����
            float dirX = Random.Range(0, 3000);
        float dirY = Random.Range(0, 3000);
        float dirZ = Random.Range(0, 3000);

        // �ʱ� ȸ���� (���� �ο�)
        Quaternion currentRotation = transform.localRotation;
        int randomIndex_x = Random.Range(0, angles.Length);
        int randomIndex_z = Random.Range(0, angles.Length);

        // �ʱ� ��ġ�� ȸ�� �ʱ�ȭ
        transform.localPosition = new Vector3(0, 1, 0); // Y���� �ణ �÷��� ���鿡 ��ġ�� �ʰ� ��
        transform.localRotation = Quaternion.Euler(angles[randomIndex_x], currentRotation.eulerAngles.y, angles[randomIndex_z]);

        // �� �������� ���� ���� ���߿� ���鼭, ������ �������� ȸ��
        float ForceRand = Random.Range(700, 1000);
        rb.AddForce(Vector3.up * ForceRand);
        rb.AddTorque(new Vector3(dirX, dirY, dirZ), ForceMode.VelocityChange);
    }
}