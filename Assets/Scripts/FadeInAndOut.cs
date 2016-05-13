using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class FadeInAndOut : MonoBehaviour {

    // Group to hold multiple panels
    public CanvasGroup[] fadeCanvasGroups;
    public float fadinspeed;
    public float waitTime;

    // The panel that needs to be worked on
    private int currentPanel;

    // Use this for initialization
    void Start() {
        StartCoroutine(FadeIn(fadeCanvasGroups[0]));
    }

    // loads next scene or start Ienumrator for the next panel
    private void NextPanel()
    {
        if (currentPanel >= fadeCanvasGroups.Length)
        {
            SceneManager.LoadScene("02MenuScreen");
            return;
        }
        StartCoroutine(FadeIn(fadeCanvasGroups[currentPanel]));
    }

	
	IEnumerator FadeIn(CanvasGroup Panel)
    {
        Panel.alpha = 0;
        while ( Panel.alpha != 1)
        {
            Panel.alpha += fadinspeed * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(waitTime);
        while (Panel.alpha != 0)
        {
            Panel.alpha -= fadinspeed * Time.deltaTime;
            yield return null;
        }
        currentPanel++;
        NextPanel();
    }
}
