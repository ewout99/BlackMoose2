﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class InputPlayer : NetworkBehaviour {


    //Editors Variables
    [SerializeField]
    private float attackSpeed;
    private float walkSpeed;
    private float normalSpeed;
    
    public bool canShoot;
    public bool oracleAttached;

    //Private Variables
    private Vector3 mousePosition = Vector3.zero;
    private Vector2 aimVec = Vector2.right;
    private Vector2 moveVec = Vector2.zero;
    private float targetAngle;

    private float inverseAttackspeed;
    private float moveSpeed;

    //Editor Refrences
    public GameObject bulletPrefab;

    //Audio Clips
    public AudioClip weaponsFire1;
    public AudioClip weaponsFire2;

    //Private Refrences
    private Camera playerCamera;
    private Rigidbody2D rBody2D;
    private GameObject weaponRef;
    private GameObject OracleRef;


    // Use this for initialization
    void Start () {
        playerCamera = GetComponent<Camera>();
        rBody2D = GetComponent<Rigidbody2D>();
        inverseAttackspeed = 1f / attackSpeed;
	}
	
	// Update is called once per frame
	void Update () {

        // Check for Local Player
        if (!isLocalPlayer)
            return;

        mousePosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        aimVec = mousePosition - transform.position;
        targetAngle = Mathf.Atan2(aimVec.y, aimVec.x) * Mathf.Rad2Deg;
        AdjustWeaponRotation();

        moveVec.x = Input.GetAxis("Horizontal");
        moveVec.y = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Walk") && oracleAttached == false)
        {
            moveSpeed = walkSpeed;
        }
        else
        {
            moveSpeed = normalSpeed;
        }

        if (Input.GetButton("Fire") && canShoot)
        {
            canShoot = false;
            StartCoroutine(ShootDelay());
            CmdShoot();
        }

        if (Input.GetButtonDown("PickUp"))
        {
            CmdAttachOracle();
        }

        if (Input.GetButtonDown("PutDown"))
        {
            CmdDettachOracle();
        }

    }

    void AdjustWeaponRotation()
    {
        weaponRef.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, targetAngle));
    }

    [Command]
    void CmdShoot()
    {
        // Spawn Bullet prefab at the edge fo the gun
    }

    [Command]
    void CmdAttachOracle()
    {

    }

    [Command]
    void CmdDettachOracle()
    {

    }

    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(inverseAttackspeed);
        canShoot = true;
    }

}
