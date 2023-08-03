using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    // Function called when another collider enters the trigger collider of this game object
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering collider belongs to the sphere
        if (other.CompareTag("TargetSphere")) // Replace "targetSphere" with the actual tag of your sphere
        {
            Debug.Log("Collision Detected with: " + other.gameObject.name);

            // Get the Audio Source from the sphere
            AudioSource audioSource = other.GetComponent<AudioSource>();

            // Check if the Audio Source exists and play the audio clip
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
                Debug.Log("play");
            }
        }
    }
}
