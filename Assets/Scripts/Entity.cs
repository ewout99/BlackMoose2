using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Entity : NetworkBehaviour {

    [SyncVar(hook = "HealthChange")]
    public float healthPoints;
    private float previousHealth;

    private Animator aniRef;
    private SpriteRenderer spRef;
    private Camera camRef;

    void Awake()
    {
        previousHealth = healthPoints;
        spRef = gameObject.GetComponent<SpriteRenderer>();
        aniRef = gameObject.GetComponent<Animator>();
        if (isLocalPlayer)
        {
            camRef = FindObjectOfType<Camera>();
        }
    }
    // Update is called once per frame
    void Update(){
        if (healthPoints <= 0)
        {
            CmdDeath();
        }
    }

    public void CmdAddHealth(float amount)
    {
        if (!isServer)
            return;
        healthPoints += amount;
    }


    public void CmdSubtractHealth(float amount)
    {
        if (!isServer)
            return;
        healthPoints -= amount;
    }


    [Command]
    private void CmdDeath()
    {
        // Play Sound

        // Play Animation

        // Destroy
        NetworkServer.Destroy(gameObject);
    }

    private void HealthChange(float input)
    {
        if (healthPoints > previousHealth)
        {
            HealFlash();
            previousHealth = healthPoints;
        }
        else
        {
            HitFlash();
            previousHealth = healthPoints;
        }
    }

    private void HitFlash()
    {
        Debug.Log("Health Down");
        StartCoroutine(ColorFlash(Color.red));
        if (isLocalPlayer)
        {
            Debug.Log("Camera Shake");
            camRef.GetComponent<CameraFollow>().ScreenShake(2, 10);
        }
    }

    private void HealFlash()
    {
        Debug.Log("Health up");
        StartCoroutine(ColorFlash(Color.green));
    }

    IEnumerator ColorFlash(Color toColor)
    {
        Color hold = gameObject.GetComponent<SpriteRenderer>().color;
        spRef.color = toColor;
        yield return new WaitForSeconds(0.1f);
        spRef.color = hold;
    }
}