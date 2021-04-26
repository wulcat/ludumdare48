using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuBehavior : MonoBehaviour
{
    public Image mute;
    public Sprite[] mutes;

    public static void NewGame()
    {
        SceneManager.LoadScene("DemoScene"); //assuming level 1 name
    }

    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Mute()
    {
        AudioManager.instance.Mute();
        mute.sprite = AudioManager.instance.mute ? mutes[1] : mutes[0];
    }
}
