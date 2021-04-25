using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public float speed = 1;
    public AudioSource audioSource;
    public GameObject explosion;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

        }
        if (other.CompareTag("Bullet"))
        {

        }
        else
        {
            //Instantiate(explosion, transform.position, Quaternion.identity);
            //audioSource.Play();
            Destroy(gameObject);
        }
    }
}
