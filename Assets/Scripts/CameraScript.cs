using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // The target (ball) to follow
    public Vector3 offset;         // Offset distance between the camera and ball
    public float smoothSpeed = 0.125f; // Smoothness of the camera's movement

    void Start()
    {
        // Set the initial offset
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        // Calculate the desired position based on the ball's position and the offset
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly move the camera toward the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        // Update the camera's position
        transform.position = smoothedPosition;

        // Optionally, make the camera always look at the ball
        transform.LookAt(target);
    }
}
