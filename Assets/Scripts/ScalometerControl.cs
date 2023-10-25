using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScalometerControl : MonoBehaviour
{
    public float speed = 1f;
    [SerializeField] Material rainbowScaleometer;
    
    private float levelHeight;
    private Transform thisTransform = null;
    private float startingHeight;
    private LevelManager levelManager;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Awake()
    {
        thisTransform = GetComponent<Transform>();
        startingHeight = thisTransform.position.y;
        
    }

    private void Start()
    {
        rainbowScaleometer.SetFloat("_Height", startingHeight);
        gameManager = FindObjectOfType<GameManager>();

    }

    private void Update()
    {

        //if (levelManager == null)
        //{
        //    levelManager = FindObjectOfType<LevelManager>();
        //}
        //else
        //{
        //    if(levelHeight == 0)
        //    {
        //        levelHeight = levelManager.LevelHeight;
        //    }

        //    MoveScalometer();
        //}
        if(gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        else
        {
            if (gameManager.runningTutorial || gameManager.CurrentLevelManager == null)
            {
                levelHeight = 60;
            }
            else
            {
                levelHeight = gameManager.CurrentLevelManager.LevelHeight;
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
