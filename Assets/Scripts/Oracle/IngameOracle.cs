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
            Instantiate(Oracle_Camera, transform.position, Quaternion.identity);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().target = gameObject.transform;
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (!isLocalPlayer)
        {
            return;
        }
        aniRef.SetFloat("HealthPercent", entityRef.healthPoints/100);

	}
}
