using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class IngameEnemy: NetworkBehaviour {

    [SerializeField]
    private GameObject orbRef;
    private bool spawned = false;

    void Update()
    {
        if (isServer && GetComponent<Entity>().deathState && !spawned)
        {
            CmdSpawnOrb();
        }
    }

    [Command]
    private void CmdSpawnOrb()
    {
        spawned = true;
        GameObject orb = Instantiate(orbRef, transform.position, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(orb);
    }
}
