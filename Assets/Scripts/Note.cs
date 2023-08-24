using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;

public class Note : MonoBehaviour
{

    public event Action<Note> OnNoteCollected;
    public bool Collected => collected;
    public float height;
    public string noteName;
    public AudioSource audioSource;
    public float hapticAmplitude;
    public float hapticDuration;
  

    private bool collected = false;
    private HapticOutput hapticOutput;

    // added for rotation
    private bool rotated = false;

    private void Start()
    {
        audioSource = gameObject.GetComponentInChildren<AudioSource>();
        hapticOutput = FindObjectOfType<HapticOutput>();
       
    }

    // added for rotation
    private void Update()
    {
        if (!rotated) 
        {
            transform.rotation *= Quaternion.Euler(90f, 0f, 0f);
            rotated = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // object check not required - collision controlled through physics matrix
        NoteHitActivities();
    }
    private void NoteHitActivities()
    {
        // Debug.Log($"Triggered {this.name}: tell {OnNoteCollected.GetInvocationList().Length} subscribers");
        audioSource.Play();
        StartCoroutine(HapticsWithDelay(0.2f));

        collected = true;
        OnNoteCollected?.Invoke(this);

    }

    private IEnumerator HapticsWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hapticOutput.SendHapticResponse(hapticAmplitude, hapticDuration);
    }
}
