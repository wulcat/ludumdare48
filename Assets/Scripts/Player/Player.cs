using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100;
    private float currentHealth;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private List<AudioClip> gruntSounds = new List<AudioClip>();
    public float getHitAgainTime = 1;
    float canTakeHit;
    bool invulnerable = false;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }
    
    private void Update()
    {
        if(Time.time >= canTakeHit)
        {
            if(invulnerable)
            {
                invulnerable = false;
            }
        }
    }

    public void TakeDamage(float value)
    {
        invulnerable = true;
        canTakeHit = Time.time + getHitAgainTime;
        if(currentHealth - value > 0)
        {
            currentHealth -= value;
            audioSource.clip = gruntSounds[Random.Range(0, gruntSounds.Count - 1)];
            audioSource.Play();
        }
        else
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        print("dieded");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            if (!invulnerable)
            {
                TakeDamage(other.GetComponent<Enemy>().touchDamage); 
            }
        }
    }
}
