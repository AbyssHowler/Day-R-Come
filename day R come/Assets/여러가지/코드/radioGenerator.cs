using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class radioGenerator : MonoBehaviourPun
{
    public GameObject radioPrefab; // ���� ������
    public Transform[] spawnPoints; // ���� ����Ʈ �迭

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // ������ Ŭ���̾�Ʈ�� ������ �����մϴ�.
            SpawnRadios();
        }
    }

    void SpawnRadios()
    {
        // �����ϰ� 5���� ���� ����Ʈ�� �����Ͽ� ������ �����մϴ�.
        int radiosToSpawn = 5;
        int[] selectedIndices = GetRandomIndices(spawnPoints.Length, radiosToSpawn);

        for (int i = 0; i < radiosToSpawn; i++)
        {
            int spawnIndex = selectedIndices[i];
            Vector3 spawnPosition = spawnPoints[spawnIndex].position;
            Quaternion spawnRotation = Quaternion.identity;

            // PhotonNetwork.Instantiate�� ����Ͽ� ������ �����ϰ�, �ٸ� �÷��̾�� ����ȭ�մϴ�.
            PhotonNetwork.Instantiate(radioPrefab.name, spawnPosition, spawnRotation);
        }
    }

    // 0���� count - 1 ������ ������ �ε��� �迭�� ��ȯ�մϴ�.
  int[] GetRandomIndices(int count, int numIndices)
{
    int[] indices = new int[count];
    for (int i = 0; i < count; i++)
    {
        indices[i] = i;
    }

    
    for (int i = 0; i < count - 1; i++)
    {
        int randomIndex = Random.Range(i, count);
        int temp = indices[i];
        indices[i] = indices[randomIndex];
        indices[randomIndex] = temp;
    }

    // ó�� numIndices ��ŭ�� �ε����� ��ȯ�մϴ�.
    int[] selectedIndices = new int[numIndices];
    System.Array.Copy(indices, selectedIndices, numIndices);
    return selectedIndices;
}
}