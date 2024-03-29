﻿using UnityEngine;
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

    private float minSpawningDistance = 10;
    private float maxSpawningDistance = 30;
    public int amountOfSquadrons = 3;
    private int activePoints;

    private bool levelDelayComplete;

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
        StartCoroutine(levelDelayStart());
        StartCoroutine(UpdatePriority());
    }

    /// <summary>
    /// Initilizing the first 5 closest spanwers
    /// </summary>
    void SetupSpawners()
    {
        List<GameObject> Temp = new List<GameObject>();
        Temp.Clear();
        foreach (GameObject local in Spawners)
        {
            Temp.Add(local);
        }
        // Check which spwaners can be activeted
        foreach (GameObject spGameOb in Spawners)
        {
            if (!spGameOb.GetComponent<EnemySpawner>().pleaseSpawnTings)
            {
                foreach (GameObject pGameOb in Players)
                {
                    if (!pGameOb || !spGameOb)
                    {
                        // Catch for fails
                    }
                    else if (Vector3.Distance(spGameOb.transform.position, pGameOb.transform.position) < minSpawningDistance && Vector3.Distance(spGameOb.transform.position, pGameOb.transform.position) > maxSpawningDistance)
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

        for (int i = 0; i < amountOfSquadrons; i++)
        {
            if (Temp.Count == 0)
            {
                break;
            }
            int rValue = Random.Range(0, Temp.Count - 1);
            Temp[rValue].GetComponent<EnemySpawner>().pleaseSpawnTings = true;
            Temp.RemoveAt(rValue);
            activePoints++;
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (!isServer || !levelDelayComplete)
        {
            return;
        }

        foreach (GameObject spGameOb in Spawners)
        {
            if (spGameOb.GetComponent<EnemySpawner>().pleaseSpawnTings)
            {
                foreach (GameObject pGameOb in Players)
                {
                    if (!pGameOb || !spGameOb)
                    {
                        // Catch for fails
                    }
                    else if (Vector3.Distance(spGameOb.transform.position, pGameOb.transform.position) < minSpawningDistance || Vector3.Distance(spGameOb.transform.position, pGameOb.transform.position) > maxSpawningDistance)
                    {
                        DisablePoint(spGameOb);
                    }
                }
            }
        }

        if (amountOfSquadrons > activePoints)
        {
            SetupSpawners();
        }
        // Check for priorities
	}

    // Up the priority of a player
    public void UpPriority(GameObject target)
    {
        if (Priortity.ContainsKey(target))
        {
            Priortity[target] = Priortity[target] + 1;
        }
    }

    // Lower priority of a player
    public void DownPriorty(GameObject target)
    {
        if (Priortity.ContainsKey(target))
        {
            Priortity[target] = Priortity[target] - 1;
        }
    }

    // Get a target to attack based on priority system
    public GameObject GetTarget()
    {
        KeyValuePair<GameObject, int> bestTarget = Priortity.First();
        foreach (KeyValuePair<GameObject, int> pTarget in Priortity)
        {
            if (!pTarget.Key)
            {
                // This needs fixing gives errors :@
            }
            else if (!pTarget.Key.GetComponent<Entity>().deathState)
            {
                if (pTarget.Value > bestTarget.Value) bestTarget = pTarget;
            }
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
                    if (!pGameOb || !spGameOb)
                    {
                        // Catch for fails
                    }
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
        }
        else
        {
            Temp[Random.Range(0, Temp.Count - 1)].GetComponent<EnemySpawner>().pleaseSpawnTings = true;
            activePoints++;
        }
    }

    IEnumerator UpdatePriority()
    {
        yield return new WaitForSeconds(5f);
        Priortity.Clear();
        Players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in Players)
        {
            if (player.GetComponent<Entity>().deathState)
            {
                Priortity.Add(player, 0);
            }
            else if (player.GetComponent<IngameOracle>())
            {
                Priortity.Add(player, 12);
            }
            else if (player.GetComponent<IngamePlayer>())
            {
                Priortity.Add(player, 10);
            }
            else if(player.GetComponent<Turret>())
            {
                Priortity.Add(player, 9);
            }
            else
            {
                Priortity.Add(player, 8);
            }
        }
        StartCoroutine(UpdatePriority());
    }

    IEnumerator levelDelayStart()
    {
        yield return new WaitForSeconds(3f);
        levelDelayComplete = true;
    }
}