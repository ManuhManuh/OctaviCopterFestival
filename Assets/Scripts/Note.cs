using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float height;

    [SerializeField] private string noteName;
    [SerializeField] private AudioClip audioClip;

    public bool Collected => collected;
    private bool collected = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("OctaviCopter"))
        {
            collected = true;
            
        }
    }

    public void ResetCollectedStatus()
    {
        // This is called by the Level Manager once it knows the note was hit
        // in case it appears multiple times in a level

        collected = false;
    }

}
