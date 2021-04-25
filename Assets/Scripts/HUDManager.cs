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
    public Image hundreds;
    public Image tens;
    public Image ones;
    public Sprite[] numbers;
    public Sprite[] mutes;
    public Image mute;

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
        mute.sprite = AudioManager.instance.mute ? mutes[1] : mutes[0]; //The text on Start
    }

    public void Updateammo(int amount)
    {
        ammo.text = amount.ToString(); //update current ammo count to HUD
    }

    public void Mute()
    {
        AudioManager.instance.Mute();
        mute.sprite = AudioManager.instance.mute ? mutes[1] : mutes[0];
    }


    public void Updatehealth(int amount)
    {
        char[] a = amount.ToString().ToCharArray();
        List<int> i = new List<int>();
        foreach (var item in a)
        {
            i.Add(int.Parse(item.ToString()));
        }

        if (i.Count == 3)
        {
            hundreds.sprite = numbers[i[0]];
            tens.sprite = numbers[i[1]];
            ones.sprite = numbers[i[2]];
        }
        else if (i.Count == 2)
        {
            hundreds.gameObject.SetActive(false);
            tens.sprite = numbers[i[0]];
            ones.sprite = numbers[i[1]];
        }
        else
        {
            hundreds.gameObject.SetActive(false);
            tens.gameObject.SetActive(false);
            ones.sprite = numbers[i[0]];
        }
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
