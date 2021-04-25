using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public float speed = 1;
    public AudioSource audioSource;
    public GameObject explosion;
    [HideInInspector]
    public float damage;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<Player>().TakeDamage(damage);
            ObjectPooler.instance.DestroyObject("Bullet", gameObject);
        }
        if (other.CompareTag("Wall"))
        {
            ObjectPooler.instance.DestroyObject("Bullet", gameObject);
        }
    }
}
