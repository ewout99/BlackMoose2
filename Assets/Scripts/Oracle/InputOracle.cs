using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InputOracle : NetworkBehaviour  {

    // Inputoptions
    [SerializeField]
    private GameObject[] InputOptions;

    // Costs of powers
    [SerializeField]
    private float healCost = 5 ,turretCost = 10, reviveCost = 20, wallCost = 5, weaponCost = 10;

    /// <summary>
    /// The miminum healthpoint that the oracle needs to spawn something
    /// </summary>
    [SerializeField]
    private float minimumFlux;

    // Mouse pos
    private Vector3 mousePosition = Vector3.zero;
    private Camera oracleCamera;

    //Used positions that are not allowed to placed on
    private List<Vector3> UsedPos = new List<Vector3>();
    private string activePower = "turret";


    // References
    private Entity entityRef;
    private Animator aniRef;


    // Use this for initialization
    void Start () {
        if(!isLocalPlayer)
        {
            return;
        }
        // Get refrences
        oracleCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        aniRef = gameObject.GetComponent<Animator>();
        entityRef = gameObject.GetComponent<Entity>();

        // Spawn buttons
    }

    // Update is called once per frame
    void Update () {

        if (!isLocalPlayer)
        {
            return;
        }
        else if (entityRef.deathState)
        {
            return;
        }

        // Listen for input and excute the selected power
        if (Input.GetButtonDown("Fire"))
        {
            mousePosition = oracleCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition = FloorPosition(mousePosition);
            if(entityRef.healthPoints < minimumFlux)
            {
                Debug.Log("To low health to do anyting");
                // Add not engouh flux message
                return;
            }
            switch (activePower)
            {
                case "heal":
                    CmdHealPlacement();
                    break;
                case "turret":
                    CmdTurretPlacement();
                    break;
                case "revive":
                    CmdRevivePlacement();
                    break;
                case "wall":
                    CmdWallPlacement();
                    break;
                case "weapon":
                    CmdWeaponPlacement();
                    break;
                default:
                    Debug.LogError("Oracle Input defaulting");
                    break;
            }
        }
    }

    // Listener for powerselection
    void SelectPower(string input)
    {
        activePower = input;
    }

    // =================
    // Placement function for powers
    [Command]
    void CmdHealPlacement()
    {
        // Check cost
        if (entityRef.healthPoints <= healCost)
        {
            Debug.Log("Not enough health to spawn");
            // Add feedback not enough
            return;
        }
        else
        {
            Debug.Log("Placing Heal");
            aniRef.SetTrigger("attack");
            entityRef.CmdSubtractHealth(healCost);
            GameObject heal = Instantiate(InputOptions[0], mousePosition, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(heal);
        }
    }

    [Command]
    void CmdTurretPlacement()
    {
        // Check cost
        if (entityRef.healthPoints <= turretCost)
        {
            Debug.Log("Not enough health to spawn");
            // Add feedback not enough
            return;
        }
        else if (AddPosToUsed(mousePosition))
        {
            Debug.Log("Making the turrert");
            aniRef.SetTrigger("Attack");
            entityRef.CmdSubtractHealth(turretCost);
            GameObject turret = Instantiate(InputOptions[1], mousePosition, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(turret);
            // SpawnTurrert
        }
    }

    [Command]
    void CmdRevivePlacement()
    {
        // Check cost
        if (entityRef.healthPoints <= reviveCost)
        {
            // Add feedback not enough
            return;
        }
        else
        {
            aniRef.SetTrigger("Attack");
            entityRef.CmdSubtractHealth(reviveCost);
            GameObject revive = Instantiate(InputOptions[2], mousePosition, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(revive);
        }
    }
    [Command]
    void CmdWallPlacement()
    {
        // Check cost
        if (entityRef.healthPoints <= wallCost)
        {
            Debug.Log("Not enough healt");
            // Add feedback not enough
            return;
        }

        if (AddPosToUsed(mousePosition))
        {
            aniRef.SetTrigger("Attack");
            entityRef.CmdSubtractHealth(wallCost);
            GameObject wall = Instantiate(InputOptions[3], mousePosition, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(wall);
        }

        // Rescan pathfinder
        GameObject.Find("PathFinder").GetComponent<AstarPath>().Scan();
    }
    [Command]
    void CmdWeaponPlacement()
    {
        // Check cost
        if (entityRef.healthPoints <= weaponCost)
        {
            // Add feedback not enough
            return;
        }
        if (AddPosToUsed(mousePosition))
        {
            aniRef.SetTrigger("Attack");
            entityRef.CmdSubtractHealth(weaponCost);
            GameObject weaponRack = Instantiate(InputOptions[4], mousePosition, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(weaponRack);
        }
    }

    // ====================

    // Placement rounding
    Vector3 FloorPosition(Vector3 input)
    {
        Vector3 outPut = new Vector3();
        outPut.x = Mathf.Round(input.x);
        outPut.y = Mathf.Round(input.y);
        outPut.z = 0;
        return outPut;
    }

    // Check placement
    private bool AddPosToUsed(Vector3 requestedPos)
    {
        if(UsedPos.Contains(requestedPos))
        {
            Debug.Log("Position in use");
            return false;
        }
        else
        {
            UsedPos.Add(requestedPos);
            return true;
        }
    }

    public void RemovePosFromUsed(Vector3 objectPos)
    {
        UsedPos.Remove(objectPos);
    }
}
