using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Spawner :  NetworkBehaviour{

    /// <summary>
    /// Array that holds the possible objects that could be spawned
    /// </summary>
    [SerializeField]
    private GameObject[] Objects;

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
    private int maxObjectAmount;
    private int currentObjectAmount;

    // 2 Arrays of Availible and non Availible locations
    private List<Vector2> Availible = new List<Vector2>();
    private List<Vector2> NotAvailible = new List<Vector2>();

    // Use this for initialization
    void Start ()
    {
        if (!isServer)
        {
            return;
        }
        // Creates the array of possible positions
        maxObjectAmount = Mathf.Clamp(maxObjectAmount, 0, SizeX * SizeY);
        for (int x = -SizeX; x < SizeX; x++)
        {
            for (int y = -SizeY; y < SizeY; y++)
            {
                Availible.Add(new Vector2(x, y));
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!isServer)
        {
            return;
        }

        // Gets a random Position and Object to be spawned
        while (currentObjectAmount < maxObjectAmount)
        {
            GameObject randomObject = Objects[Random.Range(0, Objects.Length)];
            Vector2 randomPosition = Availible[Random.Range(0, Availible.Count)];
            Availible.Remove(randomPosition);
            NotAvailible.Add(randomPosition);
            currentObjectAmount++;
            // Debug.Log(randomObject +" "+ randomPosition);
            CmdPlaceObject(randomObject, randomPosition);
        }
	}

    // Add New object to the network
    [Command]
    public void CmdPlaceObject(GameObject objectType, Vector2 objectPosition)
    {
        GameObject chosenObject = (GameObject)Instantiate(objectType, transform.position +(Vector3) objectPosition, Quaternion.identity);
        chosenObject.transform.parent = transform;
        chosenObject.GetComponent<DestructableObject>().Spawned(gameObject);
        NetworkServer.Spawn(chosenObject);
    }

    // Remove a object from the network
    [Command]
    public void CmdRemoveObject(Vector2 objectPosition)
    {
        objectPosition = ((Vector2)transform.position - objectPosition);
        NotAvailible.Remove(objectPosition);
        Availible.Add(objectPosition);
        currentObjectAmount--;
    }
}
