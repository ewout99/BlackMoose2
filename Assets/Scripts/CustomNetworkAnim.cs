using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CustomNetworkAnim : NetworkBehaviour {

    // SyncVars
    [SyncVar(hook = "SetFire")]
    private bool firing;

    [SyncVar(hook = "SetDirection")]
    private bool direction;

    //Private refrences
    private Animator weaponAniRef;
    private GameObject weaponRef;

    // Use this for initialization
    void Start()
    {
        weaponRef = transform.FindChild("Temp Weapon").gameObject;
        weaponAniRef = weaponRef.GetComponent<Animator>();
    }

    // Send network commands
    [Command]
    public void CmdFire(bool state)
    {
        firing = state;
    }

    [Command]
    public void CmdDirection(bool right)
    {
        direction = right;
    }

    // Recieve variable changes
    void SetFire(bool state)
    {
        if (state)
        {
            weaponAniRef.SetTrigger("fire");
        }
        firing = state;
    }

    void SetDirection(bool state)
    {
        weaponAniRef.SetBool("facingRight", state);
        direction = state;
    }

    void SetWeaponDisplay(int inputInt)
    {
        if (inputInt == 0)
            weaponRef.SetActive(false);
        else
            weaponRef.SetActive(true);
    }
}
