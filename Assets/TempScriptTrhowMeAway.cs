using UnityEngine;
using System.Collections;
using Pathfinding;

public class TempScriptTrhowMeAway : MonoBehaviour {

    public GameObject shitTargetObject;
    private Vector3 shitTargerVector;
    private Seeker sRef;

	// Use this for initialization
	void Start () {
        sRef = GetComponent<Seeker>();
        shitTargerVector = shitTargetObject.transform.position;
        GoToTarget(shitTargerVector);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void GoToTarget(Vector3 terribleT)
    {
        sRef.StartPath(gameObject.transform.position, terribleT, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
    }
}
