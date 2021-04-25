using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public Transform bulletSpawn;
    public GameObject bulletPrefab;
    public GameObject barrel;
    [Tooltip("Check if you want the turret to face player")]
    public bool rotateToPlayer;
    public AudioSource audioSource;

    [Header("Turret stats")]
    [Tooltip("Bullets per second")]
    public float fireRate = 1;
    public float bulletSpeed = 3;
    public float turnSpeed = 1;
    public GameObject player;

    float _canShoot;
    void Start()
    {
        _canShoot = 1 / fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= _canShoot)
        {
            Shoot();
            _canShoot = Time.time + (1 / fireRate);
        }
        TurnTurret();
    }

    void Shoot()
    {
        bulletSpawn.LookAt(player.transform);
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<Bullet>().speed = bulletSpeed;
        //audioSource.Play();
    }

    void TurnTurret()
    {
        if (rotateToPlayer)
        {
            Vector3 direction = player.transform.position - barrel.transform.position;

            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Vector3 directionVector = new Vector3(0, angle, 0);
            float step = .2f * turnSpeed;
            barrel.transform.rotation = Quaternion.RotateTowards(barrel.transform.rotation, Quaternion.Euler(directionVector), step);

        }
    }
}
