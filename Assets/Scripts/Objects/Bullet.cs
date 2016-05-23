using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    // Refrences
    private Animator aniRef;
    private Rigidbody2D rBody2D;

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

    // Private variables


    // Use this for initialization
    void Awake () {
        aniRef = GetComponent<Animator>();
        rBody2D = GetComponent<Rigidbody2D>();
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
        if (col.collider.tag != origion && col.collider.GetComponent<Entity>())
        {
            col.collider.GetComponent<Entity>().CmdSubtractHealth(bulletDmg);
            Destroy();
        }
        else
        {
            Destroy();
        }
    }

    private void Destroy()
    {
        aniRef.SetTrigger("end");
        // Play sound

        // Particle
        StopAllCoroutines();
        // Set alpha to zero and destroy object after sfx finish
        Destroy(gameObject, aniRef.GetCurrentAnimatorStateInfo(0).length);
    }

    IEnumerator Autodestroy()
    {
        yield return new WaitForSeconds(autoDestroy);
        Destroy(gameObject);
    }
}
