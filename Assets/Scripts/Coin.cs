using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField][Range(0f, 1f)] private float soundVolume = 0.5f;

    private bool collected = false;

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            // Play sound
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, soundVolume);
            }

            // Destroy coin instantly
            Destroy(gameObject);
        }
    }
}