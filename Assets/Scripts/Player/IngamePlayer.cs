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
    public GameObject Oracle_Camer;

    // Editor Variaels
    public float fluxDeliveryRange;

    // Private refrences
    private PickUp pickRef;
    private Entity entiRef;
    private GameObject oracleRef;

    [SerializeField]
    private string[] AnimCons;


    // Use this for initialization
    void Start ()
    {
        GetComponent<Animator>().runtimeAnimatorController = ((RuntimeAnimatorController)(Resources.Load(AnimCons[typeIngame])));
        nameField.text = nameIngame;
        nameField.color = colorIngame;
        pickRef = GetComponent<PickUp>();

        // Create Camera if this is the local player
        if (isLocalPlayer)
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
}
