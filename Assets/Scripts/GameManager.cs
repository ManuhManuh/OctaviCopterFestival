using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Level CurrentLevel => levels[currentLevelIndex];
    public Vector3 PlayerStartPosition => playerStartPosition;

    [SerializeField] private InputActionReference startLevelActionReference;
    [SerializeField] public List<Level> levels = new List<Level>();
    [SerializeField] private LevelManager levelManagerPrefab;
    [SerializeField] private float timeBetweenLevels;
    [SerializeField] private float timeBeforeRestartAllowed;
    [SerializeField] GameObject player;

    private LevelManager currentLevelManager;
    private int currentLevelIndex;
    private Vector3 playerStartPosition;
    private TMP_Text feedback;
    private bool restartAllowed;

    private void Awake()
    {
        currentLevelIndex = 0;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStartPosition = player.transform.position;

        feedback = GameObject.Find("Feedback").GetComponent<TMP_Text>();

        restartAllowed = true;
        PresentFeedback("Press X or A to start!");

    }

    private void Update()
    {
        float buttonPressValue = startLevelActionReference.action.ReadValue<float>();
   
        if ((buttonPressValue > 0) && restartAllowed)
        {
            PresentFeedback("");
            StartLevel();
            
        }
    }

    public void OnLevelCompleted(bool successful)
    {
        if(currentLevelManager != null) Destroy(currentLevelManager);

        Note[] notes = FindObjectsOfType<Note>();
        if(notes.Length == 0)
        {
            if (successful)
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
                    restartAllowed = true;
                }
            }
            else
            {
                PresentFeedback("Level failed! Press X or A to retry");
                restartAllowed = true;
            }
        }
        else
        {
            PresentFeedback($"There are still {notes.Length} notes in the scene - not ready for next level yet");

        }

    }

    public void StartLevel()
    {
        restartAllowed = false;

        if (currentLevelManager != null) Destroy(currentLevelManager);

        // create a new level
        currentLevelManager = Instantiate(levelManagerPrefab);

        // reset player
        RelocatePlayer(playerStartPosition);

        // pause before allowing a reset
        // StartCoroutine(MinimumWaitForRestart(timeBeforeRestartAllowed));
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

    private void RelocatePlayer(Vector3 newPosition)
    {
        // this may need to be a fade in/out vignette if it is too disorienting to just teleport back to the beginning

        player.transform.position = newPosition;

    }
}
