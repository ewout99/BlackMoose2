using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Wall : NetworkBehaviour {

    private AudioSource aSRef;
    public AudioClip spawnSound;

    void Start()
    {
        aSRef.clip = spawnSound;
        aSRef.Play();
    }

	void Ondestroy()
    {
        if (isServer)
            RpcCopy();       
    }

    [ClientRpc]
    void RpcCopy()
    {
        FindObjectOfType<InputOracle>().RemovePosFromUsed(transform.position);
    }
}
