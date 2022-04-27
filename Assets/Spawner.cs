using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Spawner : NetworkBehaviour
{
    public GameObject enemy;
    [ServerCallback]
    private void Start()
    {
        GameObject enemyInit = Instantiate(enemy);
        NetworkServer.Spawn(enemyInit);
    }
}
