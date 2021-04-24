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
    [Tooltip("How the enemy picks its new destination when target or wall is reached")]
    public NewDirectionType newDirectionType;
    [Tooltip("If wait time before new target is random")]
    public bool ifRandomWaitTime;
    public float randomWaitMin;
    public float randomWaitMax;
    public float waitTime;

    List<int> directions = new List<int> {0,45,90,135,180,225,270,315,360 };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewTarget()
    {
        Vector3 direction = new Vector3();
        switch (newDirectionType)
        {
            case NewDirectionType.Random:
                direction = new Vector3(0, direction[Random.Range(0, directions.Count - 1)], 0);
                break;
            case NewDirectionType.AlwaysLeft:
                break;
            case NewDirectionType.AlwaysRight:
                break;
            case NewDirectionType.Opposite:
                break;
            default:
                break;
        }
    }
}
