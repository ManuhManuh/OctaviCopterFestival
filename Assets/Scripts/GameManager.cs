using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public InputActionReference startLevelActionReference = null;
    public Level CurrentLevel => levels[currentLevelIndex];
    public Transform PlayerStartPosition => playerStartPosition;

    [SerializeField] public List<Level> levels = new List<Level>();
    [SerializeField] private LevelManager levelManagerPrefab;
    [SerializeField] private float timeBetweenLevels;
    [SerializeField] private float timeBeforeRestartAllowed;
    
    private LevelManager currentLevelManager;
    private int currentLevelIndex;
    private Transform playerStartPosition;
    private Text feedback;
    private bool restartAllowed;

    private void Awake()
    {
        currentLevelIndex = 0;
    }

    private void Start()
    {
        playerStartPosition = GameObject.FindGameObjectWithTag("Player").transform;
        feedback = GameObject.Find("Feedback").GetComponent<Text>();
        StartLevel();
    }

    private void Update()
    {
        float buttonPressValue = startLevelActionReference.action.ReadValue<float>();

        if ((buttonPressValue > 0) && restartAllowed)
        {
            StartLevel();

        }
        else
        {
            Debug.Log($"Button value: {buttonPressValue}");
        }
    }

    public void OnLevelCompleted(bool successful)
    {
     //   if(currentLevelManager != null) Destroy(currentLevelManager);

        if(successful)
        {
            currentLevelIndex++;
            if (currentLevelIndex == levels.Count)
            {
                // There are no more levels
                currentLevelIndex = -1;
                GameOver();
            }
            else
            {
                PresentFeedback("Level completed successfully! Press X or A to start next level");
            }
        }
        else
        {
            PresentFeedback("Level failed! Press X or A to retry");
        }


    }

    public void StartLevel()
    {
        if (currentLevelManager != null) Destroy(currentLevelManager);

        // create a new level
        currentLevelManager = Instantiate(levelManagerPrefab);
        PresentFeedback($"Instantiating level manager for {levels[currentLevelIndex].name}");
        restartAllowed = false;
        StartCoroutine(MinimumWaitForRestart(timeBeforeRestartAllowed));
    }

    
    public void GameOver()
    {
        // TODO: Update UI with game over content
        PresentFeedback("Game over!! You won!!");
    }

    private void PresentFeedback(string message)
    {
        feedback.text = (message);
        Debug.Log(message);
    }

    private IEnumerator MinimumWaitForRestart(float restartDelay)
    {
        yield return new WaitForSeconds(restartDelay);
        restartAllowed = true;

    }
}
