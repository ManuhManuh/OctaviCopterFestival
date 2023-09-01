using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System;

public class UIDisplay : MonoBehaviour
{
    
    [SerializeField] private TMP_Text levelTitle;
    [SerializeField] private TMP_Text levelInstructions;
    [SerializeField] private TMP_Text feedback;

    public void UpdateLevelTitle(string newTitleKey)
    {
        if (newTitleKey == "")
        {
            levelTitle.text = "";
        }
        else
        {
            if (Localization.currentLocalizationTable.ContainsKey(newTitleKey))
            {
                string newTitle = Localization.currentLocalizationTable[newTitleKey];
                levelTitle.text = (newTitle);
            }
            else
            {
                Debug.Log($"{newTitleKey} not found as a title");
            }
        }
    }

    public void UpdateLevelInstructions(string newInstructionsKey)
    {
        if (newInstructionsKey == "")
        {
            levelInstructions.text = "";
        }
        else
        {
            if (Localization.currentLocalizationTable.ContainsKey(newInstructionsKey))
            {
                string newTitle = Localization.currentLocalizationTable[newInstructionsKey];
                levelInstructions.text = (newTitle);
            }
            else
            {
                Debug.Log($"{newInstructionsKey} not found as an instruction key");
            }
        }
    }

    public void PresentFeedback(string messageKey)
    {
        if(messageKey == "")
        {
            feedback.text = "";
        }
        else
        {
            if (Localization.currentLocalizationTable.ContainsKey(messageKey))
            {
                string message = Localization.currentLocalizationTable[messageKey];
                feedback.text = (message);
            }
            else
            {
                Debug.Log($"{messageKey} not found as a key");
            }
        }
        
    }

    public void PresentFeedback(string messageKey, int insertNumber)
    {
        if (Localization.currentLocalizationTable.ContainsKey(messageKey))
        {
            string storedMessage = Localization.currentLocalizationTable[messageKey];
            string insertText = insertNumber.ToString();
            string message = InsertValue(storedMessage, insertText);
            feedback.text = (message);
        }
        else
        {
            Debug.Log($"{messageKey} not found as a key");
        }
    }

    public void PresentFeedback(string messageKey, string insertText)
    {
        if (Localization.currentLocalizationTable.ContainsKey(messageKey))
        {
            string storedMessage = Localization.currentLocalizationTable[messageKey];
            string message = InsertValue(storedMessage, insertText);
            feedback.text = (message);
        }
        else
        {
            Debug.Log($"{messageKey} not found as a key");
        }
    }

    public void PresentFeedback(string messageKey, List<string> insertText)
    {
        if (Localization.currentLocalizationTable.ContainsKey(messageKey))
        {
            string storedMessage = Localization.currentLocalizationTable[messageKey];
            string message = InsertValues(storedMessage, insertText);
            feedback.text = (message);
        }
        else
        {
            Debug.Log($"{messageKey} not found as a key");
        }
    }

    private string InsertValue(string originalMessage, string valueToInsert)
    {
        // curly braces are the indicators of where to insert the value
        StringBuilder newMessage = new StringBuilder();
        bool rightBraceFound = false;
        bool leftBraceFound = false;
        

        foreach(char c in originalMessage)
        {
            if(!rightBraceFound)
            {
                if (c.Equals('{'))
                {
                    rightBraceFound = true;
                    newMessage.Append(valueToInsert);
                }
                else
                {
                    newMessage.Append(c);
                }
            }
            else if(!leftBraceFound)
            {
                if (c.Equals('}'))
                {
                    leftBraceFound = true;
                }
            }
            else
            {
                newMessage.Append(c);
            }
        }

        return newMessage.ToString();
    }

    private string InsertValues(string originalMessage, List<string> valuesToInsert)
    {
        // curly braces are the indicators of where to insert the value
        StringBuilder newMessage = new StringBuilder();
        bool leftBraceFound = false;
        int valueIndex;

        foreach(char c in originalMessage)
        {
            if (!leftBraceFound)
            {
                if (c.Equals('{'))
                {
                    leftBraceFound = true;
                }
                else
                {
                    newMessage.Append(c);
                }
            }
            else
            {
                if (c.Equals('}'))
                {
                    leftBraceFound = false;
                }
                else
                {
                    valueIndex = Int32.Parse(c.ToString());
                    if(valueIndex < valuesToInsert.Count)
                    {
                        newMessage.Append(valuesToInsert[valueIndex]);
                    }
                        
                }
            }
 
        }

        return newMessage.ToString();
    }


}
