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
    private int levelAttempts;
    private int sessionPoints;
    private int lifetimePoints;

    private void Awake()
    {
        currentLevelIndex = 0;
        lifetimePoints = 0;

        if (PlayerPrefs.HasKey("LifetimePoints"))
        {
            lifetimePoints = PlayerPrefs.GetInt("LifetimePoints");
        }
            
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStartPosition = player.transform.position;

        feedback = GameObject.Find("Feedback").GetComponent<TMP_Text>();

        restartAllowed = true;
        PresentFeedback("Press X or A to start!");
        levelAttempts = 1;

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

    public void OnLevelCompleted(bool successful, int maxPoints)
    {
        if(currentLevelManager != null) Destroy(currentLevelManager);
        int pointsEarned;

        if (successful)
        {
            currentLevelIndex++;
            switch (levelAttempts)
            {
                case 1:
                    pointsEarned = maxPoints;
                    break;
                case 2:
                    pointsEarned = maxPoints / 2;
                    break;
                default:
                    pointsEarned = 0;
                    break;
            }

            sessionPoints += pointsEarned;
            lifetimePoints += pointsEarned;

            if (currentLevelIndex == levels.Count)
            {
                // There are no more levels
                currentLevelIndex = -1;
                GameOver();
            }
            else
            {
                PresentFeedback($"Correct! You have earned {pointsEarned} points for completing the level in {levelAttempts} tries! Your session total is {sessionPoints}. Press X or A to start next level");
                restartAllowed = true;
                levelAttempts = 1;

            }
        }
        else
        {
            PresentFeedback("Level failed! Press X or A to retry");
            restartAllowed = true;
            levelAttempts++;

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
        UpdateLifetimePoints(lifetimePoints);
        PresentFeedback($"Game over!! You won!! Your session total was {sessionPoints}, bringing your lifetime points total to {lifetimePoints}");
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

    private void UpdateLevelPoints(int points)
    {

    }

    private void UpdateLifetimePoints(int points)
    {
        PlayerPrefs.SetInt("LifetimePoints", lifetimePoints);
        PlayerPrefs.Save();

    }
}
