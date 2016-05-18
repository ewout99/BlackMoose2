using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class InputPlayer : NetworkBehaviour {


    //Editors Variables
    [SerializeField]
    private float attackSpeed;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float normalSpeed;
    
    public bool canShoot;
    public bool oracleAttached;

    //Private Variables
    private Vector3 mousePosition = Vector3.zero;
    private Vector2 aimVec = Vector2.right;
    private Vector2 moveVecInput;
    private float targetAngle;

    private float inverseAttackspeed;
    private float moveSpeedInput;

    //Editor Refrences
    public GameObject[] bulletPrefabs;

    //Audio Clips
    public AudioClip weaponsFire1;
    public AudioClip weaponsFire2;

    //Private Refrences
    private Camera playerCamera;
    private GameObject weaponRef;
    private Transform bulletSpawnRef;
    private GameObject OracleRef;
    private MovementPlayer moveRef;
    private Animator aniRef;
    private SpriteRenderer spRef;


    // Use this for initialization
    void Start () {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        moveRef = GetComponent<MovementPlayer>();
        aniRef = GetComponent<Animator>();
        spRef = GetComponent<SpriteRenderer>();
        weaponRef = transform.FindChild("Temp Weapon").gameObject;
        bulletSpawnRef = transform.FindChild("Temp Weapon").gameObject.transform.FindChild("Temp Spawn");
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

        moveVecInput.x = Input.GetAxis("Horizontal");
        moveVecInput.y = Input.GetAxis("Vertical");

        if (Input.GetButton("Walk") || oracleAttached)
        {
            moveSpeedInput = walkSpeed;
        }
        else
        {
            moveSpeedInput = normalSpeed;
        }

        // Call the Movement class
        moveRef.Move(moveVecInput.x, moveVecInput.y, moveSpeedInput);

        // Check which animation needs to be played
        if ( moveVecInput.x > 0 || moveVecInput.x < 0 || moveVecInput.y > 0 || moveVecInput.y < 0)
        {
            if ( moveSpeedInput == walkSpeed)
            {
                aniRef.SetBool("walking", true);
                aniRef.SetBool("running", false);
            }
            else
            {
                aniRef.SetBool("walking", false);
                aniRef.SetBool("running", true);
            }
            if (moveVecInput.x < 0)
            {
                spRef.flipX = true;
            }
            else if (moveVecInput.x > 0)
            {
                spRef.flipX = false;
            }
        }
        else
        {
            aniRef.SetBool("walking", false);
            aniRef.SetBool("running", false);
        }

        // Firing bullet
        if (Input.GetButton("Fire") && canShoot)
        {
            canShoot = false;
            StartCoroutine(ShootDelay());
            CmdShoot(aimVec);
        }

        // Pick up and drop the oracle
        if (Input.GetButtonDown("PickUp"))
        {
            CmdAttachOracle();
        }

        if (Input.GetButtonDown("PutDown"))
        {
            CmdDettachOracle();
        }

        RpcAniUpdate();
    }

    void AdjustWeaponRotation()
    {
        // Add weapons to player then enable
        weaponRef.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, targetAngle));
    }

    [Command]
    void CmdShoot(Vector2 direction)
    {
        // Spawn Bullet prefab at the edge fo the gun
        GameObject bullet = (GameObject)Instantiate(bulletPrefabs[0], bulletSpawnRef.position, bulletSpawnRef.rotation);
        bullet.GetComponent<Bullet>().Go(direction, transform.tag);
        NetworkServer.Spawn(bullet);
    }

    [Command]
    void CmdAttachOracle()
    {

    }

    [Command]
    void CmdDettachOracle()
    {

    }

    void RpcAniUpdate()
    {
        aniRef.SetBool("walking", aniRef.GetBool("walking"));
        aniRef.SetBool("running", aniRef.GetBool("running"));
        spRef.flipX = spRef.flipX;
    }

    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(inverseAttackspeed);
        canShoot = true;
    }

}
