using UnityEngine;
using System.Collections;

public class MovementEnemy : MonoBehaviour {

    private Rigidbody2D rBody2D;
	// Use this for initialization
	void Start ()
    {
        rBody2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        rBody2D.velocity = Vector2.zero;
	}
}
