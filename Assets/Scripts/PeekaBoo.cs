using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PeekaBoo : MonoBehaviour
{
    [SerializeField] private float peekDuration = 1.0f;
    [SerializeField] private Material peekaBooMaterial;
    [SerializeField] private InputActionReference leftTriggerReference;
    [SerializeField] private InputActionReference rightTriggerReference;

    private GameManager gameManager;
    private Material originalMaterial;
    private MeshRenderer meshRenderer;
    private bool gripping = false;
    private float gripValue;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;
    }

    private void Update()
    {
        if(gameManager.CurrentLevelManager != null || gameManager.runningTutorial || gameManager.runningControllerTour) //
        {
            float leftTriggerValue = leftTriggerReference.action.ReadValue<float>();
            float rightTriggerValue = rightTriggerReference.action.ReadValue<float>();

            if (leftTriggerValue > 0.1)
            {
                gripValue = leftTriggerValue;
            }
            else if (rightTriggerValue > 0.1)
            {
                gripValue = rightTriggerValue;
            }
            else
            {
                gripValue = 0;
            }

            if ((gripValue > 0.1))
            {
                gripping = true;
                meshRenderer.material = peekaBooMaterial;
                Color newColor = peekaBooMaterial.color;
                newColor.a = Mathf.Clamp(1 - gripValue, 0.25f, 1.0f);
                peekaBooMaterial.color = newColor;
            }
            else if(gripping == true)
            {
                gripping = false;
                meshRenderer.material = originalMaterial;
            }
        }
        
    }

    
}
