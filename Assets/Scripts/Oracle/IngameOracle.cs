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
    private GameObject camRef;
    [SerializeField]
    private GameObject UI_Ref;
    public Image healthBar;
    public Image colorDisplay;

    // Refrences
    private Entity entityRef;
    private Animator aniRef;

    // Use this for initialization
    void Start ()
    {
        nameField = GetComponentInChildren<Text>();
        colorDisplay.color = colorIngame;
        entityRef = GetComponent<Entity>();
        aniRef = GetComponent<Animator>();
        if (isLocalPlayer)
        {
            UI_Ref = Instantiate(UI_Ref);
            UI_Ref.GetComponent<InGameUI>().oracleRef = gameObject;
            UI_Ref.GetComponent<InGameUI>().EnableOracle();
            Invoke("CmdAddWithDealy", 1f);
            camRef = Instantiate (Oracle_Camera, transform.position, Quaternion.identity) as GameObject;
            camRef.GetComponent<CameraFollow>().target = gameObject.transform;
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
        healthBar.transform.localScale = new Vector2((Mathf.Clamp(gameObject.GetComponent<Entity>().healthPoints, 0, 100) / 100), 1);
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

        if (!isLocalPlayer)
        {
            return;
        }
        if (flux != 0)
        {
            entityRef.CmdAddHealth(flux);
            CmdResetFlux();
        }
        if (gameObject.GetComponent<Entity>().healthPoints <= 0)
        {
            CentralScript.instance.CmdDefeat();
        }
        aniRef.SetFloat("HealthPercent", entityRef.healthPoints/100);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "EndOfLevel")
        {
            CentralScript.instance.CmdVictory();
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
    //======================
    [Command]
    void CmdAddWithDealy()
    {
        CentralScript.instance.CmdAddPlayer(nameIngame, typeIngame, colorIngame);
    }
    [Command]
    void CmdVictory()
    {
        CentralScript.instance.CmdVictory();
    }
    [Command]
    void CmdDefeat()
    {
        CentralScript.instance.CmdDefeat();
    }

}
