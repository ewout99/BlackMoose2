using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class IngameOracle : NetworkBehaviour {

    [SyncVar]
    public string nameIngame;
    public Text nameField;
    [SyncVar]
    public int typeIngame;
    [SyncVar]
    public Color colorIngame;

    [SyncVar]
    public float flux = 0;

    [SerializeField]
    GameObject Oracle_Camera;
    GameObject camRef;

    // Refrences
    private Entity entityRef;
    private Animator aniRef;

    // Use this for initialization
    void Start ()
    {
        nameField = GetComponentInChildren<Text>();
        nameField.text = nameIngame;
        nameField.color = colorIngame;
        entityRef = GetComponent<Entity>();
        aniRef = GetComponent<Animator>();
        if (isLocalPlayer)
        {
            camRef = Instantiate (Oracle_Camera, transform.position, Quaternion.identity) as GameObject;
            camRef.GetComponent<CameraFollow>().target = gameObject.transform;
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (flux != 0)
        {
            entityRef.CmdAddHealth(flux);
            CmdResetFlux();
        }
        aniRef.SetFloat("HealthPercent", entityRef.healthPoints/100);
	}

    //==================
    // Flux adding, reducing and reseting
    [Command]
    public void CmdAddFlux(float amount)
    {
        flux += amount;
    }

    [Command]
    public void CmdSubtractFlux(float amount)
    {
        flux -= amount;
    }

    [Command]
    public void CmdResetFlux()
    {
        flux = 0;
    }
    //======================
}
