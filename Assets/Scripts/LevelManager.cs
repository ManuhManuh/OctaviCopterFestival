using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    
    public enum LevelState
    {
        None,
        LoadingLevel,
        FlyingTrack,
        EvaluatingLevel,
        LevelOver
    }

    private GameManager gameManager;

    private LevelState currentState;
    private Level currentLevel;
    private GameObject player;
    private Text feedback;
    private List<Note> selectedTrackNotes = new List<Note>();
    private List<Note> winningTrackNotes = new List<Note>();
    private int notesCollected;

    private void Start()
    {
        // Get reference to game manager
        gameManager = FindObjectOfType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");

        // Set the initial state and starting values of flags and indexes
        currentLevel = gameManager.CurrentLevel;
        feedback = GameObject.Find("Feedback").GetComponent<Text>();

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
                    break;
                }
            case LevelState.EvaluatingLevel:
                {
                    EvaluatingLevelEntered();
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

        // get a reference to the SO for the current level
        currentLevel = gameManager.CurrentLevel;
        feedback.text = ($"Starting {currentLevel.name}!");

        // collect a list of the tracks in the level
        float playerPos = player.transform.position.x;
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

            InstantiateTrack(createTracks[randomTrack], currentTrackPosition);

            createTracks.RemoveAt(randomTrack);
            currentTrackPosition += currentLevel.trackSpacing;

        }

        // Give player note clue

        StartCoroutine(PerformTrackHint());

        
    }

    private void InstantiateTrack(Track track, float trackXPosition)
    {
        float yPosition;
        float zPosition = currentLevel.trackStartDistance - currentLevel.noteSpacing;
        bool isWinningTrack = track.name == currentLevel.winningTrack.name;

        foreach(Note note in track.notes)
        {
            yPosition = note.height;
            zPosition += currentLevel.noteSpacing;
            Vector3 notePosition = new Vector3(trackXPosition, yPosition, zPosition);

            Note newNote = Instantiate(note, notePosition, Quaternion.identity);
            if (isWinningTrack)
            {
                // collect and subscribe to the note
                winningTrackNotes.Add(note);
                note.OnNoteCollected += CheckNote;
            }
            

        }

    }
    
    private void EvaluatingLevelEntered()
    {

        // check to see if we are done the level
        if (winningTrackNotes.Count == 0)
        {
            GotoState(LevelState.LevelOver);
        }
        else
        {
            feedback.text = "Oops - this was the wrong track! Try again!";
            GotoState(LevelState.FlyingTrack);
        }
    }

    private void AllNotesFoundEntered()
    {

        // let gameManager know that level is complete
        gameManager.OnLevelCompleted();

    }

    private void LevelOverEntered()
    {
        // TODO: Update UI with Level Over content
        feedback.text = ($"Level {currentLevel.name} over!");
    }

    private void CheckNote(Note hitNote)
    {
        notesCollected++;
        feedback.text = ($"Hit {hitNote.noteName}");

        foreach(Note note in winningTrackNotes)
        {
            if(note == hitNote)
            {
                winningTrackNotes.Remove(note);
               
            }
            
        }

        if (notesCollected == winningTrackNotes.Count)
        {
            GotoState(LevelState.EvaluatingLevel);
        }

    }

    private IEnumerator PerformTrackHint()
    {
        
        foreach(Note note in winningTrackNotes)
        {

            note.audioSource.Play();

            // if visual clues become a thing (i.e., keyboard keys light up) it can go here too
            yield return new WaitForSeconds(currentLevel.clueTiming);
        }

        if(currentState == LevelState.LoadingLevel) GotoState(LevelState.FlyingTrack);
    }
}
