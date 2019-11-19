using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public void LoadScene(int SceneIndex)
    {
        SceneManager.LoadScene(SceneIndex);
    }
    public void exitApplication()
    {
        Application.Quit();
    }

public void LoaddScene(string sceneL)
    {
        SceneManager.LoadScene("Loading", LoadSceneMode.Additive);
    }
}
