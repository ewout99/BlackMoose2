using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class IngamePlayer : NetworkBehaviour {

    [SyncVar]
    public string nameIngame;
    [SyncVar]
    public int typeIngame;
    [SyncVar]
    public Color colorIngame;

    [SyncVar]
    public float flux = 0f;

    // Editor refrences
    public GameObject Player_Camera;
    private GameObject camRef;
    public GameObject UI_Ref;
    public GameObject Oracle_Ref;

    public Image healthBar;
    public Image colorDisplay;

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
        colorDisplay.color = colorIngame;
        nameIngame = nameIngame.Length > 12 ? nameIngame.Substring(0, 12) : nameIngame;
        pickRef = GetComponent<PickUp>();
        GetComponent<Animator>().runtimeAnimatorController = ((RuntimeAnimatorController)(Resources.Load(AnimCons[typeIngame])));

        if (isLocalPlayer && typeIngame == 4)
        {
            CmdReplaceMe();
            return;
        }
        else if (isLocalPlayer)
        {
            camRef = Instantiate(Player_Camera, transform.position, Quaternion.identity) as GameObject;
            camRef.GetComponent<CameraFollow>().target = gameObject.transform;
            UI_Ref = Instantiate(UI_Ref);
            Invoke("AddWithDealy", 2f);
        }
    }

    // Update is called once per frame
    void Update () {

        healthBar.transform.localScale = new Vector2((Mathf.Clamp(gameObject.GetComponent<Entity>().healthPoints, 0, 100) / 100),1);
        if (gameObject.GetComponent<Entity>().healthPoints > 50)
        {
            healthBar.color = Color.green;
        }
        else if (gameObject.GetComponent<Entity>().healthPoints <= 50 && gameObject.GetComponent<Entity>().healthPoints >= 20)
        {
            healthBar.color = Color.yellow;
        }
        else
        {
            healthBar.color = Color.red;
        }
        // Check if its the local player
        if (!isLocalPlayer)
            return;

        if (!oracleRef)
        {
            oracleRef = GameObject.Find("Temp Oracle(Clone)");
            return;
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

    void AddWithDealy()
    {
        CentralScript.instance.CmdAddPlayer(nameIngame, typeIngame, colorIngame);
    }
}
