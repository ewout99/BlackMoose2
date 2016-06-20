using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class InputOracle : NetworkBehaviour  {

    // Buttons
    [SerializeField]
    private bool debugGUI = true;
    public Texture btnTexture;
    public Vector2 buttonSize = new Vector2(10f,10f);
    public Vector2 buttonStartPos = new Vector2(10f,10f);
    public float buttonSpacing = 100f;
    private float ogStartPos;

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
    private string activePower = "Player";


    // References
    private Entity entityRef;
    private Animator aniRef;
    private AudioSource aSRef;

    public AudioClip inputSound;


    // Use this for initialization
    void Start () {
        // Get refrences
        aniRef = gameObject.GetComponent<Animator>();
        entityRef = gameObject.GetComponent<Entity>();

        if (isLocalPlayer)
        {
            oracleCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        // Check screen values for buttons

        ogStartPos = buttonStartPos.x;

        buttonStartPos = new Vector2(buttonStartPos.x, Screen.height - buttonStartPos.y);
    }

    void OnGUI()
    {
        buttonStartPos.x = ogStartPos;
        if (!isLocalPlayer || !debugGUI)
        {
            return;
        }
        
        if (!btnTexture)
        {
            Debug.LogError("Please assign a texture on the inspector");
            return;
        }

        foreach (GameObject input in InputOptions)
        {
            if (GUI.Button(new Rect(buttonStartPos, buttonSize), input.tag))
            {
                SelectPower(input.tag);
                Debug.Log(input.tag);
            }
            buttonStartPos.x += buttonSpacing;
        }
    }
    // Update is called once per frame
    void Update () {

        if (!isLocalPlayer)
        {
            return;
        }
        if (!entityRef)
        {
            entityRef = gameObject.GetComponent<Entity>();
        }
        else if (entityRef.deathState)
        {
            return;
        }

        // Listen for input and excute the selected power
        if (Input.GetButtonDown("Fire"))
        {
            if (!(GUIUtility.hotControl == 0))
            {
                // Debug UI input
                return;
            }
            if(EventSystem.current.IsPointerOverGameObject())
            {
                // Canvas UI input
                return;
            }
            
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
                case "Health":
                    HealLocalCheck();
                    break;
                case "Player":
                    TurrertLocalCheck();
                    break;
                case "Revive":
                    ReviveLocalCheck();
                    break;
                case "Wall":
                    WallLocalCheck();
                    break;
                case "Weapon":
                    WeaponLocalCheck();
                    break;
                default:
                    Debug.LogError("Oracle Input defaulting");
                    break;
            }
        }
    }

    // Listener for powerselection
    public void SelectPower(string input)
    {
        activePower = input;
    }

    // =================
    // Placement function for powers

    void HealLocalCheck()
    {
        if (entityRef.healthPoints <= healCost)
        {
            Debug.Log("Not enough health to spawn");
            // Add feedback not enough
            return;
        }
        aniRef.SetTrigger("Attack");
        CmdHealPlacement(mousePosition);
        aSRef.clip = inputSound;
        aSRef.Play();
    }

    [Command]
    void CmdHealPlacement(Vector3 targetPos)
    {   
            Debug.Log("Placing Heal");
            entityRef.CmdSubtractHealth(healCost);
            GameObject heal = Instantiate(InputOptions[0], targetPos, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(heal);
    }

    void TurrertLocalCheck()
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
            aniRef.SetTrigger("Attack");
            CmdTurretPlacement(mousePosition);
        }
        aSRef.clip = inputSound;
        aSRef.Play();
    }

    [Command]
    void CmdTurretPlacement(Vector3 targetPos)
    {
            Debug.Log("Placing turret");
            entityRef.CmdSubtractHealth(turretCost);
            GameObject turret = Instantiate(InputOptions[1], targetPos, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(turret);
    }

    void ReviveLocalCheck()
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
            CmdRevivePlacement(mousePosition);
        }
        aSRef.clip = inputSound;
        aSRef.Play();
    }

    [Command]
    void CmdRevivePlacement(Vector3 targetPos)
    {
        Debug.Log("Placing revive");
        entityRef.CmdSubtractHealth(reviveCost);
        GameObject revive = Instantiate(InputOptions[2], targetPos, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(revive);
    }

    void WallLocalCheck()
    {
        // Check cost
        if (entityRef.healthPoints <= wallCost)
        {
            Debug.Log("Not enough health");
            // Add feedback not enough
            return;
        }
        else if (AddPosToUsed(mousePosition))
        {
            aniRef.SetTrigger("Attack");
            CmdWallPlacement(mousePosition);
        }
        aSRef.clip = inputSound;
        aSRef.Play();
    }

    [Command]
    void CmdWallPlacement(Vector3 targetPos)
    {
        Debug.Log("Placing wall");
        entityRef.CmdSubtractHealth(wallCost);
        GameObject wall = Instantiate(InputOptions[3], targetPos, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(wall);
        GameObject.Find("PathFinder").GetComponent<AstarPath>().Scan();
    }

    void WeaponLocalCheck()
    {
        // Check cost
        if (entityRef.healthPoints <= weaponCost)
        {
            // Add feedback not enough
            return;
        }
        else if (AddPosToUsed(mousePosition))
        {
            aniRef.SetTrigger("Attack");
            CmdWeaponPlacement(mousePosition);
        }
        aSRef.clip = inputSound;
        aSRef.Play();
    }

    [Command]
    void CmdWeaponPlacement(Vector3 targetPos)
    {
        Debug.Log("Placing weapon");
        entityRef.CmdSubtractHealth(weaponCost);
        GameObject weaponRack = Instantiate(InputOptions[4], targetPos, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(weaponRack);
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
