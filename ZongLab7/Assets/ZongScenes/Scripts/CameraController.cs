using UnityEngine;
using UnityEngine.InputSystem;

// Control the movement of the camera around the origin
public class CameraController : MonoBehaviour
{
    private Vector2 movementVector;
    private float upDownInput;
    public float speed = 1.0f;
    public float rotationSpeed = 100.0f;

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
        Vector3 pos = transform.position;

        float angle = movementVector.x * rotationSpeed * Time.deltaTime;
        pos = Quaternion.Euler(0, angle, 0) * pos;

        float horizontalDist = new Vector3(pos.x, 0, pos.z).magnitude;
        horizontalDist += movementVector.y * speed * Time.deltaTime;
        horizontalDist = Mathf.Max(0.1f, horizontalDist);
        Vector3 horizontalDir = new Vector3(pos.x, 0, pos.z).normalized;
        pos.x = horizontalDir.x * horizontalDist;
        pos.z = horizontalDir.z * horizontalDist;

        pos.y += upDownInput * speed * Time.deltaTime;
        transform.position = pos;

        transform.LookAt(Vector3.zero);
    }
}
