using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MovementOrb : NetworkBehaviour {

    public float orbSpeed;
    public float lifeTime = 10;

    private GameObject[] Players;
    private List<float> Distances = new List<float>();
    private float smallestDistance;

    //References
    private Rigidbody2D rBody2D;

    // Audiosrouce
    private AudioSource audioRef;
    // AudioClips
    public AudioClip orbSpawn;
    public AudioClip orbPickup;

    void Start()
    {
        Players = GameObject.FindGameObjectsWithTag("Player");
        rBody2D = GetComponent<Rigidbody2D>();
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

        Distances.Clear();

        rBody2D.velocity = Vector2.zero;
        foreach (GameObject player in Players)
        {
            if (player)
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
        }
        smallestDistance = Distances.Min();
        // Debug.Log ("Smallest distancc " + Distances.Min());
        // Debug.Log(" Indexy Distance" + Distances.IndexOf(smallestDistance));

        if (smallestDistance <= 2)
        {
            int index = Distances.IndexOf(smallestDistance);
            GameObject targetPlayer = Players[index];
            float step = Time.deltaTime * orbSpeed;
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.transform.position, step);
        }
    }

    // Start destory
    [Command]
    public void CmdDestroyGameObject()
    {
        RpcDeath();
        NetworkDestroy(0.7f);
    }

    // Play sound and disbale render and collider
    [ClientRpc]
    void RpcDeath()
    {
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        audioRef.clip = orbPickup;
        audioRef.Play();
    }

    // Wait for while
    IEnumerator NetworkDestroy(float Wait)
    {
        yield return new WaitForSeconds(Wait);
        CmdDestroyGameObject2();
    }

    // Yeea we get to kill it
    [Command]
    private void CmdDestroyGameObject2()
    {
        NetworkServer.Destroy(gameObject);
    }
}
