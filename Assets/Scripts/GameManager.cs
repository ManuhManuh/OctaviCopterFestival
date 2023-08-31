using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Level CurrentLevel => levels[currentLevelIndex];
    public string Mode
    {
        get { return mode; }
        set { mode = value; }
    }

    public Vector3 PlayerStartPosition => playerStartPosition;
    public GameObject player;

    [SerializeField] private InputActionReference startLevelActionReference;
    [SerializeField] public List<Level> levels = new List<Level>();
    [SerializeField] private LevelManager levelManagerPrefab;
    [SerializeField] private float timeBetweenLevels;
    [SerializeField] private float timeBeforeRestartAllowed;
    [SerializeField] private GameObject backgroundAsset;
    
    private LevelManager currentLevelManager;
    private int currentLevelIndex;
    private Vector3 playerStartPosition;
    private bool restartAllowed;
    private int levelAttempts;
    private int sessionPoints;
    private int lifetimePoints;
    private string mode = "Beginner";
    private UIDisplay uiDisplay;

    private void Awake()
    {
        currentLevelIndex = 0;
        lifetimePoints = 0;

        if (PlayerPrefs.HasKey("LifetimePoints"))
        {
            lifetimePoints = PlayerPrefs.GetInt("LifetimePoints");
        }
        if (PlayerPrefs.HasKey("LastLanguage"))
        {
            lifetimePoints = PlayerPrefs.GetInt("LastLanguage");
        }

        backgroundAsset.SetActive(true);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStartPosition = player.transform.position;

        uiDisplay = GameObject.FindObjectOfType<UIDisplay>();
        restartAllowed = true;
        uiDisplay.PresentFeedback("StartPrompt");
        levelAttempts = 1;

    }

    private void Update()
    {
        float buttonPressValue = startLevelActionReference.action.ReadValue<float>();
   
        if ((buttonPressValue > 0) && restartAllowed)
        {
            uiDisplay.PresentFeedback("");
            StartLevel();
            
        }
    }

    public void OnLevelCompleted(bool successful, int maxPoints)
    {
        backgroundAsset.SetActive(true);

        if (currentLevelManager != null) Destroy(currentLevelManager);
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
                List<string> pointList = new List<string>();
                pointList.Add(pointsEarned.ToString());
                pointList.Add(sessionPoints.ToString());

                uiDisplay.PresentFeedback("PointsPlusTotal", pointList);
                restartAllowed = true;
                levelAttempts = 1;

            }
        }
        else
        {
            uiDisplay.PresentFeedback("RetryPrompt");
            restartAllowed = true;
            levelAttempts++;

        }

    }

    public void StartLevel()
    {
        backgroundAsset.SetActive(false);  // will be replaced by level environment asset

        restartAllowed = false;
        if (currentLevelManager != null) Destroy(currentLevelManager);

        // create a new level
        currentLevelManager = Instantiate(levelManagerPrefab);

        // reset player
        RelocatePlayer(playerStartPosition);

    }

    
    public void GameOver()
    {
     
        UpdateLifetimePoints(lifetimePoints);
        uiDisplay.PresentFeedback("GameOver", sessionPoints);
    }

    private IEnumerator MinimumWaitForRestart(float restartDelay)
    {
        yield return new WaitForSeconds(restartDelay);
        restartAllowed = true;

    }

    private void RelocatePlayer(Vector3 newPosition)
    {

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
