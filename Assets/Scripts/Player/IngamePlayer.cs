using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class IngamePlayer : NetworkBehaviour {

    [SyncVar(hook = "NameChange")]
    public string nameIngame;
    [SyncVar(hook = "TypeChange")]
    public string typeIngame;
    [SyncVar(hook = "ColorChange")]
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


    // Use this for initialization
    void Start ()
    {
        // Getting refrences
        oracleRef = GameObject.FindGameObjectWithTag("Oracle");
        GetComponent<SpriteRenderer>().color = colorIngame;
        pickRef = GetComponent<PickUp>();

        // Create Camera if this is the local player
        if (isLocalPlayer)
        {
            Instantiate(Player_Camera, transform.position, Quaternion.identity);
        }
	}
	
	// Update is called once per frame
	void Update () {

        // Check if its the local player
        if (!isLocalPlayer)
            return;

        // If the oracle is near by send your flux to the oracle
        if (flux > 0 || (oracleRef.transform.position - transform.position).magnitude <= fluxDeliveryRange)
        {
            // Call oracle ref send flux to it

            //Reset the Flux to zero
            flux = 0;
        }
	}
    
    void NameChange(string newName)
    {
        nameIngame = newName;
    }

    void TypeChange(string newType)
    {
        typeIngame = newType;
    }

    void ColorChange (Color newColor)
    {
        colorIngame = newColor;
    }
}
