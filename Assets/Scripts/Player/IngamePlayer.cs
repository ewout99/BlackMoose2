using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class IngamePlayer : NetworkBehaviour {

    [SyncVar]
    public string nameIngame;
    public Text nameField;
    [SyncVar]
    public int typeIngame;
    [SyncVar]
    public Color colorIngame;

    [SyncVar]
    public float flux = 0f;

    // Editor refrences
    public GameObject Player_Camera;
    public GameObject Oracle_Ref;

    // Editor Variaels
    public float fluxDeliveryRange = 4f;

    // Private refrences
    private PickUp pickRef;
    private Entity entiRef;
    private GameObject oracleRef;

    [SerializeField]
    private string[] AnimCons;

    [SerializeField]
    private Sprite[] WeaponSprites;


    // Use this for initialization
    void Start ()
    {
        nameField.text = nameIngame;
        nameField.color = colorIngame;
        pickRef = GetComponent<PickUp>();
        GetComponent<Animator>().runtimeAnimatorController = ((RuntimeAnimatorController)(Resources.Load(AnimCons[typeIngame])));

        if (isLocalPlayer && typeIngame == 4)
        {
            CmdReplaceMe();
        }
        else if (isLocalPlayer)
        {
            Instantiate(Player_Camera, transform.position, Quaternion.identity);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().target = gameObject.transform;
        }
    }

    // Update is called once per frame
    void Update () {
        // Check if its the local player
        if (!isLocalPlayer)
            return;


        if (!oracleRef)
        {
            oracleRef = GameObject.Find("Temp Oracle(Clone)");
        }

       
        // If the oracle is near by send your flux to the oracle
        if (flux > 0 && Vector3.Distance(oracleRef.transform.position , transform.position) <= fluxDeliveryRange)
        {
            Debug.Log(Vector3.Distance(oracleRef.transform.position, transform.position));
            // Call oracle ref send flux to it

            oracleRef.GetComponent<IngameOracle>().CmdAddFlux(flux);
            // Play flux delivery animation here
            // Play flux delivery sounds here

            //Reset the Flux to zero
            CmdResetFlux();
        }
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

    //==============


    [Command]
    void CmdReplaceMe()
    {
        GameObject tempOracle = Instantiate(Oracle_Ref, transform.position, Quaternion.identity) as GameObject;
        IngameOracle IGO = tempOracle.GetComponent<IngameOracle>();
        NetworkIdentity nIGO = tempOracle.GetComponent<NetworkIdentity>();
        IGO.nameIngame = nameIngame;
        IGO.typeIngame = typeIngame;
        IGO.colorIngame = colorIngame;
        nIGO.localPlayerAuthority = true;
        NetworkServer.Spawn(tempOracle);
        NetworkServer.ReplacePlayerForConnection(connectionToClient, tempOracle, playerControllerId);
        NetworkServer.Destroy(gameObject);
    }
}
