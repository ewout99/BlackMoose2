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


    [SerializeField]
    private float rampUpTime;

    [SerializeField]
    private float slowDownTime;

    // Speed && direction in the previous frame
    private float lastFrameSpeed;
    private Vector2 lastDirection;
    // Speed to calculate
    private float calSpeed;
    // Float for calculating smoothdamping 
    private float velocityCalSpeed;

	// Use this for initialization
	void Awake () {
        rBody2D = gameObject.GetComponent<Rigidbody2D>();
    }
	
    /// <summary>
    /// Moves the charcter either based on physiscs or on a translate
    /// </summary>
    /// <param name="horizontal">The horizontal input</param>
    /// <param name="vertical">The vertical input </param>
    /// <param name="speed">The current given movespeed</param>
    public void Move(float horizontal, float vertical, float speed)
    {
        rBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        // Physics based movment if enabled
        if (physicsBased)
        {
            moveVecRB.x = horizontal;
            moveVecRB.y = vertical;
            moveSpeedRB = speed;
        }
        // Time based movment
        else
        {
            if (horizontal != 0 || vertical != 0 && lastFrameSpeed != speed)
            {
                calSpeed = Mathf.SmoothDamp(calSpeed, speed, ref velocityCalSpeed, rampUpTime);
                moveVecTB.x = horizontal;
                moveVecTB.y = vertical;
                lastDirection.x = horizontal;
                lastDirection.y = vertical;
            }
            else if(horizontal == 0 || vertical == 0 && lastFrameSpeed > 0)
            {
                calSpeed = Mathf.SmoothDamp(calSpeed, 0, ref velocityCalSpeed , slowDownTime);
                moveVecTB = lastDirection;
            }
            calSpeed = Mathf.Clamp(calSpeed, 0, speed);
            moveVecTB = moveVecTB.normalized;
            moveSpeedTB = calSpeed;
            lastFrameSpeed = calSpeed;
        }
    }

    public void Recoil(Vector2 shootDirection)
    {
        Vector2 recoilDirection = -shootDirection;
        recoilDirection = recoilDirection.normalized;
        recoilDirection.x *= 0.1f;
        recoilDirection.y *= 0.1f;
        rBody2D.MovePosition(new Vector2(transform.position.x, transform.position.y) + recoilDirection);
    }

    /*  Update is called once per frame
        Used for time based movement
    */
    void Update()
    {
        float dt = Time.deltaTime;
        gameObject.transform.Translate(moveVecTB.x * dt * moveSpeedTB, moveVecTB.y * dt * moveSpeedTB, 0f);
        moveVecTB = Vector2.zero;
    }

    // Used for physics based movement
    void FixedUpdate()
    {
        rBody2D.AddForce(moveVecRB.normalized * moveSpeedRB * rBody2D.mass,ForceMode2D.Force);
        moveVecRB = Vector2.zero;
    }
}
