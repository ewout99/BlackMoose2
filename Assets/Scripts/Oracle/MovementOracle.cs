using UnityEngine;
using System.Collections;

public class MovementOracle : MonoBehaviour {

    private Rigidbody2D rBody2D;

	// Use this for initialization
	void Start () {

        rBody2D = GetComponent<Rigidbody2D>();
        rBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        rBody2D.velocity = Vector2.zero;

    }
	
	// Update is called once per frame
	void Update () {
        rBody2D.velocity = Vector2.zero;

    }
}
