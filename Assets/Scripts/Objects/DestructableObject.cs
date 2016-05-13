using UnityEngine;
using System.Collections;

public class DestructableObject : MonoBehaviour {

    public Animator AniRef;

    // Use this for initialization
    void Start () {
        AniRef = GetComponent<Animator>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D Other)
    {
        if (Other.tag != gameObject.tag)
        {
            // Trigger Destroy animation
        }
    }
}
