using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    public GameObject panel;
    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    // Dilersen takýlma azalmasý için async sürüm:
    public void PlayAsync()
    {
        StartCoroutine(LoadGame());
    }
    System.Collections.IEnumerator LoadGame()
    {
        var op = SceneManager.LoadSceneAsync("Game");
        op.allowSceneActivation = true; // basit hali
        yield return null;
    }

    public void Setting()
    {
        panel.GetComponent<Animator>().SetTrigger("Pop");

    }
        
}