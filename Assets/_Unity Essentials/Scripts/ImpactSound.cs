using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ImpactSound : MonoBehaviour
{
    public AudioClip impactClip;
    public float minImpactSpeed = 0.5f;   // ignore tiny touches / rolling
    public float maxImpactSpeed = 10f;    // speed that plays at full volume
    public Vector2 pitchRange = new Vector2(0.9f, 1.1f);

    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        float impact = collision.relativeVelocity.magnitude;
        if (impact < minImpactSpeed) return;

        float volume = Mathf.InverseLerp(minImpactSpeed, maxImpactSpeed, impact);
        source.pitch = Random.Range(pitchRange.x, pitchRange.y);
        source.PlayOneShot(impactClip, volume);
    }
}