using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuBehavior : MonoBehaviour
{
    public static void NewGame()
    {
        SceneManager.LoadScene("Level 1"); //assuming level 1 name
    }

    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static void Mute()
    {
        AudioListener.pause = !AudioListener.pause;
        if (AudioListener.pause == true)
            GameObject.Find("MuteButton").GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Un\nmute";
        else
            GameObject.Find("MuteButton").GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Mute";
    }
}
