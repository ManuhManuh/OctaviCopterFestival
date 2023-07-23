using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int currentLevelID;

    [SerializeField] public List<Level> levels = new List<Level>();

    public void OnLevelCompleted()
    {
        currentLevelID++;
        if (currentLevelID == levels.Count)
        {
            // There are no more levels
            currentLevelID = -1;
            GameOver();
        }

    }

    public void GameOver()
    {
        // TODO: Update UI with game over content
    }
}
