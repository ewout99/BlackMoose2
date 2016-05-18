using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    // Refrences
    private Animator aniRef;


    // Editor variables
    public string origion;

    [SerializeField]
    private float initTime;
    [SerializeField]
    private float autoDestroy;

    public float bulletDmg;

    // Private variables
    private float lauchTime;

    // Use this for initialization
    void Start () {
        lauchTime = Time.time;
        aniRef = GetComponent<Animator>();
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

    void OnCollisionEnter2D (Collider2D other)
    {
        if (other.tag != origion && other.GetComponent<Entity>())
        {
            other.GetComponent<Entity>().subtractHealth(bulletDmg);
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
