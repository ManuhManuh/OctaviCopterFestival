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
    [SerializeField] private ParticleSystem fireworks;
    
    public LevelManager currentLevelManager;
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
    private void Awake()
    {
        currentLevelIndex = 0;
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
        playerStartPosition = player.transform.position;

        uiDisplay = GameObject.FindObjectOfType<UIDisplay>();
        restartAllowed = true;
        currentFeedbackText = "StartPrompt";
        SendMessageToUI();
        levelAttempts = 1;

    }

    private void Update()
    {
        float buttonPressValue = startLevelActionReference.action.ReadValue<float>();
   
        if ((buttonPressValue > 0) && restartAllowed)
        {
            currentFeedbackText = "";
            uiDisplay.PresentFeedback(currentFeedbackText);
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
                pointList = new List<string>();
                pointList.Add(pointsEarned.ToString());
                pointList.Add(sessionPoints.ToString());
                currentFeedbackText = "PointsPlusTotal";
                SendMessageToUI();
                restartAllowed = true;
                levelAttempts = 1;

            }
        }
        else
        {
            currentFeedbackText = "RetryPrompt";
            uiDisplay.PresentFeedback(currentFeedbackText);
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

        fireworks.gameObject.SetActive(true);
        UpdateLifetimePoints(lifetimePoints);
        currentFeedbackText = "GameOver";
        SendMessageToUI();

    }

    private IEnumerator MinimumWaitForRestart(float restartDelay)
    {
        yield return new WaitForSeconds(restartDelay);
        restartAllowed = true;

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

    private void UpdateLevelPoints(int points)
    {

    }

    private void UpdateLifetimePoints(int points)
    {
        PlayerPrefs.SetInt("LifetimePoints", lifetimePoints);
        PlayerPrefs.Save();

    }
}
