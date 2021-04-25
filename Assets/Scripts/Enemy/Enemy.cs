using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHp;
    private float currentHp;
    public GameObject witchObject;
    public float gunDamage = 10;
    ObjectPooler.Pool pool;
    ObjectPooler objectPooler;
    public AudioSource audioSource;
    public List<AudioClip> hitClips = new List<AudioClip>();
    public AudioClip deathClip;
    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp;
        objectPooler = ObjectPooler.instance;
        pool = objectPooler.pools[1];
        if (GetComponent<EnemyShoot>())
        {
            GetComponent<EnemyShoot>().damage = gunDamage;
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
        audioSource.clip = deathClip;
        audioSource.Play();
        GetComponent<Collider>().enabled = false;
        if (GetComponentInParent<EnemyMovement>())
        {
            if (GetComponentInParent<EnemyMovement>().enabled)
            {
                GetComponentInParent<EnemyMovement>().enabled = false;
                DropWitch();
            }
        }
        while (audioSource.isPlaying)
        {
            yield return new WaitForSeconds(.2f);
        }
        gameObject.SetActive(false);
    }

    void DropWitch()
    {
        witchObject.transform.position = new Vector3(transform.position.x, witchObject.GetComponent<BoxCollider>().size.y / 2, transform.position.z);
    }

    public void PlayHitAudio()
    {
        audioSource.clip = hitClips[Random.Range(0, hitClips.Count - 1)];
        audioSource.Play();
    }
}
