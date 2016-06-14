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
    public float fluxDeliveryRange;

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

        // If the oracle is near by send your flux to the oracle
        if (true)
        {
            // Temp blockade
        }
        else if (flux > 0 || (oracleRef.transform.position - transform.position).magnitude <= fluxDeliveryRange)
        {
            // Call oracle ref send flux to it

            //Reset the Flux to zero
            flux = 0;
        }
	}

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
