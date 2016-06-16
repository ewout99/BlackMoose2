using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HealthOrb : NetworkBehaviour{

    [SerializeField]
    private float healAmount = 10;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!isServer)
        {
            return;
        }

        if (col.collider.tag == "Player" && col.collider.GetComponent<IngamePlayer>())
        {
            Debug.Log("Healing player");
            col.collider.GetComponent<Entity>().CmdAddHealth(healAmount);
            GetComponent<MovementOrb>().CmdDestroyGameObject();
        }
        else
        {
            Debug.Log("Im not healing you: " + col.collider.name);
        }
    }
}
