using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuDisplay : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private TMP_Text modeButtonText;
    [SerializeField] private TMP_Text modeButtonLabel;
    [SerializeField] private TMP_Text languageButtonText;
    [SerializeField] private TMP_Text languageButtonLabel;
    [SerializeField] private TMP_Text instructionsButtonText;
    [SerializeField] private TMP_Text speedLabel;
    [SerializeField] private TMP_Text exitButtonText;

    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private TMP_Text backButtonText;

    [SerializeField] private List<string> modes = new List<string>();
    [SerializeField] private List<string> languages = new List<string>();

    private GameManager gameManager;
    private bool allowChange = true;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

    }
    public void DisplayInstructions()
    {
        // button fires twice, but deactivation preempts the second call
        instructionPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void BackToMain()
    {
        // button fires twice, but deactivation preempts the second call
        mainPanel.SetActive(true);
        instructionPanel.SetActive(false);
    }

    public void CycleMode()
    {
        // button fires twice, so trapping the first call only
        if (allowChange)
        {
            // update the UI
            allowChange = false;
            int currentMode = modes.IndexOf(modeButtonText.text);
            int newMode = (currentMode + 1) % modes.Count;
            modeButtonText.text = modes[newMode];

            // update the value in the Game Manager
            gameManager.Mode = modes[newMode];

            StartCoroutine(ResetChangeFlag());
        }

        
    }

    public void CycleLanguages()
    {
        // button fires twice, so trapping the first call only
        if (allowChange)
        {
            allowChange = false;
            int currentLanguage = languages.IndexOf(languageButtonText.text);
            int newLanguage = (currentLanguage + 1) % languages.Count;
            languageButtonText.text = languages[newLanguage];
            StartCoroutine(ResetChangeFlag());
        }


    }

    private IEnumerator ResetChangeFlag()
    {
        // delay just long enough that the second button call doens't get acted on
        yield return new WaitForEndOfFrame();
        allowChange = true;
    }

    public void LocalizeSelections()
    {

    }

    public void UpdateSpeed()
    {

    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log($"Game will exit in build");
    }


}

