using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public void tryAgain(string TryAgain)
    {
        SceneManager.LoadScene(0);
    }
    public void exitApplication (string Exit)
    {
        Application.Quit();
    }
}
