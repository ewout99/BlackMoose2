using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Bullet : NetworkBehaviour {

    // Refrences
    private Animator aniRef;
    private Rigidbody2D rBody2D;
    private Collider2D col2D;

    // Editor variables
    [HideInInspector]
    public string origion;

    [SerializeField]
    private float autoDestroy;

    public float bulletDmg;
    [SerializeField]
    private float bulletSpeed;
    public float attackSpeed;
    [SerializeField]
    public float randomFactor;
    [SerializeField]
    private bool friendlyFire;
    [SerializeField]
    private bool freezOnDeath;

    // Private variables


    // Use this for initialization
    void Awake () {
        aniRef = gameObject.GetComponent<Animator>();
        rBody2D = gameObject.GetComponent<Rigidbody2D>();
        col2D = gameObject.GetComponent<Collider2D>();
        StartCoroutine(NetworkDestroy(autoDestroy));
        // Play Fireing sound
        // Player firing Animation
        // Play start particle
	}
    
    // Used to initate the bullet and fire it
    public void Go(Vector2 direction, string og)
    {
        origion = og;
        direction = direction.normalized;
        direction.x += Random.Range(-randomFactor, randomFactor);
        direction.y += Random.Range(-randomFactor, randomFactor);
        rBody2D.AddForce(direction.normalized * bulletSpeed);
    }

    // If the origion and the hit target don't match the bullet tries to damage the object
    void OnCollisionEnter2D (Collision2D col)
    {
        if (!isServer)
        {
            return;
        }

        if ((col.collider.tag != origion && col.collider.GetComponent<Entity>()) || (friendlyFire && col.collider.GetComponent<Entity>()))
        {
            col.collider.GetComponent<Entity>().CmdSubtractHealth(bulletDmg);
            RpcDestroyThis();
        }
        else if (col.collider.tag == gameObject.tag)
        {
            // Do nothing
        }
        else
        {
            RpcDestroyThis();
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (!isServer)
        {
            return;
        }

        if ((other.tag != origion && other.GetComponent<Entity>()) || (friendlyFire && other.GetComponent<Entity>()))
        {
            other.GetComponent<Entity>().CmdSubtractHealth(bulletDmg);
            RpcDestroyThis();
        }
        else if (other.tag == gameObject.tag)
        {
            // Do nothing
        }
        else
        {
            RpcDestroyThis();
        }
    }

    [ClientRpc]
    private void RpcDestroyThis()
    {
        col2D.enabled = false;
        // Freez the position
        if (freezOnDeath)
        {
            rBody2D.velocity = Vector2.zero;
            rBody2D.isKinematic = true;
            rBody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        aniRef.SetTrigger("end");
        // Play sound

        // Particle

        // Stop Autodestroy to prevent errors
        StopAllCoroutines();
        // Destorys the gameobject after the animation is finished
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
