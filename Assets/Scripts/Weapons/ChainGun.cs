using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainGun : MonoBehaviour, IGun
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
    public GameObject Muzzle { get { return muzzle; } set { muzzle = value; } }
    [SerializeField]
    private AudioSource audioSource;
    public AudioSource AudioSource { get { return audioSource; } set { audioSource = value; } }

    private bool shooting;
    public bool Shooting { get { return shooting; } set { shooting = value; } }
    [SerializeField]
    private Transform muzzleTransform;
    public Transform MuzzleTransform { get { return muzzleTransform; } set { muzzleTransform = value; } }
    [SerializeField]
    private GameObject model;

    private float canShoot = 0;
    private float currentBarrelSpeed = 0;

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
        if (currentBarrelSpeed <= 2000)
        {
            currentBarrelSpeed += 15;
        }
        else
        {
            if (Time.time >= canShoot)
            {
                canShoot = Time.time + 1 / fireRate;
                audioSource.Play();
                muzzle.SetActive(true);
                RaycastHit hit;
                if (Physics.Raycast(muzzleTransform.position, muzzleTransform.forward, out hit, Mathf.Infinity))
                {
                    Debug.DrawRay(muzzleTransform.position, muzzleTransform.forward * hit.distance, Color.red, 2f);
                }
                else
                {
                    Debug.DrawRay(muzzleTransform.position, muzzleTransform.forward * 1000, Color.white, 2f);
                }
            }
        }
        model.transform.Rotate(new Vector3(0, 0, currentBarrelSpeed * Time.deltaTime));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (shooting)
        {
            FireWeapon();
        }
        else
        {
            SlowDownBarrel();
        }
    }

    public void UnShoot()
    {
        shooting = false;
    }

    void SlowDownBarrel()
    {
        muzzle.SetActive(false);
        if (currentBarrelSpeed > 0)
        {
            currentBarrelSpeed -= 20;
            model.transform.Rotate(new Vector3(0, 0, currentBarrelSpeed * Time.deltaTime));
        }
        else
        {
            currentBarrelSpeed = 0;
        }
    }
}
