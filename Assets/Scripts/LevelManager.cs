using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    
    public enum LevelState
    {
        None,
        LoadingLevel,
        LookingForNote,
        EvaluatingLevel,
        AllNotesFound,
        LevelOver
    }

    private LevelState currentState;
    private Level currentLevel;
    private int currentNoteIndex = 0;
    private Note targetNote;
    private GameManager gameManager;

    private void Start()
    {
        // Get reference to game manager
        gameManager = FindObjectOfType<GameManager>();

        // Set the initial state and starting values of flags and indexes
        ResetLevel();
    }

    private void OnStateEntered(LevelState state)
    {
        switch (state)
        {
            case LevelState.None:
                {
                    break;
                }
            case LevelState.LoadingLevel:
                {
                    LoadingLevelEntered();
                    break;
                }
            case LevelState.LookingForNote:
                {
                    LookingForNoteEntered();
                    break;
                }
            case LevelState.EvaluatingLevel:
                {
                    EvaluatingLevelEntered();
                    break;
                }
            case LevelState.AllNotesFound:
                {
                    AllNotesFoundEntered();
                    break;
                }
            case LevelState.LevelOver:
                {
                    LevelOverEntered();
                    break;
                }
        }
    }

    private void OnStateLeft(LevelState state)
    {
        switch (state)
        {
            case LevelState.None:
                {
                    break;
                }
            case LevelState.LoadingLevel:
                {
                    break;
                }
            case LevelState.LookingForNote:
                {
                    currentNoteIndex++;
                    break;
                }
            case LevelState.EvaluatingLevel:
                {
                    break;
                }
            case LevelState.AllNotesFound:
                {
                    break;
                }
        }
    }

    private void GotoState(LevelState newState)
    {
        OnStateLeft(currentState);
        currentState = newState;
        OnStateEntered(currentState);
    }

    public void Update()
    {
        if(currentState == LevelState.LookingForNote)
        {
            LookingForNoteUpdate();
        }
        
    }

    private void LoadingLevelEntered()
    {

        // get a reference to the SO for the current level
        currentLevel = gameManager.levels[gameManager.currentLevelID];
        foreach(Note note in currentLevel.requiredNotes)
        {
            Instantiate(note, GetRandomPosition(), Quaternion.identity);
        }

        GotoState(LevelState.LookingForNote);
    }

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(currentLevel.minXNotePosition, currentLevel.maxXNotePosition);
        float y = Random.Range(currentLevel.minYNotePosition, currentLevel.maxYNotePosition);
        float z = Random.Range(currentLevel.minZNotePosition, currentLevel.maxZNotePosition);

        return new Vector3(x, y, z);
    }

    private void LookingForNoteEntered()
    {
        // Set the target note
        targetNote = currentLevel.requiredNotes[currentNoteIndex];

        // TODO: Update UI with note to be found
    }

    
    private void LookingForNoteUpdate()
    {
        
        // check to see if the next note we need has been hit
        if (targetNote.Collected)
        {
            // Reset its hit flag in case it is used multiple times in a level
            targetNote.ResetCollectedStatus();
            GotoState(LevelState.EvaluatingLevel);
        }

    }

    private void LookingForNoteLeft()
    {
        // TODO: Update UI to show correct hit
    }

    private void EvaluatingLevelEntered()
    {
        // check to see if we are done the level
        if (currentNoteIndex == currentLevel.requiredNotes.Length)
        {
            GotoState(LevelState.AllNotesFound);
        }
        else
        {
            currentNoteIndex++;
            GotoState(LevelState.LookingForNote);
        }
    }

    private void AllNotesFoundEntered()
    {

        // let gameManager know that level is complete
        gameManager.OnLevelCompleted();

        // see if we need to do another level
        if (gameManager.currentLevelID != -1)
        {
            ResetLevel();
        }
    }

    private void ResetLevel()
    {
        currentNoteIndex = 0;
        GotoState(LevelState.LoadingLevel);

    }

    private void LevelOverEntered()
    {
        // TODO: Update UI with Level Over content
    }
}
