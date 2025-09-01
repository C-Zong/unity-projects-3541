using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 movementVector;
    private float upDownInput;
    public float speed = 1.0f;

    // The following input handling method (OnMove) was adapted from Unity Input System tutorial examples.
    private void OnMove(InputValue movementValue)
    {
        movementVector = movementValue.Get<Vector2>();
    }

    private void OnUpDown(InputValue value)
    {
        upDownInput = value.Get<float>();
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementVector.x, upDownInput, movementVector.y);
        // The original tutorial suggested AddForce which caused constant movement.
        // I asked AI how to make the player move only when pressing keys.
        // AI suggested using transform.Translate().
        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }
}