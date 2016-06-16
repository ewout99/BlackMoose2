using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Wall : NetworkBehaviour {

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
