using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{

    public GameObject health;
    public GameObject ammo;
    int hp;
    int ammoAmount;

    // Start is called before the first frame update
    void Start()
    {
        hp = 100;
        ammoAmount = 5;
        health.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 100.ToString();
        ammo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 5.ToString();
    }

    public void Shoot()
    {
        ammoAmount--;
        ammo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = ammoAmount.ToString();
    }

    public void LoseHealth(int amount) //take damage for amount
    {
        hp -= amount;
        if (hp > 0)
        {
            Updatehealth();
        }
        else
        {
            GameOver();
        }
    }

    public void GainHealth(int amount) //Heal player for amount
    {
        hp += amount;
        if (hp > 100)
        {
            hp = 100;
        }
        Updatehealth();
    }

    private void Updatehealth()
    {
        health.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = hp.ToString();
    }

    private void GameOver()
    {
        health.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "0";
    }
}
