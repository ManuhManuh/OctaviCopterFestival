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
    private void OnCollisionEnter(Collision collision)
    {
        // object check not required - collision controlled through physics matrix

        NoteHitActivities();
    }

    private void NoteHitActivities()
    {
        audioSource.Play();
        collected = true;
        OnNoteCollected?.Invoke(this);

    }
}
