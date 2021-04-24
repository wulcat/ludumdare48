using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance = null;

    public TMP_Text health;
    public TMP_Text ammo;
    public GameObject pauseMenu;
    public TMP_Text startMuteText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        health.text = 100.ToString();
        ammo.text = 5.ToString();
        startMuteText.text = AudioManager.instance.mute ? "Un\nmute" : "Mute"; //The text on Start
    }

    public void Updateammo(int amount)
    {
        ammo.text = amount.ToString(); //update current ammo count to HUD
    }

    public void Mute()
    {
        AudioManager.instance.Mute();
    }


    public void Updatehealth(int amount)
    {
        health.text = amount.ToString(); //update current health to HUD from Player
    }

    private void GameOver()
    {
        health.text = "Dead";
    }

    public void Resume()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    //public void LoseHealth(int amount) //take damage for amount
    //{
    //    hp -= amount;
    //    if (hp > 0)
    //    {
    //        Updatehealth();
    //    }
    //    else
    //    {
    //        hp = 0;
    //        GameOver();
    //    }
    //}

    //public void GainHealth(int amount) //Heal player for amount
    //{
    //    hp += amount;
    //    if (hp > 100)
    //    {
    //        hp = 100;
    //    }
    //    Updatehealth();
    //}


}
