using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class IngameOrb : NetworkBehaviour {

    public float fluxValue = 5;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!isServer)
        {
            return;
        }

        if (col.collider.tag == "Player" && col.collider.GetComponent<IngamePlayer>())
        {
            Debug.Log("Flux for player");
            col.collider.GetComponent<IngamePlayer>().CmdAddFlux(fluxValue);
            GetComponent<MovementOrb>().CmdDestroyGameObject();
        }

        if(col.collider.tag == "Player" && col.collider.GetComponent<IngameOracle>())
        {
            Debug.Log("Flux for Oracle");
            col.collider.GetComponent<IngameOracle>().CmdAddFlux(fluxValue);
            GetComponent<MovementOrb>().CmdDestroyGameObject();
        }
        else
        {
            Debug.Log("Not A flux target: " + col.collider.tag);
        }
    }
}
