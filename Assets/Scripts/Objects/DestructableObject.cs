using UnityEngine;
using System.Collections;

public class DestructableObject : MonoBehaviour {

    public AudioClip objectHitSound;

    private Animator aniRef;

    private bool objectHit = false;

    // Use this for initialization
    void Start ()
    {
        aniRef = GetComponent<Animator>();
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        if (transform.tag != gameObject.tag && !objectHit)
        {
            Hit();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag != gameObject.tag && !objectHit)
        {
            Hit();
        }
    }

    private void Hit()
    {
        objectHit = true;
        // Trigger Destroy animation
        aniRef.SetTrigger("destroy");
        // Play sound

        // Play Particle Effect
        gameObject.GetComponent<PolygonCollider2D>().enabled = false;
    }
}
