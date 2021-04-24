using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField]
    private Shotgun startingWeapon;

    private IGun currentWeapon;
    // Start is called before the first frame update
    private void Awake()
    {
        currentWeapon = startingWeapon;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnShoot()
    {
        currentWeapon.Shoot();
    }
}
