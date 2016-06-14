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

    [SerializeField]
    GameObject Oracle_Camera;

    // Use this for initialization
    void Start ()
    {
        if (typeIngame != 4)
        {
            Debug.LogError("AAAAH error, this is not the oracle");
        }
        else
        {
            Debug.Log("Succes oracle");
        }

        nameField = GetComponentInChildren<Text>();

        Debug.Log(isLocalPlayer);
        if (isLocalPlayer)
        {
            Debug.Log("Making Camera");
            Instantiate(Oracle_Camera, transform.position, Quaternion.identity);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().target = gameObject.transform;
        }
        nameField.text = nameIngame;
        nameField.color = colorIngame;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (!isLocalPlayer)
        {
            return;
        }
	}
}
