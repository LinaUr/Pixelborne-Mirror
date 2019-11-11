using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //another hardcoded way with the name of scene:
        //SceneManager.LoadScene("Multiplayer");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
