using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;
    public void Initialize(Vector3 start, Vector3 to)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, to);
        StartCoroutine(DestroyObject());
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(.05f);
        ObjectPooler.instance.DestroyObject("BulletTrail", gameObject);
    }
}
