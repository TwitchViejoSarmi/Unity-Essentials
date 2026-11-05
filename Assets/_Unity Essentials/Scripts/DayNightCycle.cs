using UnityEngine;

/// <summary>
/// Attach this to your Directional Light.
/// Rotates the light around the X axis to simulate the sun moving across the sky.
/// </summary>
[RequireComponent(typeof(Light))]
public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("How many real-world seconds a full 24-hour day takes.")]
    [Min(0.01f)]
    public float secondsPerDay = 120f;

    [Tooltip("Starting time of day in hours (0 = midnight, 6 = sunrise, 12 = noon, 18 = sunset).")]
    [Range(0f, 24f)]
    public float startTimeOfDay = 8f;

    [Header("Sun Orientation")]
    [Tooltip("Rotation around the Y axis. Changes the compass direction the sun travels.")]
    public float sunYaw = 170f;

    // Current time in hours (0-24)
    private float timeOfDay;

    private void Start()
    {
        timeOfDay = startTimeOfDay;
        UpdateSunRotation();
    }

    private void Update()
    {
        // Advance time: 24 in-game hours spread over secondsPerDay real seconds
        timeOfDay += (24f / secondsPerDay) * Time.deltaTime;

        if (timeOfDay >= 24f)
            timeOfDay -= 24f;

        UpdateSunRotation();
    }

    private void UpdateSunRotation()
    {
        // Map 0-24 hours to 0-360 degrees, offset so that:
        // 6h  -> 0°   (sunrise on the horizon)
        // 12h -> 90°  (sun directly overhead)
        // 18h -> 180° (sunset on the horizon)
        // 0h  -> 270° (midnight, sun below the ground)
        float sunAngle = (timeOfDay / 24f) * 360f - 90f;

        transform.rotation = Quaternion.Euler(sunAngle, sunYaw, 0f);
    }
}
