using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public Material hintMaterial;
    public Material originalMaterial;
    public bool BeenPlayed => beenPlayed;

    private bool beenPlayed = false;

    public void Played()
    {
        beenPlayed = true;
    }

}
