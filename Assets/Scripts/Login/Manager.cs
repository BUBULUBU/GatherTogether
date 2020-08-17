using Photon.Pun;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public string player_prefab;
    public SpawnPoints spawnPoints;

    private void Start()
    {
        Spawn();
    }

    private void Spawn()
    {
        int posCount = Random.Range(0, spawnPoints.spawnPos.Length);

        PhotonNetwork.Instantiate(player_prefab, spawnPoints.spawnPos[posCount], Quaternion.identity);
    }
}
