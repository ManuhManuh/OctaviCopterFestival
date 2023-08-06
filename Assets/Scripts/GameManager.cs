using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Level CurrentLevel => levels[currentLevelIndex];

    [SerializeField] public List<Level> levels = new List<Level>();
    [SerializeField] private LevelManager levelManagerPrefab;
    
    private LevelManager currentLevelManager;
    private int currentLevelIndex;


    private void Awake()
    {
        currentLevelIndex = -1;
    }

    private void Start()
    {
        OnLevelCompleted();
    }
    public void OnLevelCompleted()
    {
        if(currentLevelManager != null) Destroy(currentLevelManager);

        currentLevelIndex++;
        if (currentLevelIndex == levels.Count)
        {
            // There are no more levels
            currentLevelIndex = -1;
            GameOver();
        }
        else
        {
            // create a new level
            currentLevelManager = Instantiate(levelManagerPrefab);
            Debug.Log($"Instantiating level manager for {levels[currentLevelIndex].name}");
        }

    }

    public void GameOver()
    {
        // TODO: Update UI with game over content
        Debug.Log("Game over!!");
    }
}
