using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class radioGenerator : MonoBehaviourPun
{
    public GameObject radioPrefab; // 라디오 프리팹
    public Transform[] spawnPoints; // 스폰 포인트 배열

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트만 라디오를 스폰합니다.
            SpawnRadios();
        }
    }

    void SpawnRadios()
    {
        // 랜덤하게 5개의 스폰 포인트를 선택하여 라디오를 스폰합니다.
        int radiosToSpawn = 5;
        int[] selectedIndices = GetRandomIndices(spawnPoints.Length, radiosToSpawn);

        for (int i = 0; i < radiosToSpawn; i++)
        {
            int spawnIndex = selectedIndices[i];
            Vector3 spawnPosition = spawnPoints[spawnIndex].position;
            Quaternion spawnRotation = Quaternion.identity;

            // PhotonNetwork.Instantiate를 사용하여 라디오를 스폰하고, 다른 플레이어에게 동기화합니다.
            PhotonNetwork.Instantiate(radioPrefab.name, spawnPosition, spawnRotation);
        }
    }

    // 0부터 count - 1 사이의 랜덤한 인덱스 배열을 반환합니다.
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

    // 처음 numIndices 만큼의 인덱스를 반환합니다.
    int[] selectedIndices = new int[numIndices];
    System.Array.Copy(indices, selectedIndices, numIndices);
    return selectedIndices;
}
}