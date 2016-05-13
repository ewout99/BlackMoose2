using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonReactions : MonoBehaviour {

	public void StartGame()
    {
        SceneManager.LoadScene("03LobbyScreen");
    }

    public void ShowOptiopns()
    {

    }

    public void QuitGame()
    {
        // Thomas didn't allow for more features here Q.Q
        Application.Quit();
    }


}
