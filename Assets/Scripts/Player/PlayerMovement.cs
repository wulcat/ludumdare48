using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Min(0f)]
    private float movementSpeed = 10f;
    [SerializeField, Min(0f)]
    private float turnSpeed = 10f;
    [SerializeField]
    private Rigidbody rb;

    private Vector3 movement;
    private Vector3 turn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }

    public void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
    }

    void MovePlayer()
    {
        Vector3 velocity = new Vector3(movement.x, 0 , movement.y) * movementSpeed * Time.fixedDeltaTime;
        velocity = transform.TransformDirection(velocity);
        rb.velocity = velocity;
    }
    
    public void OnTurn(InputValue value)
    {
        turn = value.Get<Vector2>();
    }

    void RotatePlayer()
    {
        Vector3 turnEuler = new Vector3(0, turn.x, 0) * turnSpeed * Time.fixedDeltaTime;
        rb.angularVelocity = turnEuler;
    }
}
