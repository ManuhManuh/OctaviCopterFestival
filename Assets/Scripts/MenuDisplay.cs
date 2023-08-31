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

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

    }
    public void DisplayInstructions()
    {
 
        instructionPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void BackToMain()
    {
  
        mainPanel.SetActive(true);
        instructionPanel.SetActive(false);
    }

    public void CycleMode()
    {

        int currentMode = modes.IndexOf(modeButtonText.text);
        int newMode = (currentMode + 1) % modes.Count;
        modeButtonText.text = modes[newMode];

        // update the value in the Game Manager
        gameManager.Mode = modes[newMode];

    }

    public void CycleLanguages()
    {
            int currentLanguage = languages.IndexOf(languageButtonText.text);
            int newLanguage = (currentLanguage + 1) % languages.Count;
            languageButtonText.text = languages[newLanguage];

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

