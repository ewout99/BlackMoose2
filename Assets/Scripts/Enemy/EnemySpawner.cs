using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : NetworkBehaviour
{

    /// <summary>
    /// Array that holds the possible objects that could be spawned
    /// </summary>
    [SerializeField]
    private GameObject[] Enemies;

    /// <summary>
    /// X Size of the grid relative to the spaners position
    /// </summary>
    [SerializeField]
    private int SizeX;

    /// <summary>
    /// Y Size of the grid relative to the spawners position
    /// </summary>
    [SerializeField]
    private int SizeY;

    [SerializeField]
    private int maxEnemyAmount;
    private int currentEnemyAmount;

    // 2 Arrays of Availible and non Availible locations
    private List<Vector2> Availible = new List<Vector2>();
    private List<Vector2> NotAvailible = new List<Vector2>();

    public bool pleaseSpawnTings;

    // Use this for initialization
    void Start()
    {
        if (!isServer)
        {
            return;
        }

        // Creates the array of possible positions
        maxEnemyAmount = Mathf.Clamp(maxEnemyAmount, 0, SizeX * SizeY);
        for (int x = -SizeX; x < SizeX; x++)
        {
            for (int y = -SizeY; y < SizeY; y++)
            {
                Availible.Add(new Vector2(x, y));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer || !pleaseSpawnTings)
        {
            return;
        }

        if (currentEnemyAmount == 0)
        {
            // Gets a random Position and Enemt to be spawned
            while (currentEnemyAmount < maxEnemyAmount)
            {
                GameObject randomEnemy = Enemies[Random.Range(0, Enemies.Length)];
                Vector2 randomPosition = Availible[Random.Range(0, Availible.Count - 1)];
                Availible.Remove(randomPosition);
                NotAvailible.Add(randomPosition);
                currentEnemyAmount++;
                CmdPlaceObject(randomEnemy, randomPosition);
                StartCoroutine(MakeAvaible(randomPosition));
            }
        }
    }

    // Add New Enmey to the network
    [Command]
    public void CmdPlaceObject(GameObject objectType, Vector2 objectPosition)
    {
        GameObject chosenObject = (GameObject)Instantiate(objectType, transform.position + (Vector3)objectPosition, Quaternion.identity);
        chosenObject.transform.parent = transform;
        chosenObject.GetComponent<AiEnemy>().Spawned(gameObject);
        NetworkServer.Spawn(chosenObject);
    }

    // Remove a Enemy from the list
    public void RemoveObject()
    {
        currentEnemyAmount--;
    }

    // Make A spot avaible after 5 second for enmies to spwan on
    IEnumerator MakeAvaible(Vector2 objectPosition)
    {
        yield return new WaitForSeconds(5f);
        NotAvailible.Remove(objectPosition);
        Availible.Add(objectPosition);
    }
}