﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Entity : NetworkBehaviour {

    [SyncVar(hook = "HealthChange")]
    public float healthPoints;
    private float previousHealth;

    [SyncVar(hook = "DeathStateChange")]
    public bool deathState = false;

    private Animator aniRef;
    private AudioSource aSRef;
    private SpriteRenderer spRef;
    private Camera camRef;
    private Rigidbody2D rBody2D;
    private Color hold;

    public AudioClip hitSound;
    public AudioClip deathSound;

    [SerializeField]
    private int hitShakeIntesity, hitShakeAmount, deathShakeIntesity, deathShakeAmount;


    void Start()
    {
        previousHealth = healthPoints;
        spRef = gameObject.GetComponent<SpriteRenderer>();
        aniRef = gameObject.GetComponent<Animator>();
        rBody2D = gameObject.GetComponent<Rigidbody2D>();
        aSRef = gameObject.GetComponent<AudioSource>();
        hold = spRef.color;

        if (isLocalPlayer)
        {
            StartCoroutine(GetCameraDealy());
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (gameObject.tag == "Player")
        {
            if (!isLocalPlayer)
                return;
        }
        if (gameObject.tag == "Enemy")
        {
            if (!isServer)
                return;
        }
        
        if (healthPoints <= 0 && !deathState)
        {
            CmdDeath();
        }

        if (healthPoints > 0 && deathState)
        {
            CmdRevive();
        }
    }

  
    // Commands for Chaning deathstate
    [Command]
    public void CmdDeath()
    {
        deathState = true;
        // DeathStateChange(deathState);
    }

    /// <summary>
    /// Revive the player
    /// </summary>
    [Command]
    public void CmdRevive()
    {
        deathState = false;
        // DeathStateChange(deathState);
    }

    // Hook for Deathstate
    private void DeathStateChange(bool state)
    {
        deathState = state;
        if (state)
        {
            // Play Sound
            aSRef.clip = deathSound;
            aSRef.Play();

            // Play Animation           

            // Contrain rigidbody
            if (GetComponent<Rigidbody2D>())
            {
                rBody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            // For Players
            if (gameObject.tag == "Player")
            {
                aniRef.SetTrigger("death");

                if (isLocalPlayer)
                {
                    camRef.GetComponent<CameraFollow>().ScreenShake(deathShakeIntesity, deathShakeAmount);
                }

                else if (GetComponent<Turret>())
                {
                    StartCoroutine(NetworkDestroy(1f));
                }
            }
            // For Enemies
            else if (gameObject.tag == "Enemy")
            {
                // Add animtion on complete network destroy float 0.1f
                StartCoroutine(NetworkDestroy(1f));
                gameObject.GetComponent<AiEnemy>().EnemyDestoryed();
            }
            else
            {
                StartCoroutine(NetworkDestroy(0.1f));
            }
        }
        else if (!state)
        {
            // Play Respawn animation
            rBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            CmdAddHealth(100);
            aniRef.SetTrigger("revive");
            GetComponent<CustomNetworkAnim>().SetWeaponDisplay(1);
        }
    }

    // Commands for chaging health
    [Command]
    public void CmdAddHealth(float amount)
    {
        healthPoints += amount;
        healthPoints = Mathf.Clamp(healthPoints, -100f, 100f);
    }

    [Command]
    public void CmdSubtractHealth(float amount)
    {
        aSRef.clip = hitSound;
        aSRef.Play();
        healthPoints -= amount;
        healthPoints = Mathf.Clamp(healthPoints, -100f, 100f);
    }

    // Hook for the healthvariable
    private void HealthChange(float input)
    {
        healthPoints = input;
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

    // Flash when damaged
    private void HitFlash()
    {
        aSRef.clip = hitSound;
        aSRef.Play();
        StartCoroutine(ColorFlash(Color.red));
        if (isLocalPlayer)
        {
            if (!camRef)
            {
                camRef = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            }
            camRef.GetComponent<CameraFollow>().ScreenShake(hitShakeIntesity, hitShakeAmount);
        }
    }

    // Flash when healed
    private void HealFlash()
    {
        StartCoroutine(ColorFlash(Color.green));
    }

    /// <summary>
    /// Flash with color
    /// </summary>
    /// <param name="toColor">The Color it flashes to</param>
    /// <returns>No return, is wait time</returns>
    IEnumerator ColorFlash(Color toColor)
    {
        spRef.color = toColor;
        yield return new WaitForSeconds(0.1f);
        spRef.color = hold;
    }

    // Delayed Network Destroy
    IEnumerator NetworkDestroy(float Wait)
    {
        yield return new WaitForSeconds(Wait);
        CmdDestroyGameObject();
    }

    [Command]
    void CmdDestroyGameObject()
    {
        NetworkServer.Destroy(gameObject);
    }

    IEnumerator GetCameraDealy()
    {
        yield return new WaitForSeconds(1f);
        camRef = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
}