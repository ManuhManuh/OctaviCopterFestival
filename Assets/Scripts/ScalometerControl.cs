using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScalometerControl : MonoBehaviour
{
    public float speed = 1f;
    [SerializeField] Material rainbowScaleometer;
    [SerializeField] private float levelHeight;

    private Transform thisTransform = null;
    private float startingHeight;
    private LevelManager levelManager;

    // Start is called before the first frame update
    void Awake()
    {
        thisTransform = GetComponent<Transform>();
        startingHeight = thisTransform.position.y;
        
    }

    private void Start()
    {
        rainbowScaleometer.SetFloat("_Height", startingHeight);

    }

    private void Update()
    {
        if (levelManager == null)
        {
            levelManager = FindObjectOfType<LevelManager>();
        }
        else
        {
            if(levelHeight == 0)
            {
                levelHeight = levelManager.LevelHeight;
            }

            MoveScalometer();
        }
        

    }

    private void MoveScalometer()
    {
        float meterHeight = (float)thisTransform.position.y / (levelHeight - startingHeight);
        rainbowScaleometer.SetFloat("_Height", meterHeight);

    }


}
