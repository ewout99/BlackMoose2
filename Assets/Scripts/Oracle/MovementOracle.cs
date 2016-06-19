using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MovementOracle : NetworkBehaviour {


    [SyncVar]
    public bool beingCarried = false;

    public NetworkInstanceId target;
    private GameObject targetObject;
    private Transform targetTransform;
    [SerializeField]
    private float carryOffSet = 1.5f;
    private Vector3 targetVector;

    private Animator aniRef;
    private Rigidbody2D rBody2D;


    // Use this for initialization
    void Start () {

        rBody2D = GetComponent<Rigidbody2D>();
        aniRef = GetComponent<Animator>();
        rBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        rBody2D.velocity = Vector2.zero;
    }
	
	// Update is called once per frame
	void Update () {
        rBody2D.velocity = Vector2.zero;

        if (!isLocalPlayer)
        {
            return;
        }

        if(!beingCarried)
        {
            aniRef.SetBool("Carried", false);
            rBody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            Debug.Log("target opbject is " + targetObject.name);
            aniRef.SetBool("Carried", true);
            rBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            targetVector.y = targetObject.transform.position.y + carryOffSet;
            targetVector.x = targetObject.transform.position.x;
            targetVector.z = targetObject.transform.position.z;
            transform.position = targetVector;
        }
    }

    [Command]
    public void CmdSetTargetObject(NetworkInstanceId inputID)
    {
        beingCarried = true;
        GetComponent<Collider2D>().enabled = false;
        target = inputID;
        targetObject = NetworkServer.FindLocalObject(target);
        RpcSetTargetObject(inputID);
    }

    [ClientRpc]
    private void RpcSetTargetObject(NetworkInstanceId inputID)
    {
        GetComponent<Collider2D>().enabled = false;
        beingCarried = true;
        target = inputID;
        targetObject = ClientScene.FindLocalObject(target);
    }

    [Command]
    public void CmdResetTarget()
    {
        beingCarried = false;
        GetComponent<Collider2D>().enabled = true;
        target = netId;
        targetVector = Vector3.down;
        targetVector.y -= carryOffSet;
        transform.Translate(targetVector);
    }
}
