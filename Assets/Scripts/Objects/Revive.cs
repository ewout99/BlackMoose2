using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Revive: NetworkBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!isServer)
        {
            return;
        }

        if (col.collider.tag == "Player" && col.collider.GetComponent<IngamePlayer>() && col.collider.GetComponent<Entity>().deathState)
        {
            Debug.Log("Reviving player");
            col.collider.GetComponent<Entity>().CmdRevive();
            GetComponent<MovementOrb>().CmdDestroyGameObject();
        }
        else
        {
            Debug.Log(col.collider.tag);
            Debug.Log("IngamePlayer = " + col.collider.GetComponent<IngamePlayer>());
            Debug.Log("DeathStae = " + col.collider.GetComponent<Entity>().deathState);
            Debug.Log("Im not reviving you: " + col.collider.name);
        }
    }
}
