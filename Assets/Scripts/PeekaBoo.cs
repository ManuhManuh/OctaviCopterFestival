using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PeekaBoo : MonoBehaviour
{
    [SerializeField] private float peekDuration = 1.0f;
    [SerializeField] private Material peekaBooMaterial;
    [SerializeField] private InputActionReference leftGripReference;
    [SerializeField] private InputActionReference rightGripReference;

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
        if(gameManager.CurrentLevelManager != null)
        {
            float leftGripValue = leftGripReference.action.ReadValue<float>();
            float rightGripValue = rightGripReference.action.ReadValue<float>();

            if (leftGripValue > 0.1)
            {
                gripValue = leftGripValue;
            }
            else if (rightGripValue > 0.1)
            {
                gripValue = rightGripValue;
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
