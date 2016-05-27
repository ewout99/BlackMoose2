using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DestructableObject : NetworkBehaviour {

    // Sound Played when hit
    public AudioClip objectHitSound;

    // Refrences to the attached animator
    private Animator aniRef;

    // Bool set so it can only be detroyed once
    private bool objectHit = false;

    // Refrence if for if it's created with a Object Spawner
    private GameObject ParentSpawner;

    // Use this for initialization
    void Start ()
    {
        aniRef = GetComponent<Animator>();
	}

    // Collision
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (!isServer)
            return;
        if (transform.tag != coll.gameObject.tag && !objectHit)
        {
            Hit();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer)
            return;
        if (transform.tag != other.tag && !objectHit)
        {
            Hit();
        }
    }

    // When the object is hit it excutes this
    private void Hit()
    {
        objectHit = true;
        // Trigger Destroy animation
        aniRef.SetTrigger("destroy");
        // Play sound

        // Play Particle Effect
        gameObject.GetComponent<PolygonCollider2D>().enabled = false;

        if (ParentSpawner != null)
        {
            // needs fixing Trhoes errors when the objects are placed.
            StartCoroutine(NetworkDestroy(3f));

        }
    }

    // Pulic function to assign a parent to the object 
    public void Spawned(GameObject parent)
    {
        ParentSpawner = parent;
    }

    // When destroyed it and it has a parentSpawner it makes sure to create a new object
    void OnDestroy()
    {
        if (ParentSpawner == true)
        {
            ParentSpawner.GetComponent<Spawner>().CmdRemoveObject(transform.position);
        }
    }

    // Delayed Network Destroy
    IEnumerator NetworkDestroy(float Wait)
    {
        yield return new WaitForSeconds(Wait);
        CmdDestroyGameObject();
    }

    [Command]
    void CmdDestroyGameObject()
    {
        NetworkServer.Destroy(gameObject);
    }
}
