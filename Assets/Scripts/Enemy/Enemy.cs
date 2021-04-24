using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHp;
    private float currentHp;
    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(float value)
    {
        if(currentHp - value > 0)
        {
            currentHp -= value;
        }
        else
        {
            currentHp = 0;
            Die();
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
    }
}
