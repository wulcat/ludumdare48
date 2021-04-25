using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour, IGun
{
    [SerializeField]
    private float fireRate;
    public float FireRate { get { return fireRate; } set { fireRate = value; } }
    [SerializeField]
    private bool isAutomatic;
    public bool IsAutomatic { get { return IsAutomatic; } set { IsAutomatic = value; } }
    [SerializeField]
    private float damage;
    public float Damage { get { return damage; } set { damage = value; } }
    [SerializeField]
    private GameObject muzzle;
    public GameObject Muzzle { get { return muzzle;} set { muzzle = value; } }
    [SerializeField]
    private AudioSource audioSource;
    public AudioSource AudioSource { get { return audioSource; }set { audioSource = value; } }

    private bool shooting;
    public bool Shooting { get { return shooting; }set { shooting = value; } }
    [SerializeField]
    private Transform muzzleTransform;
    public Transform MuzzleTransform { get { return muzzleTransform; }set { muzzleTransform = value; } }

    private float canShoot = 0;
    public LayerMask layerMask;

    public void Reload()
    {
        throw new System.NotImplementedException();
    }

    public void Shoot()
    {
        shooting = true;
    }

    void FireWeapon()
    {
        audioSource.Play();
        shooting = false;
        canShoot = Time.time + 1 / fireRate;
        int dir = 0;
        for (int i = 0; i < 5; i++)
        {
            if (i > 0)
            {
                if(i % 2 == 0)
                {
                    dir *= -1;
                }
                else
                {
                    dir = (dir + 10) * (-1);
                }
                muzzleTransform.localEulerAngles += new Vector3(0, dir, 0);
            }
            RaycastHit hit;
            if (Physics.Raycast(muzzleTransform.position, muzzleTransform.forward, out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(muzzleTransform.position, muzzleTransform.forward * hit.distance, Color.red, 2f);
                if(hit.transform.CompareTag("Enemy"))
                {
                    hit.transform.GetComponent<Enemy>().TakeDamage(damage, hit.point);
                }
                else
                {
                    ObjectPooler.instance.SpawnFromPool("WallParticles", hit.point, hit.transform.rotation, ObjectPooler.instance.pools[2]);
                }
            }
            else
            {
                Debug.DrawRay(muzzleTransform.position, muzzleTransform.forward * 1000, Color.white, 2f);
            }
            muzzleTransform.localEulerAngles = Vector3.zero;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        muzzle.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(shooting && Time.time >= canShoot)
        {
            FireWeapon();
            StartCoroutine(MuzzleOnOffDelay());
        }
        else
        {
            shooting = false;
        }
    }

    IEnumerator MuzzleOnOffDelay()
    {
        muzzle.SetActive(true);
        yield return new WaitForSeconds(.2f);
        muzzle.SetActive(false);
    }

    public void UnShoot()
    {
        
    }
}
