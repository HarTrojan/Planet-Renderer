using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlanetRenderer planetRenderer;

    [SerializeField] private float orbitSpeed = 2.0f;
    [SerializeField] private float zoomSpeed = 0.002f;

    [SerializeField] private float planetRadius = 512f;

    private float distance = 1024f;

    // Horizontal orbit
    private float yaw = -Mathf.PI / 2f;

    // Vertical orbit
    private float pitch = 0f;

    // Prevents camera from reaching poles
    private float maxPitch = Mathf.PI * 0.49f;

    void Update()
    {
        float heightAboveSurface = distance - planetRadius;

        float panSpeed = orbitSpeed * heightAboveSurface / planetRadius;

        float latitudeScale = Mathf.Max(Mathf.Cos(pitch), 0.05f);

        float yawSpeed = panSpeed / latitudeScale;
        float pitchSpeed = panSpeed;

        // Horizontal movement
        if (Keyboard.current.aKey.isPressed)
            yaw -= yawSpeed * Time.deltaTime;

        if (Keyboard.current.dKey.isPressed)
            yaw += yawSpeed * Time.deltaTime;

        // Vertical movement
        if (Keyboard.current.wKey.isPressed)
            pitch += pitchSpeed * Time.deltaTime;

        if (Keyboard.current.sKey.isPressed)
            pitch -= pitchSpeed * Time.deltaTime;

        // Stop near poles
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        // Zoom
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0)
        {
            heightAboveSurface *= Mathf.Exp(-scroll * zoomSpeed);

            heightAboveSurface =
                Mathf.Max(heightAboveSurface, 0.0001f);

            distance = planetRadius + heightAboveSurface;
        }

        // Spherical orbit coordinates
        Vector3 cameraPos = new Vector3(
            Mathf.Cos(pitch) * Mathf.Cos(yaw),
            Mathf.Sin(pitch),
            Mathf.Cos(pitch) * Mathf.Sin(yaw)
        ) * distance;

        planetRenderer.SetCameraPosition(cameraPos);
    }
}