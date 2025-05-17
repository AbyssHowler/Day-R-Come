using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item :MonoBehaviour
{
    public float rotationSpeed = 30.0f;
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon,Craft};
    public Type type;
    public int value;

    // Update �Լ��� Ŭ���� ���η� �̵�
    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}  