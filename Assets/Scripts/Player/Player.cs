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
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void TakeDamage(float value)
    {
        print("auts");
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
}
