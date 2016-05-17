using UnityEngine;
using System.Collections;

public class MovementPlayer : MonoBehaviour {

    //Editor Varibles
    [SerializeField]
    private bool physicsBased;


    //Public Variables

    // Private Refremces
    private GameObject oracleThing;
    // RigidBody movement
    private Rigidbody2D rBody2D;
    private Vector2 moveVecRB;
    private float moveSpeedRB;
    // Time Based movement
    private Vector2 moveVecTB;
    private float moveSpeedTB;


	// Use this for initialization
	void Awake () {
        rBody2D = GetComponent<Rigidbody2D>();
    }
	
    public void Move(float horizontal, float vertical, float speed)
    {
        if (physicsBased)
        {
            moveVecRB.x = horizontal;
            moveVecRB.y = vertical;
            moveSpeedRB = speed;
        }
        else
        {
            moveVecTB.x = horizontal;
            moveVecTB.y = vertical;
            moveSpeedTB = speed;
        }
    }

    // Update is called once per frame
    // Used for time based movement
    void Update()
    {
        float dt = Time.deltaTime;
        gameObject.transform.Translate(moveVecTB.x * dt * moveSpeedTB, moveVecTB.y * dt * moveSpeedTB, 0f);
    }

    // Used for physics based movenmetn
    void FixedUpdate()
    {
        rBody2D.AddForce(moveVecRB.normalized * moveSpeedRB * rBody2D.mass);
    }
}
