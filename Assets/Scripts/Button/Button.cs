using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public AudioSource ads;
    public AudioClip clickb;
    void Start()
    {
        ads = GetComponent<AudioSource>();
    }
    public void LoadScene(int SceneIndex)
    {
        ads.clip = clickb;
        ads.Play();
        SceneManager.LoadScene(SceneIndex);
    }
    public void exitApplication()
    {
        Application.Quit();
    }

public void LoaddScene(string sceneL)
    {
        ads.clip = clickb;
        ads.Play();
        SceneManager.LoadScene("Loading", LoadSceneMode.Additive);
    }
}
