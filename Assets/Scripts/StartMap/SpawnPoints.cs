using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public Vector2[] spawnPos;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < spawnPos.Length; i++)
        {
            Gizmos.DrawWireSphere(spawnPos[i], 0.5f);
        }
    }
}
