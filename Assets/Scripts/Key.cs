using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public Material hintMaterial;
    public Material originalMaterial;
    public bool BeenPlayed => beenPlayed;

    private GameManager gameManager;
    [SerializeField] private bool beenPlayed = false;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    public void Played()
    {
        if(gameManager.runningTutorial) beenPlayed = true;

    }

}
