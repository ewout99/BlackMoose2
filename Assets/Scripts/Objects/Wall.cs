using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Wall : NetworkBehaviour {

	void Ondestroy()
    {
        if (isServer)
            FindObjectOfType<InputOracle>().RemovePosFromUsed(transform.position);
    }
}
