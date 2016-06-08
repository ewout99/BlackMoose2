using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CustomNetworkAnim : NetworkBehaviour {

    [SyncVar(hook = "SetFire")]
    private bool firing;

    [SyncVar(hook = "SetDirection")]
    private bool direction;

    private Animator weaponAniRef;

	// Use this for initialization
	void Start ()
    {
        weaponAniRef = transform.FindChild("Temp Weapon").gameObject.GetComponent<Animator>();
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
}
