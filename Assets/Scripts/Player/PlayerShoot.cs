using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField]
    private GameObject startingWeapon;

    private IGun currentWeapon;
    // Start is called before the first frame update
    private void Awake()
    {
        GameObject go = Instantiate(startingWeapon, transform);
        currentWeapon = go.GetComponent<IGun>();
    }

    public void OnShoot()
    {
        currentWeapon.Shoot();
    }

    public void OnUnShoot()
    {
        currentWeapon.UnShoot();
    }
}
