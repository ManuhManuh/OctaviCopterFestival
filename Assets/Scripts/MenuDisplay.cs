using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDisplay : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject instructionPanel;

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
}
