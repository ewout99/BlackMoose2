using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    // Refrences
    private Animator aniRef;
    private Rigidbody2D rBody2D;

    // Editor variables
    public string origion;

    [SerializeField]
    private float initTime;
    [SerializeField]
    private float autoDestroy;

    public float bulletDmg;
    public float bulletSpeed;

    // Private variables
    private float lauchTime;


    // Use this for initialization
    void Start () {
        lauchTime = Time.time;
        aniRef = GetComponent<Animator>();
        rBody2D = GetComponent<Rigidbody2D>();
        // Play Fireing sound
        // Player firing Animation
        // Play start particle
	}
	
	// Update is called once per frame
	void Update () {
	
        if ((Time.time - lauchTime) < initTime)
        {
            // set trigger to move to next phase flying animation
        }       
	}
    
    // Used to initate the bullet and fire it
    public void Go(Vector2 direction, string og)
    {
        origion = og;
        rBody2D.AddForce(direction.normalized * bulletSpeed);
    }

    // If the origion and the hit target don't match the bullet tries to damage the object
    void OnCollisionEnter2D (Collision2D col)
    {
        if (col.collider.tag != origion && col.collider.GetComponent<Entity>())
        {
            col.collider.GetComponent<Entity>().subtractHealth(bulletDmg);
            Destroy();
        }
        else
        {
            Destroy();
        }
    }

    private void Destroy()
    {
        aniRef.SetTrigger("destroy");
        // Play sound

        // Particle
        StopAllCoroutines();
        // Set alpha to zero and destroy object after sfx finish
        Destroy(gameObject);
    }

    IEnumerator Autodestroy()
    {
        yield return new WaitForSeconds(autoDestroy);
        Destroy(gameObject);
    }
}
