using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CentralScript : NetworkBehaviour {

    public static CentralScript instance = null;

    public GameObject UIref;
    public InGameUI UICodeRef;

    public SyncListString Names = new SyncListString();
    public SyncListInt Types = new SyncListInt();

    [SyncVar]
    private bool updatePlease = false;

    [SyncVar]
    private bool victory = false;
    private bool victoryCalled;
    [SyncVar]
    private bool defeat = false;
    private bool defeatCalled;

    [SyncVar]
    private Color pc1 = Color.white;
    [SyncVar]
    private Color pc2 = Color.white;
    [SyncVar]
    private Color pc3 = Color.white;
    [SyncVar]
    private Color pc4 = Color.white;

    // Use this for initialization
    void Start () {
        if(instance == null)
        {
            Debug.Log("Singleton made");
            instance = this;
        }
        else
        {
            Debug.LogError("Singleton fail");
        }
        StartCoroutine(GetUI());
	}
	
	// Update is called once per frame
	void Update()
    {   
        if (UICodeRef && updatePlease)
        {
            updatePlease = false;
            UICodeRef.playerName1.gameObject.SetActive(true);
            UICodeRef.playerColor1.gameObject.SetActive(true);
            UICodeRef.playerName1.text = Names[0];
            UICodeRef.playerColor1.color = pc1;
            if (Names.Count == 1) return;
            UICodeRef.playerName2.gameObject.SetActive(true);
            UICodeRef.playerColor2.gameObject.SetActive(true);
            UICodeRef.playerName2.text = Names[1];
            UICodeRef.playerColor1.color = pc2;
            if (Names.Count == 2) return;
            UICodeRef.playerName3.gameObject.SetActive(true);
            UICodeRef.playerColor3.gameObject.SetActive(true);
            UICodeRef.playerName3.text = Names[2];
            UICodeRef.playerColor3.color = pc3;
            if (Names.Count == 3) return;
            UICodeRef.playerName4.gameObject.SetActive(true);
            UICodeRef.playerColor4.gameObject.SetActive(true);
            UICodeRef.playerName4.text = Names[3];
            UICodeRef.playerColor4.color = pc4;
        }

        if(victory && !defeat && !victoryCalled)
        {
            victoryCalled = true;
            UICodeRef.victory.gameObject.SetActive(true);
            StartCoroutine(LoadLevel("02MenuScreen"));
        }
        if(defeat && !victory && !defeatCalled)
        {
            defeatCalled= true;
            UICodeRef.defeat.gameObject.SetActive(true);
            StartCoroutine(LoadLevel("02MenuScreen"));
        }
    }

    [Command]
    public void CmdAddPlayer(string playerName, int typeIngame, Color playerColor)
    {
        Names.Add(playerName);
        Types.Add(typeIngame);
        if (pc1 == Color.white)
        {
            pc1 = playerColor;
        }
        else if (pc2 == Color.white)
        {
            pc2 = playerColor;
        }
        else if (pc3 == Color.white)
        {
            pc3 = playerColor;
        }
        else if (pc4 == Color.white)
        {
            pc4 = playerColor;
        }
        updatePlease = true;
    }

    [Command]
    public void CmdDefeat()
    {
        defeat = true;
    }

    [Command]
    public void CmdVictory()
    {
        victory = true;
    }

    IEnumerator GetUI()
    {
        while (!UIref)
        {
            UIref =  GameObject.FindGameObjectWithTag("UI");
            UICodeRef = UIref.GetComponent<InGameUI>();
            yield return null;
        }
    }

    IEnumerator LoadLevel(string levelToLoad)
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(levelToLoad);
    }
}
