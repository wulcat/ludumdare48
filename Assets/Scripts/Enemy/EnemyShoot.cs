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

    float _canShoot;
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public ObjectPooler.Pool bulletPool;
    GameObject player;
    ObjectPooler objectPooler;
    public Animator animator;
    void Start()
    {
        _canShoot = 1 / fireRate;
        player = GameManager.instance.player;
        objectPooler = ObjectPooler.instance;
        bulletPool = objectPooler.pools[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= _canShoot)
        {
            animator.SetTrigger("Shoot");
            _canShoot = Time.time + (1 / fireRate);
        }
        TurnTurret();
    }

    public void Shoot()
    {
        bulletSpawn.LookAt(player.transform);
        GameObject bullet = objectPooler.SpawnFromPool("Bullet", bulletSpawn.transform.position, bulletSpawn.transform.rotation, bulletPool);
        Bullet b = bullet.GetComponent<Bullet>();
        b.speed = bulletSpeed;
        b.damage = damage;
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
