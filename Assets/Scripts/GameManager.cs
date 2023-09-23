using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Level CurrentLevel;
    public LevelManager CurrentLevelManager => currentLevelManager;
    public GameObject NoteCollector => noteCollector;
    public string Mode
    {
        get { return mode; }
        set { mode = value; }
    }

    public Vector3 PlayerStartPosition => playerStartPosition;
    public bool TutorialSuccessful => tutorialSuccessful;
    public bool TutorialCompleted => tutorialCompleted;

    public GameObject player;
    public bool restartRequested = false;
    public bool runningTutorial = false;
    

    [SerializeField] private InputActionReference startLevelActionReference;
    [SerializeField] public List<Level> levels = new List<Level>();
    [SerializeField] private LevelManager levelManagerPrefab;
    [SerializeField] private float timeBetweenLevels;
    [SerializeField] private float timeBeforeRestartAllowed;
    [SerializeField] private GameObject tutorialPrefab;
    [SerializeField] private GameObject backgroundAsset;
    [SerializeField] private ParticleSystem fireworks;
    [SerializeField] private AudioSource endingSong;
    
    private LevelManager currentLevelManager;
    private int currentLevelIndex;
    private Vector3 playerStartPosition;
    private bool restartAllowed;
    private int levelAttempts;
    private int sessionPoints;
    private int lifetimePoints;
    private string mode;
    private UIDisplay uiDisplay;
    public string currentFeedbackText;
    private List<string> pointList;
    private GameObject noteCollector;
    private bool tutorialCompleted = false;
    private bool tutorialSuccessful = false;

    private void Awake()
    {
        lifetimePoints = 0;

        if (PlayerPrefs.HasKey("LifetimePoints"))
        {
            lifetimePoints = PlayerPrefs.GetInt("LifetimePoints");
        }

        backgroundAsset.SetActive(true);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        noteCollector = GameObject.Find("NoteCollector");

        playerStartPosition = player.transform.position;

        InitiateGame();

    }

    public void InitiateGame()
    {

        currentLevelIndex = 0;
        sessionPoints = 0;
        fireworks.gameObject.SetActive(false);
        endingSong.gameObject.SetActive(false);
        uiDisplay = GameObject.FindObjectOfType<UIDisplay>();
        restartAllowed = true;
        currentFeedbackText = "StartPrompt";
        SendMessageToUI();
        levelAttempts = 1;
        runningTutorial = false;

    }

    private void Update()
    {
        if (runningTutorial)
        {
            if (currentFeedbackText != "")
            {
                currentFeedbackText = "";
                uiDisplay.PresentFeedback(currentFeedbackText);
            }
        }
        else
        {
            float buttonPressValue = startLevelActionReference.action.ReadValue<float>();

            if ((buttonPressValue > 0) && restartAllowed)
            {
                currentFeedbackText = "";
                uiDisplay.PresentFeedback(currentFeedbackText);
                StartLevel();

            }
        }
        
    }

    public void RunTutorial()
    {
        runningTutorial = true;
        Instantiate(tutorialPrefab, Vector3.zero, Quaternion.identity);
    }

    public void OnLevelCompleted(bool successful, int maxPoints)
    {
        backgroundAsset.SetActive(true);

        if (currentLevelManager != null) Destroy(currentLevelManager.gameObject);
        int pointsEarned;

        if (successful)
        {
            if (runningTutorial)
            {
                tutorialSuccessful = true;
                tutorialCompleted = true;
                Debug.Log ("Game manager initiating game");
                InitiateGame();

            }
            else
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
                    pointList = new List<string>();
                    pointList.Add(pointsEarned.ToString());
                    pointList.Add(sessionPoints.ToString());
                    currentFeedbackText = "PointsPlusTotal";
                    SendMessageToUI();
                    restartAllowed = true;
                    levelAttempts = 1;

                }
            }
            
        }
        else
        {
            if (runningTutorial)
            {
                tutorialCompleted = true;
                
            }
            else
            {
                if (restartRequested)
                {
                    currentLevelIndex = 0;
                    restartRequested = false;
                    currentFeedbackText = "StartPrompt";
                    restartAllowed = true;
                    sessionPoints = 0;
                }
                else
                {
                    levelAttempts++;
                    currentFeedbackText = "RetryPrompt";
                }

                uiDisplay.PresentFeedback(currentFeedbackText);
                restartAllowed = true;
            }
            

        }

    }

    public void StartLevel()
    {
        backgroundAsset.SetActive(false);  // will be replaced by level environment asset

        restartAllowed = false;
        if (currentLevelManager != null) Destroy(currentLevelManager.gameObject);

        // create a new level
        CurrentLevel = levels[currentLevelIndex];
        currentLevelManager = Instantiate(levelManagerPrefab);

        // reset player
        RelocatePlayer(playerStartPosition);

    }

    public void StartLevel(Level levelToStart)
    {
        backgroundAsset.SetActive(false);  // will be replaced by level environment asset

        restartAllowed = false;
        if (currentLevelManager != null) Destroy(currentLevelManager.gameObject);

        // create a new level
        CurrentLevel = levelToStart;
        currentLevelManager = Instantiate(levelManagerPrefab);

        // reset player
        RelocatePlayer(playerStartPosition);
    }

    
    public void GameOver()
    {

        fireworks.gameObject.SetActive(true);
        endingSong.gameObject.SetActive(true);

        UpdateLifetimePoints(lifetimePoints);
        currentFeedbackText = "GameOver";
        SendMessageToUI();

    }

    public void SendMessageToUI()
    {
        switch (currentFeedbackText)
        {
            case "StartPrompt": 
            case "RetryPrompt":
                {
                    uiDisplay.PresentFeedback(currentFeedbackText);
                    break;
                }
            case "PointsPlusTotal":
                {
                    uiDisplay.PresentFeedback(currentFeedbackText, pointList);
                    break;
                }
            case "GameOver":
                {
                    uiDisplay.PresentFeedback(currentFeedbackText, sessionPoints);
                    break;
                }


        }


    }
    private void RelocatePlayer(Vector3 newPosition)
    {

        player.transform.position = newPosition;

    }

    private void UpdateLifetimePoints(int points)
    {
        PlayerPrefs.SetInt("LifetimePoints", lifetimePoints);
        PlayerPrefs.Save();

    }
}
