using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallParticles : MonoBehaviour
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
            ObjectPooler.instance.DestroyObject("WallParticles", gameObject);
        }
    }
}
