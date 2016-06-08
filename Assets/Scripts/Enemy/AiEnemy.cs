using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Pathfinding;

public class AiEnemy : NetworkBehaviour {

    [SerializeField]
    private float moveDistance;

    [SerializeField]
    private float attackDistance;

    // Targets to move to
    public GameObject targetObject = null;
    public Transform targetTransform = null;
    public Vector3 targetVector = Vector3.zero;

    // Private refs
    private Entity entiyRef;
    private Animator aniRef;
    private GameObject PathfinderRef;
    private Seeker sRef;
    private SpriteRenderer spRef;

    private bool gettingPath;

    // State Enum
    enum States
    {
        Walking,
        Attacking,
        Idle,
        Death
    }

    private States currentState = States.Idle;

    //The calculated path
    public Path path;
    //The AI's speed per second
    public float speed = 100;
    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3;
    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;

    [SyncVar(hook = "FlipXHook")]
    private bool CurrentFlipX;


    // Use this for initialization
    void Start()
    {
        aniRef = GetComponent<Animator>();
        sRef = GetComponent<Seeker>();
        spRef = GetComponent<SpriteRenderer>();
        entiyRef = GetComponent<Entity>();
        PathfinderRef = GameObject.Find("PathFinder");
        PathfinderRef.GetComponent<AstarPath>().Scan();
        targetObject = PathfinderRef.GetComponent<AiController>().GetTarget();

    }

    // Update is called once per frame
    void Update()
    {
        // Server only check
        if (!isServer)
        {
            return;
        }     
        
        if (moveDistance > Vector3.SqrMagnitude(targetObject.transform.position - transform.position ))
        {
            currentState = States.Walking;
        }

        if (attackDistance > Vector3.SqrMagnitude(targetObject.transform.position - transform.position))
        {
            currentState = States.Attacking;
        }

        if (entiyRef.deathState)
        {
            currentState = States.Death;
        }

            switch (currentState)
        {
            case States.Walking:
                Move();
                break;
            case States.Attacking:
                Attack();
                break;
            case States.Idle:
                Idling();
                break;
            case States.Death:
                ImDead();
                break;
            default:
                Default();
                break;
        }
    }

    // Move functionality
    private void Move()
    {
        if (path == null && !gettingPath)
        {
            Debug.Log("No Path");
            if (targetObject != null)
            {
                GoToTarget(targetObject);
                return;
            }
            else if (targetTransform != null)
            {
                GoToTarget(targetTransform);
                return;
            }
            else if (targetVector != Vector3.zero)
            {
                GoToTarget(targetVector);
                return;
            }

            else
            {
                Debug.Log("Getting new target");
                targetObject = PathfinderRef.GetComponent<AiController>().GetTarget();
            }
            //We have no path and no target to move
            return;
        }
        // If we are callulating a path pause for a frame
        if (gettingPath)
        {
            return;
        }

        // If we are at the final way point ask the Ai Manager for priority
        if (currentWaypoint >= path.vectorPath.Count)
        {
            Debug.Log("End Of Path Reached");
            // Ask for new path 
            ResetTarget();
            ResetPath();
            targetObject = PathfinderRef.GetComponent<AiController>().GetTarget();
            return;
        }

        //Direction to the next waypoint
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;

        // Call movment fuction on the enemy movement class


        // Remove this after adding movement class
        dir *= speed * Time.deltaTime;
        transform.Translate(dir);
        aniRef.SetBool("walking", true);
        if (dir.x < 0)
        {
            spRef.flipX = true;
            CmdFlipX(true);
        }
        else if (dir.x > 0)
        {
            spRef.flipX = false;
            CmdFlipX(false);
        }

        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }

    /// <summary>
    /// Give a Target vector/transfrom/object to go to.
    /// </summary> 
    public void GoToTarget(Vector3 tVector)
    {
        gettingPath = true;
        sRef.StartPath(gameObject.transform.position, tVector, OnPathComplete);
    }

    public void GoToTarget(Transform tTransform)
    {
        gettingPath = true;
        sRef.StartPath(gameObject.transform.position, tTransform.position, OnPathComplete);
    }

    public void GoToTarget(GameObject tObject)
    {
        gettingPath = true;
        sRef.StartPath(gameObject.transform.position, tObject.transform.position, OnPathComplete);
    }

    /// <summary>
    /// Reset all the targets for the Ai
    /// </summary>
    public void ResetTarget()
    {
        targetObject = null;
        targetTransform = null;
        targetVector = Vector3.zero;
    }

    /// <summary>
    /// Reset the current path to null
    /// </summary>
    public void ResetPath()
    {
        path = null;
    }

    // Path Completion
    private void OnPathComplete(Path p)
    {
        gettingPath = false;
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        if (!p.error)
        {
            // New path is path
            path = p;
            //Reset the waypoint counter
            currentWaypoint = 0;
        }
    }

    // Attack functions
    private void Attack()
    {
        aniRef.SetTrigger("attack");
    }

    private void Idling()
    {
        aniRef.SetBool("walking", false);
    }

    // Ai is defeated
    private void ImDead()
    {
        aniRef.SetTrigger("die");
    }

    private void Default()
    {
        Debug.Log("I'm defaulting something went wrong");
    }


    [Command]
    void CmdFlipX(bool setTo)
    {
        gameObject.GetComponent<SpriteRenderer>().flipX = setTo;
        CurrentFlipX = setTo;
    }

    // Hook for SyncVar
    private void FlipXHook(bool setTo)
    {
        gameObject.GetComponent<SpriteRenderer>().flipX = setTo;
        CurrentFlipX = setTo;
    }
}