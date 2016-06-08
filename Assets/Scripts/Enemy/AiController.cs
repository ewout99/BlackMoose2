using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AiController : NetworkBehaviour {

    // The Enemy Spawners
    private GameObject[] Spawners;

    // All the currently conneted players
    private GameObject[] Players;

    //Private Ref
    private GameObject PathfinderRef;

    // Use this for initialization
    void Start ()
    {
        // Enable random spawnpoints
        PathfinderRef = GameObject.Find("PathFinder");
        PathfinderRef.GetComponent<AstarPath>().Scan();
    }
	
	// Update is called once per frame
	void Update () {
        if (!isServer)
        {
            return;
        }


        // Check for priorities
	}

    // Up the priority of a player
    public void UpPriority()
    {

    }

    // Lower priority of a player
    public void DownPriorty()
    {

    }

    // Get a target to attack based on priority system
    public GameObject GetTarget()
    {
        Players = GameObject.FindGameObjectsWithTag("Player");
        if (Players.Length > 0)
        {
            GameObject pTarget = Players[Random.Range(0, Players.Length)];
            return pTarget;
        }
        else
        {
            Debug.LogError("No players in scene");
            return null;
        }
    }

    // Disable spawnPoint && activate random other point
    public void DisablePoint()
    {

    }
}
