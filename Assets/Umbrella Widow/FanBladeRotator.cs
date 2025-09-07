using UnityEngine;

public class FanBladeRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 300f; // degrees per second
    public Vector3 rotationAxis = Vector3.forward; // default Z-axis

    void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
