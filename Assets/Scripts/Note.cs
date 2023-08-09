using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Note : MonoBehaviour
{
    
    public event Action<Note> OnNoteCollected;
    public bool Collected => collected;
    public float height;
    public string noteName;
    public AudioSource audioSource;

    private bool collected = false;

    private void Start()
    {
        audioSource = gameObject.GetComponentInChildren<AudioSource>();
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
        collected = true;
        OnNoteCollected?.Invoke(this);

    }
}
