using UnityEngine;
using UnityEngine.InputSystem;

// Control the movement of the target sphere in 3D space
public class TargetController : MonoBehaviour
{
    private Vector2 movementVector;
    private float upDownInput;
    public float speed = 1.0f;

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
        transform.Translate(movement * speed * Time.deltaTime, Camera.main.transform);
    }
}
