using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Revive: NetworkBehaviour
{
    private AudioSource aSRef;
    public AudioClip spawnSound;

    void Start()
    {
        aSRef = GetComponent<AudioSource>();
        aSRef.clip = spawnSound;
        aSRef.Play();
    }

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
