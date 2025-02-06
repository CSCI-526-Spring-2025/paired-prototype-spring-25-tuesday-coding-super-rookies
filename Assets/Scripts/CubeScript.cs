using UnityEngine;

public class CubeController : MonoBehaviour
{
    public float tiltSpeed = 10f;  // Speed at which the cube tilts

    void Update()
    {
        // Get horizontal and vertical input (Arrow keys or 'A', 'D', 'W', 'S')
        float horizontalInput = Input.GetAxis("Horizontal");  // Left/Right (Arrow keys or 'A'/'D')
        float verticalInput = Input.GetAxis("Vertical");      // Up/Down (Arrow keys or 'W'/'S')

        // Apply rotation to the cube
        transform.Rotate(Vector3.right * verticalInput * tiltSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * horizontalInput * tiltSpeed * Time.deltaTime);
    }
}
