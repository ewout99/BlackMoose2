﻿using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    public Transform target;

    // Camera follow
    [SerializeField]
    private float smooth;
    private float zPos = -10f;

    // Camera shake
    [SerializeField]
    private float shakeIntesity;
    private int shakeCount = 10;
    private bool shaking = false;

    // Refereneces
    private Camera camRef;

    // Use this for initialization
    void Start()
    {
        camRef = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null && !shaking)
        {
            float smoothX = Mathf.Lerp(transform.position.x, target.position.x, Time.deltaTime * smooth);
            float smoothY = Mathf.Lerp(transform.position.y, target.position.y, Time.deltaTime * smooth);
            transform.position = new Vector3(smoothX, smoothY, zPos);
        }
    }

    /// <summary>
    /// Shakes the camera
    /// </summary>
    /// <param name="intetsity">The Intesisty of the shake</param>
    /// <param name="count">The times it shakes 1 sec = 28 Shakes</param>
    public void ScreenShake(float intetsity, int count)
    {
        shakeIntesity = intetsity;
        shakeCount = count;
        StartCoroutine(StartScreenShake());
    }

    // Copy of the previous funtion without params
    private void ScreenShake()
    {
        StartCoroutine(StartScreenShake());
    }

    // Starts coroutine for background flash
    public void ScreenFlash()
    {
        StartCoroutine(ColorFlash());
    }


    // Corotine for camera shake
    IEnumerator StartScreenShake()
    {
        Vector3 newPos;
        for (int i = 0; i < shakeCount; i++)
        {
            float shakeAmount = Random.Range(-shakeIntesity, shakeIntesity) * Time.deltaTime;
            newPos = transform.position;
            newPos.x += shakeAmount;
            newPos.y += shakeAmount;
            newPos.z = zPos;
            transform.position = newPos;
            shakeIntesity *= 0.8f;
            yield return new WaitForSeconds(0.03f);
        }

        // try perlin noise here
    }

    IEnumerator ColorFlash()
    {
        Color Hold = camRef.backgroundColor;
        camRef.backgroundColor = Color.white;
        yield return new WaitForSeconds(0.1f);
        camRef.backgroundColor = Hold;
    }

}
