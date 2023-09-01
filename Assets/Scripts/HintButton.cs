using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintButton : MonoBehaviour
{
    [SerializeField] Animation buttonPush;
    [SerializeField] AudioSource clickFX;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();

        }

        if (gameManager.currentLevelManager != null)    // level has started
        {
            if (!gameManager.currentLevelManager.HintIsPlaying)
            {
                buttonPush.Play();
                clickFX.Play();
                StartCoroutine(gameManager.currentLevelManager.PerformTrackHint());
            }
            
        }

    }
}
