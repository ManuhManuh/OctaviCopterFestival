using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OctaviCopterVertical : MonoBehaviour
{
    public InputActionReference moveUpReference = null;
    public InputActionReference moveDownReference = null;
    public float speed = 1f;
    // [SerializeField] Material rainbowScaleometer;
    [SerializeField] private float totalNoteStackHeight = 18f;

    private Transform thisTransform = null;
    private float startingHeight;

    // Start is called before the first frame update
    void Awake()
    {
        thisTransform = GetComponent<Transform>();
        startingHeight = thisTransform.position.y;
        
    }

    private void Start()
    {
        /*
        if (rainbowScaleometer == null)
        {
            Debug.Log("Not sure why the rainbow is unfound...");
        }
        else
        {
            rainbowScaleometer.SetFloat("_Height", startingHeight);
        }
        */
    }

    private void Update()
    {
        float upValue = moveUpReference.action.ReadValue<float>();
        MoveOctaviCopterUp(upValue);

        float downValue = moveDownReference.action.ReadValue<float>();
        MoveOctaviCopterDown(downValue);
    }

    private void MoveOctaviCopterUp(float value)
    {
        thisTransform.Translate(0, value * speed * Time.deltaTime, 0);
        // Adjust shader on Rainbow Scalometer to reflect new height
        float height = (float)thisTransform.position.y / (totalNoteStackHeight - startingHeight);
        // rainbowScaleometer.SetFloat("_Height", height);
    }

    private void MoveOctaviCopterDown(float value)
    {
        thisTransform.Translate(0, value * -1 * speed * Time.deltaTime, 0);
        // Adjust shader on Rainbow Scalometer to reflect new height
        float height = (float)thisTransform.position.y / (totalNoteStackHeight - startingHeight);
        // rainbowScaleometer.SetFloat("_Height", height);

    }

}
