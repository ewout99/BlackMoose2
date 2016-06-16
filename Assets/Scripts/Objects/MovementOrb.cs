using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MovementOrb : NetworkBehaviour {

    public float fluxValue;
    public float orbSpeed;
    public float lifeTime = 10;

    private GameObject[] Players;
    private List<float> Distances = new List<float>();
    private float smallestDistance;

    // Audiosrouce
    private AudioSource audioRef;
    // AudioClips
    public AudioClip orbSpawn;
    public AudioClip orbPickup;

    void Start()
    {
        Players = GameObject.FindGameObjectsWithTag("Player");
        audioRef = gameObject.GetComponent<AudioSource>();
        audioRef.clip = orbSpawn;
        audioRef.Play();
        StartCoroutine(NetworkDestroy(lifeTime));
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
        {
            return;
        }

        foreach (GameObject player in Players)
        {
            float distance = Vector3.Magnitude(transform.position - player.transform.position);
            if (!player.GetComponent<IngameOracle>())
            {
                Distances.Add(distance);
            }
            else
            {
                Distances.Add(100f);
            }
        }

        Distances.Sort();
        smallestDistance = Distances[0];

        if (smallestDistance <= 2)
        {
            float minVal = Distances.Min();
            int index = Distances.IndexOf(minVal);
            GameObject targetPlayer = Players[index];
            float step = Time.deltaTime * orbSpeed;
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.transform.position, step);
        }
    }


    IEnumerator NetworkDestroy(float Wait)
    {
        yield return new WaitForSeconds(Wait);
        CmdDestroyGameObject();
    }

    [Command]
    public void CmdDestroyGameObject()
    {
        RpcDeath();
        Invoke("CmdDestory2", 0.7f);
    }

    [Command]
    void CmdDestroy2()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ClientRpc]
    void RpcDeath()
    {
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        audioRef.clip = orbPickup;
        audioRef.Play();
    }
}
