using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NewDirectionType
{
    Random,
    AlwaysLeft,
    AlwaysRight,
    Opposite
}
public class EnemyMovement : MonoBehaviour
{
    public float speed = 1;
    [Tooltip("How the enemy picks its new destination when target or wall is reached")]
    public NewDirectionType newDirectionType;
    [Tooltip("If wait time before new target is random")]
    public bool ifRandomWaitTime;
    public float randomWaitMin;
    public float randomWaitMax;
    public float waitTime;
    public float newTargetDistance;
    public bool ifTargetDistanceRandom;
    public float targetDistanceMax;
    public LayerMask layerMask;
    public Animator animator;

    Vector3 target;
    float canNewTarget;
    bool inTarget;

    List<int> directions = new List<int> { 0, 45, 90, 135, 180, 225, 270, 315, 360 };

    private void Start()
    {
        NewTarget();
    }
    void Update()
    {
        if(inTarget)
        {
            if(Time.time >= canNewTarget)
            {
                NewTarget();
            }
        }
    }

    public void NewTarget()
    {
        inTarget = false;
        Vector3 direction = new Vector3();
        switch (newDirectionType)
        {
            case NewDirectionType.Random:
                direction = new Vector3(0, directions[Random.Range(0, directions.Count - 1)], 0);
                break;
            case NewDirectionType.AlwaysLeft:
                direction = new Vector3(0, transform.localEulerAngles.y - 45, 0);
                break;
            case NewDirectionType.AlwaysRight:
                direction = new Vector3(0, transform.localEulerAngles.y + 45, 0);
                break;
            case NewDirectionType.Opposite:
                direction = new Vector3(0, transform.localEulerAngles.y + 180, 0);
                break;
            default:
                break;
        }
        transform.localEulerAngles = direction;
        float distance;
        if (ifTargetDistanceRandom)
        {
            distance = Random.Range(0, targetDistanceMax);
        }
        else
        {
            distance = newTargetDistance;
        }
        RaycastHit hit;
        if (Physics.BoxCast(transform.position, transform.localScale / 2, transform.forward, out hit, transform.rotation, distance, layerMask))
        {
            distance = hit.distance - .7f;
        }
        target = transform.position + transform.forward * distance;
        StartCoroutine(MoveToTarget(target));
    }

    IEnumerator MoveToTarget(Vector3 targetVector)
    {
        while (transform.position.x != targetVector.x || transform.position.z != targetVector.z)
        {
            animator.SetBool("isMoving", true);
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetVector, step);
            if(!enabled)
            {
                break;
            }
            yield return new WaitForSeconds(.002f);
        }
        animator.SetBool("isMoving", false);
        CalculateNewWaitTime();
    }

    void CalculateNewWaitTime()
    {
        if (ifRandomWaitTime)
        {
            canNewTarget = Random.Range(randomWaitMin, randomWaitMax) + Time.time;
        }
        else
        {
            canNewTarget = waitTime + Time.time;
        }
        inTarget = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(target, transform.localScale);
    }
}
