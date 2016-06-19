using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameUI : MonoBehaviour
{
    // Names
    public Text playerName1;
    public Text playerName2;
    public Text playerName3;
    public Text playerName4;
    // Colors
    public Image playerColor1;
    public Image playerColor2;
    public Image playerColor3;
    public Image playerColor4;
    // Objectives
    public Text Objective1;
    public Text Objective2;
    public Text Objective3;
    public Text Objective4;
    // Victory and defeat
    public Image victory;
    public Image defeat;
    // Oracle
    public Image healPower;
    public Image turretPower;
    public Image revivePower;
    public Image wallPower;

    public GameObject oracleRef;
    private Entity entiRef;
    private bool thing;

    public void EnableOracle()
    {
        Debug.Log("Enabling Oracle");
        healPower.transform.parent.gameObject.SetActive(true);
        entiRef = oracleRef.GetComponent<Entity>();
    }

    void Update()
    {
        if (!oracleRef)
        {
            return;
        }

        // This doesn't work at this time
        if (thing == true)
        {
            // Health
            if (entiRef.healthPoints < 15)
            {
                healPower.GetComponent<Animator>().SetBool("Avaible", false);
            }
            else
            {
                healPower.GetComponent<Animator>().SetBool("Avaible", true);
            }

            // Turrert
            if (entiRef.healthPoints < 10)
            {
                turretPower.GetComponent<Animator>().SetBool("Avaible", false);
            }
            else
            {
                turretPower.GetComponent<Animator>().SetBool("Avaible", true);
            }

            // Revive
            if (entiRef.healthPoints < 20)
            {
                revivePower.GetComponent<Animator>().SetBool("Avaible", false);
            }
            else
            {
                revivePower.GetComponent<Animator>().SetBool("Avaible", true);
            }

            // Wall
            if (entiRef.healthPoints < 5)
            {
                wallPower.GetComponent<Animator>().SetBool("Avaible", false);
            }
            else
            {
                wallPower.GetComponent<Animator>().SetBool("Avaible", true);
            }
        }
    }

    public void SelectPower(string input)
    {
        oracleRef.GetComponent<InputOracle>().SelectPower(input);

    }
}
