using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    
    public enum LevelState
    {
        None,
        LoadingLevel,
        FlyingTrack,
        EvaluatingLevel
      
    }

    public float LevelHeight => levelHeight;
    private float levelHeight;

    private GameManager gameManager;
    private LevelState currentState;
    private Level currentLevel;
    private Vector3 playerStartPosition;
    
    private Dictionary<Note, bool> levelNotes = new Dictionary<Note, bool>();
    private int correctNotesCollected;
    private int notesCollected;
    private int notesPerTrack;

    private TMP_Text levelTitle;
    private TMP_Text levelInstructions;
    private TMP_Text feedback;

    private void Start()
    {
        // Get reference to game manager
        gameManager = FindObjectOfType<GameManager>();

        // Set the initial state and starting values of flags and indexes
        currentLevel = gameManager.CurrentLevel;

        levelTitle = GameObject.Find("LevelTitle").GetComponent<TMP_Text>();
        levelInstructions = GameObject.Find("LevelInstructions").GetComponent<TMP_Text>();
        feedback = GameObject.Find("Feedback").GetComponent<TMP_Text>();

        if (currentLevel.environmentAsset != null)
        {
            Instantiate(currentLevel.environmentAsset);
        }

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

            case LevelState.FlyingTrack:
                {
                    PresentFeedback($"Start flying!");
                    break;
                }
            case LevelState.EvaluatingLevel:
                {
                    EvaluatingLevelEntered();
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

            case LevelState.FlyingTrack:
                {
                    break;
                }
            case LevelState.EvaluatingLevel:
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

    private void LoadingLevelEntered()
    {
        // make sure we're starting with a clean slate
        InitializeLevel();

        // collect a list of the tracks in the level
        float playerPos = gameManager.PlayerStartPosition.x;
        float startPosition = currentLevel.tracks.Length % 2 == 0 ?
                                playerPos - (currentLevel.trackSpacing * (currentLevel.tracks.Length / 2)) - (currentLevel.trackSpacing / 2) :
                                playerPos - (currentLevel.trackSpacing * (currentLevel.tracks.Length / 2));

        List<Track> createTracks = new List<Track>();

        for(int i = 0; i < currentLevel.tracks.Length; i++)
        {
            createTracks.Add(currentLevel.tracks[i]);
        }

        float currentTrackPosition = startPosition;

        // instantiate the tracks for the level
        for (int i = 0; i < currentLevel.tracks.Length; i++)
        {
            int randomTrack = Random.Range(0, createTracks.Count - 1);

            notesPerTrack = 0;  // will keep the value found for the last track (all should be the same count)
            InstantiateTrack(createTracks[randomTrack], currentTrackPosition);

            createTracks.RemoveAt(randomTrack);
            currentTrackPosition += currentLevel.trackSpacing;

        }

        //levelHeight = currentLevel.maxHeight;
        //levelTitle.text = currentLevel.name;
        //levelInstructions.text = currentLevel.instructions;

        // Give player note clue
        StartCoroutine(PerformTrackHint());

    }

    private void InitializeLevel()
    {
        // reset all control variables
        levelNotes = new Dictionary<Note, bool>();
        correctNotesCollected = 0;
        notesCollected = 0;

        levelHeight = currentLevel.maxHeight;
        levelTitle.text = currentLevel.name;
        levelInstructions.text = currentLevel.instructions;

    }

    private void InstantiateTrack(Track track, float trackXPosition)
    {
        float yPosition;
        float zPosition = currentLevel.trackStartDistance - currentLevel.noteSpacing;
        bool needToCollect = track.name == currentLevel.winningTrack.name;

        foreach(Note note in track.notes)
        {
            yPosition = note.height;
            zPosition += currentLevel.noteSpacing;
            Vector3 notePosition = new Vector3(trackXPosition, yPosition, zPosition);

            Note newNote = Instantiate(note, notePosition, Quaternion.identity);

            // collect and subscribe to the note
            levelNotes.Add(newNote, needToCollect);
            newNote.OnNoteCollected += CheckNote;

        }

        notesPerTrack = track.notes.Length;

    }

    private void CheckNote(Note hitNote)
    {
        notesCollected++;
        PresentFeedback($"You hit {hitNote.noteName}");

        if (levelNotes[hitNote])
        {
            correctNotesCollected++;
        }

        // if we've collected our quota of notes
        if (notesCollected == notesPerTrack)
        {
            GotoState(LevelState.EvaluatingLevel);
        }

    }

    private void EvaluatingLevelEntered()
    {
        StartCoroutine(CleanUpAndEndLevel());

    }


    private IEnumerator PerformTrackHint()
    {
        
        yield return new WaitForSeconds(1); 

        PresentFeedback($"Giving {currentLevel.name} hint");
        List<Note> playNotes = new List<Note>();

        foreach (Note note in levelNotes.Keys)
        {
            // if the Value of the note (needs to be collected) is true 
            if (levelNotes[note]) playNotes.Add(note);

        }

        foreach(Note note in playNotes)
        {
            note.audioSource.Play();
            // if visual clues become a thing (i.e., keyboard keys light up) it can go here too
            yield return new WaitForSeconds(currentLevel.clueTiming);
        }

        if(currentState == LevelState.LoadingLevel) GotoState(LevelState.FlyingTrack);
    }

    private void PresentFeedback(string message)
    {
        feedback.text = (message);
        Debug.Log(message);
    }

    private IEnumerator CleanUpAndEndLevel()
    {
        yield return new WaitForSeconds(3); // time for sound from last note collected to decay
        // destroy the notes, thereby also removing the subscriptions to them ;)
        Note[] notes = FindObjectsOfType<Note>();
        foreach(Note note in notes)
        {
            Destroy(note.gameObject);
        }

        yield return new WaitForSeconds(1);

        // pass whether we collected the correct notes
        gameManager.OnLevelCompleted(correctNotesCollected == notesPerTrack);

    }
}
