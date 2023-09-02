using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PeekaBoo : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> objectsToPeekThrough = new List<MeshRenderer>();
    [SerializeField] private float peekDuration = 1.0f;
    [SerializeField] private Material peekaBooMaterial;
    [SerializeField] private InputActionReference gripReference;

    private GameManager gameManager;
    private bool changeable = true;
    
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        float buttonPressValue = gripReference.action.ReadValue<float>();

        // if ((buttonPressValue > 0) && gameManager.currentLevelManager.CurrentState == LevelManager.LevelState.FlyingTrack)
        if ((buttonPressValue > 0) && changeable)
        {
            changeable = false;
            PeekThrough();
        }
    }

    public void PeekThrough()
    {
        StartCoroutine(MaterialPeek(peekDuration));
    }

    private IEnumerator MaterialPeek(float duration)
    {
        // change materials to PeekaBoo material
        List<Material> oldMaterials = new List<Material>();

        foreach(MeshRenderer obj in objectsToPeekThrough)
        {
            oldMaterials.Add(obj.material);
            obj.material = peekaBooMaterial;
        }
        
        yield return new WaitForSeconds(duration);

        // change material back to original material

        int materialIndex = 0;

        foreach (MeshRenderer obj in objectsToPeekThrough)
        {
            Debug.Log($"Changing material back to {oldMaterials[materialIndex].name}");
            obj.material = oldMaterials[materialIndex];
            materialIndex++;
        }

        changeable = true;
    }
}
