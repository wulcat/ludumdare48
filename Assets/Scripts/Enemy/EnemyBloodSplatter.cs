using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBloodSplatter : MonoBehaviour
{
    bool firstSpawn = true;
    private void OnDisable()
    {
        if(firstSpawn)
        {
            firstSpawn = false;
        }
        else
        {
            ObjectPooler.instance.DestroyObject("EnemyBloodSplatter", gameObject);
        }
    }
}
