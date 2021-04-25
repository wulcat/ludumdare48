using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHp;
    private float currentHp;
    public GameObject witchObject;
    public float gunDamage = 10;
    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp;
        if(GetComponent<EnemyShoot>())
        {
            GetComponent<EnemyShoot>().damage = gunDamage;
        }
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
        if(GetComponentInParent<EnemyMovement>())
        {
            if(GetComponentInParent<EnemyMovement>().enabled)
            {
                GetComponentInParent<EnemyMovement>().enabled = false;
                DropWitch();
            }
        }
    }

    void DropWitch()
    {
        witchObject.transform.position = new Vector3(transform.position.x, witchObject.GetComponent<BoxCollider>().size.y / 2, transform.position.z);
    }
}
