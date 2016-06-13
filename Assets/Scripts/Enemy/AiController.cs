using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AiController : NetworkBehaviour {

    // The Enemy Spawners
    private List<GameObject> SpawnersList = new List<GameObject>();
    private GameObject[] Spawners;

    // All the currently conneted players
    private GameObject[] Players;
    private Dictionary<GameObject, int> Priortity = new Dictionary<GameObject, int>();

    //Private Ref
    private GameObject PathfinderRef;

    private float minSpawningDistance = 5;
    private float maxSpawningDistance = 30;
    private int amountOfSquadrons = 4;
    private int activePoints;

    // Use this for initialization
    void Start ()
    {
        if (!isServer)
        {
            return;
        }
        PathfinderRef = GameObject.Find("PathFinder");
        PathfinderRef.GetComponent<AstarPath>().Scan();
        Spawners = GameObject.FindGameObjectsWithTag("SpawnerEnemy");
        foreach (var spLocal in Spawners)
        {
            SpawnersList.Add(spLocal);
        }
        Players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in Players)
        {
            Priortity.Add(player, 10);
        }
        SetupSpawners();
    }

    /// <summary>
    /// Initilizing the first 5 closest spanwers
    /// </summary>
    void SetupSpawners()
    {
        List<GameObject> Temp = new List<GameObject>();

        foreach (GameObject local in Spawners)
        {
            Temp.Add(local);
        }
        Debug.Log(Temp.Count + "Temp Leght before");
        // Check which spwaners can be activeted
        foreach (GameObject spGameOb in Spawners)
        {
            if (!spGameOb.GetComponent<EnemySpawner>().pleaseSpawnTings)
            {
                foreach (GameObject pGameOb in Players)
                {
                    if (Vector3.Distance(spGameOb.transform.position, pGameOb.transform.position) < minSpawningDistance && Vector3.Distance(spGameOb.transform.position, pGameOb.transform.position) > maxSpawningDistance)
                    {
                        Temp.Remove(spGameOb);
                    }
                }
            }
            else
            {
                Temp.Remove(spGameOb);
            }
        }
        Debug.Log(Temp.Count + "Temp Leght after");

        for (int i = 0; i < amountOfSquadrons; i++)
        {
            if (Temp.Count == 0)
            {
                break;
            }
            Debug.Log("Activating point " + Temp.Count);
            int rValue = Random.Range(0, Temp.Count - 1);
            Temp[rValue].GetComponent<EnemySpawner>().pleaseSpawnTings = true;
            Temp.RemoveAt(rValue);
            activePoints++;
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (!isServer)
        {
            return;
        }

        foreach (GameObject spGameOb in Spawners)
        {
            if (spGameOb.GetComponent<EnemySpawner>().pleaseSpawnTings)
            {
                foreach (GameObject pGameOb in Players)
                {
                    if (Vector3.Distance(spGameOb.transform.position, pGameOb.transform.position) < minSpawningDistance || Vector3.Distance(spGameOb.transform.position, pGameOb.transform.position) > maxSpawningDistance)
                    {
                        DisablePoint(spGameOb);
                    }
                }
            }
        }

        if (amountOfSquadrons > activePoints)
        {
            Debug.Log("Meh... not enough points, we have " + activePoints +" activepoints atm");
            SetupSpawners();
        }
        // Check for priorities
	}

    // Up the priority of a player
    public void UpPriority(GameObject target)
    {
        Priortity[target] = Priortity[target] + 1;
    }

    // Lower priority of a player
    public void DownPriorty(GameObject target)
    {
        Priortity[target] = Priortity[target] - 1;
    }

    // Get a target to attack based on priority system
    public GameObject GetTarget()
    {
        KeyValuePair<GameObject, int> bestTarget = Priortity.First();
        foreach (KeyValuePair<GameObject, int> pTarget in Priortity)
        {
            if (pTarget.Value > bestTarget.Value) bestTarget = pTarget;
        }
        DownPriorty(bestTarget.Key);
        return bestTarget.Key;
    }

    // Get a random player
    public GameObject GetRandomTarget()
    {

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
    public void DisablePoint(GameObject targetSpawner)
    {
        // Set target to be disabled
        targetSpawner.GetComponent<EnemySpawner>().pleaseSpawnTings = false;
        activePoints--;
        // Initlize local vars
        List<GameObject> Temp = new List<GameObject>();
        Temp = SpawnersList;

        // Check which spwaners can be activeted
        foreach (GameObject spGameOb in Spawners)
        {
            if (!spGameOb.GetComponent<EnemySpawner>().pleaseSpawnTings)
            {
                foreach (GameObject pGameOb in Players)
                {
                    if (Vector3.Distance(spGameOb.transform.position, pGameOb.transform.position) < minSpawningDistance && Vector3.Distance(spGameOb.transform.position, pGameOb.transform.position) > maxSpawningDistance)
                    {
                        Temp.Remove(spGameOb);
                    }
                }
            }
            else
            {
                Temp.Remove(spGameOb);
            }
        }

        // Activate a random one
        if (Temp.Count == 0)
        {
            Debug.LogWarning("No avaibles spawners to activate");
        }
        else
        {
            Debug.Log(" ");
            Temp[Random.Range(0, Temp.Count - 1)].GetComponent<EnemySpawner>().pleaseSpawnTings = true;
            activePoints++;
        }
    }
}
