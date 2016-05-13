using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Entity : NetworkBehaviour {

    [SyncVar]
    public float healthPoints;

    public Animator aniRef;
	
	// Update is called once per frame
	void Update () {

        if (healthPoints <= 100)
        {
            CmdDeath();
        }
    }

    public void addHealth(float amount)
    {
        healthPoints += amount;
    }

    public void subtractHealth(float amount)
    {
        healthPoints -= amount;
    }

    [Command]
    private void CmdDeath()
    {
        // Play Sound

        // Play Animation

        // Destroy
        NetworkServer.Destroy(gameObject);
    }
}
