using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHp;
    private float currentHp;
    public GameObject witchObject;
    public float gunDamage = 10;
    public float touchDamage = 10;
    ObjectPooler.Pool pool;
    ObjectPooler objectPooler;
    public AudioSource audioSource;
    public List<AudioClip> hitClips = new List<AudioClip>();
    public AudioClip deathClip;
    [HideInInspector]
    public bool isAlive = true;
    public EnemyShoot enemyShoot;
    public EnemyMovement enemyMovement;
    public GameObject deathExplosion;
    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp;
        objectPooler = ObjectPooler.instance;
        pool = objectPooler.pools[1];
        if (enemyShoot)
        {
            enemyShoot.damage = gunDamage;
        }
    }

    public void TakeDamage(float value, Vector3 hitPoint)
    {
        if(currentHp - value > 0)
        {
            currentHp -= value;
            Vector3 direction = (hitPoint - transform.position).normalized;
            Quaternion bloodRot = Quaternion.LookRotation(direction);
            GameObject blood = objectPooler.SpawnFromPool("EnemyBloodSplatter", hitPoint, bloodRot, pool);
            blood.transform.parent = gameObject.transform;
            blood.transform.localEulerAngles = new Vector3(0, blood.transform.localEulerAngles.y, 0);
            PlayHitAudio();
        }
        else
        {
            currentHp = 0;
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        isAlive = false;
        audioSource.clip = deathClip;
        audioSource.Play();
        GetComponent<Collider>().enabled = false;
        Instantiate(deathExplosion, transform.position, Quaternion.identity);
        if (enemyMovement)
        {
            if (enemyMovement.enabled)
            {
                enemyMovement.enabled = false;
            }
        }
        while (audioSource.isPlaying)
        {
            yield return new WaitForSeconds(.2f);
        }
        DropWitch();
        gameObject.SetActive(false);
    }

    void DropWitch()
    {
        enemyShoot.animator.SetTrigger("InAir");
    }

    public void PlayHitAudio()
    {
        audioSource.clip = hitClips[Random.Range(0, hitClips.Count - 1)];
        audioSource.Play();
    }
    
    public void SetInAirToTrue()
    {
        enemyShoot.inAir = true;
    }

    public void SetInAirToFalse()
    {
        enemyShoot.inAir = false;
        witchObject.transform.position = new Vector3(transform.position.x, -0.8f, transform.position.z);
    }
}
