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
        currentLevel = gameManager.CurrentLevel;
        currentNoteIndex = 0;
        GotoState(LevelState.LoadingLevel);

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
        currentLevel = gameManager.CurrentLevel;

        if (currentLevel.randomNoteSpawn)
        {
            RandomNoteSpawn();
        }
        else
        {
            FixedNoteSpawn();
        }

        GotoState(LevelState.LookingForNote);
    }

    private void FixedNoteSpawn()
    {
        float zPosition = 0;

        foreach (Note note in currentLevel.requiredNotes)
        {
            zPosition = zPosition + currentLevel.noteSpacing;
            Vector3 notePosition = new Vector3(currentLevel.xPosition, note.height, zPosition);
            Instantiate(note, notePosition, Quaternion.identity);
        }
    }

    private void RandomNoteSpawn()
    {

        foreach (Note note in currentLevel.requiredNotes)
        {
            float xPosition = RandomPosition(gameManager.arenaWidth);
            float yPosition = Mathf.Abs(RandomPosition(gameManager.arenaHeight));
            float zPosition = RandomPosition(gameManager.arenaDepth);

            Vector3 notePosition = new Vector3(xPosition, yPosition, zPosition);
            Instantiate(note, notePosition, Quaternion.identity);
        }
        
    }

    private float RandomPosition(float fullDimension)
    {
        return Random.Range(fullDimension * -0.5f, fullDimension * 0.5f);
    }

    private void LookingForNoteEntered()
    {
        // Set the target note
        targetNote = currentLevel.requiredNotes[currentNoteIndex];

        // TODO: Update UI with note to be found
        Debug.Log($"Note to find: {targetNote}");
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
        Debug.Log("Correct hit!");
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

    }

    private void ResetLevel()
    {

        

    }

    private void LevelOverEntered()
    {
        // TODO: Update UI with Level Over content
        Debug.Log($"Level {currentLevel.name} over!");
    }
}
