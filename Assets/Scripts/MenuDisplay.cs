using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class MenuDisplay : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private TMP_Text modeButtonText;
    [SerializeField] private TMP_Text languageButtonText;
    [SerializeField] private Slider speedSlider;

    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private TMP_Text backButtonText;

    [SerializeField] private List<string> modes = new List<string>();
    [SerializeField] private List<string> languages = new List<string>();
    [SerializeField] private List<TMP_Text> textFields = new List<TMP_Text>();

    [SerializeField] private float minSpeedBeginner;
    [SerializeField] private float maxSpeedBeginner;
    [SerializeField] private float minSpeedAdvanced;
    [SerializeField] private float maxSpeedAdvanced;
    [SerializeField] private TMP_Text speedSliderLeft;
    [SerializeField] private TMP_Text speedsliderRight;

    private GameManager gameManager;
    private bool localizationInitialized = false;
    private ActionBasedContinuousMoveProvider moveProvider;
    private float currentMinSpeed;
    private float currentMaxSpeed;
    

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if(PlayerPrefs.GetInt("LastLanguageUsed") != (int)Locale.en)
        {
            LocalizeSelections();
        }
        else
        {
            gameManager.Mode = "Beginner";
        }

        moveProvider = FindObjectOfType<ActionBasedContinuousMoveProvider>();
        UpdateSpeedRanges();
        UpdateSpeedSlider();

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
        UpdateSpeedRanges();
        UpdateSpeedSlider();
    }

    public void CycleMode()
    {

        int currentMode = modes.IndexOf(modeButtonText.text);
        int newMode = (currentMode + 1) % modes.Count;
        modeButtonText.text = modes[newMode];

        // update the value in the Game Manager
        gameManager.Mode = modes[newMode];

        // update the min and max speed values
        UpdateSpeedRanges();
        UpdateSpeedSlider();

    }

    

    public void CycleLanguages()
    {
        int currentLanguage = languages.IndexOf(languageButtonText.text);
        int newLanguage = (currentLanguage + 1) % languages.Count;
        languageButtonText.text = languages[newLanguage];
        LocalizeSelections();

    }

    public void LocalizeSelections()
    {
        Locale oldLocale = Localization.currentLocale;
        Locale newLocale = localizationInitialized? Localization.FindLocaleFromButtonText(languageButtonText.text): (Locale)PlayerPrefs.GetInt("LastLanguageUsed");

        Localization.currentLocale = newLocale;
        PlayerPrefs.SetInt("LastLanguageUsed",(int)newLocale);

        // update mode selection 
        string modeKey = Localization.FindKeyFromValue(oldLocale, modeButtonText.text);
        Localization.currentLocalizationTable.TryGetValue(modeKey, out string newMode);

        if (modeKey != null)
        {
            modeButtonText.text = newMode;
            gameManager.Mode = newMode;
        }

        // update mode list
        for (int i = 0; i < modes.Count; i++)
        {
            string textKey = Localization.FindKeyFromValue(oldLocale, modes[i]);
            Localization.currentLocalizationTable.TryGetValue(textKey, out string newModeText);
 
            if(newModeText != null)
            {
                modes[i] = newModeText;
            }
            
        }

        // start the language selector on the right language
        if (!localizationInitialized)
        {
            Localization.currentLocalizationTable.TryGetValue("LanguageSelected", out string newLanguage);
            languageButtonText.text = newLanguage;

        }

        // update list of text fields
        for (int i = 0; i < textFields.Count; i++)
        {
            string textKey = Localization.FindKeyFromValue(oldLocale, textFields[i].text);
            Localization.currentLocalizationTable.TryGetValue(textKey, out string newText);
            if (newText != null)
            {
                textFields[i].text = newText;
            }

        }

        

        gameManager.SendMessageToUI();

        if (gameManager.CurrentLevelManager != null)    // level has started
        {
            gameManager.CurrentLevelManager.UpdateLevelUIFields();
            gameManager.CurrentLevelManager.SendMessageToUI();
        }

        localizationInitialized = true;

    }

    private void UpdateSpeedRanges()
    {
        // update the min and max speed values
        string modeKey = Localization.FindKeyFromValue(Localization.currentLocale, gameManager.Mode);
        switch (modeKey)
        {
            case "ModeBeginner":
                {
                    speedSliderLeft.text = minSpeedBeginner.ToString();
                    speedsliderRight.text = maxSpeedBeginner.ToString();
                    currentMinSpeed = minSpeedBeginner;
                    currentMaxSpeed = maxSpeedBeginner;
                    break;
                }
            case "ModeAdvanced":
                {
                    speedSliderLeft.text = minSpeedAdvanced.ToString();
                    speedsliderRight.text = maxSpeedAdvanced.ToString();
                    currentMinSpeed = minSpeedAdvanced;
                    currentMaxSpeed = maxSpeedAdvanced;
                    break;
                }

        }

    }

    public void UpdateSpeed()
    {
        float newSpeed = speedSlider.value * (currentMaxSpeed - currentMinSpeed) + currentMinSpeed;
        moveProvider.moveSpeed = newSpeed;

    }

    private void UpdateSpeedSlider()
    {
        float newSliderValue;
        float speedRange;
        float speedPortion;

        speedRange = currentMaxSpeed - currentMinSpeed;
        speedPortion = moveProvider.moveSpeed - currentMinSpeed;
        if (speedRange == 0)    // will only happen if max and min speed are the same
        {
            newSliderValue = 0;
        }
        else
        {
            newSliderValue = speedPortion / speedRange;
        }
 
        speedSlider.value = Mathf.Clamp(newSliderValue,0,1);

    }

    public void RestartGame()
    {
        if(gameManager.CurrentLevelManager == null)
        {
            gameManager.InitiateGame();
        }
        else
        {
            gameManager.restartRequested = true;
            gameManager.CurrentLevelManager.GotoState(LevelManager.LevelState.EvaluatingLevel);
        }
        
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log($"Game will exit in build");
    }


}

