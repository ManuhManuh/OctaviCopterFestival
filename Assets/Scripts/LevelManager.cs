using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    
    public enum LevelState
    {
        None,
        LoadingLevel,
        ReconaissanceFlying,
        FlyingTrack,
        EvaluatingLevel
      
    }

    public float LevelHeight => levelHeight;
    private float levelHeight;
    public bool HintIsPlaying => hintIsPlaying;
    public GameObject EnvironmentAsset => environmentAsset;

    [SerializeField] private InputActionReference cycleTrackActionReference;
    [SerializeField] private InputActionReference startFlyingActionReference;

    private GameManager gameManager;
    private LevelState currentState;
    private Level currentLevel;
    private GameObject environmentAsset;
    
    private Dictionary<Note, bool> levelNotes = new Dictionary<Note, bool>();
    private int correctNotesCollected;
    private int notesCollected;
    private int notesPerTrack;
    private List<float> trackXPositions = new List<float>();
    private List<float> trackYPositions = new List<float>();
    private List<GameObject> trackObjects = new List<GameObject>();
    private GameObject noteCollector;
    private int selectedTrack;
    private bool cycleEnabled;

    private TMP_Text levelTitle;
    private TMP_Text levelInstructions;
    private UIDisplay uiDisplay;
    private string currentFeedbackText;
    private string lastNoteHit;
    private bool hintIsPlaying = false;

    private void Awake()
    {
        // Get reference to game manager and note collector
        gameManager = FindObjectOfType<GameManager>();
        noteCollector = GameObject.Find("NoteCollector");
    }
    private void Start()
    {
        
        noteCollector.SetActive(false); // disable until a track is selected

        // Set the initial state and starting values of flags and indexes
        currentLevel = gameManager.CurrentLevel;

        levelTitle = GameObject.Find("LevelTitle").GetComponent<TMP_Text>();
        levelInstructions = GameObject.Find("LevelInstructions").GetComponent<TMP_Text>();
        uiDisplay = GameObject.FindObjectOfType<UIDisplay>();

        if (currentLevel.environmentAsset != null)
        {
            environmentAsset = Instantiate(currentLevel.environmentAsset);

        }

        
        GotoState(LevelState.LoadingLevel);

    }

    private void Update()
    {
        if(currentState == LevelState.ReconaissanceFlying)
        {

            float buttonPressValue = cycleTrackActionReference.action.ReadValue<float>();
            if (buttonPressValue > 0 && cycleEnabled)
            {
                cycleEnabled = false;
                currentFeedbackText = "TrackSelect";
                SendMessageToUI();
                CycleThroughTracks();

            }

            float triggerPressValue = startFlyingActionReference.action.ReadValue<float>();
            if (triggerPressValue > 0)
            {
                GotoState(LevelState.FlyingTrack);

            }
        }

        if(currentState == LevelState.FlyingTrack)
        {
            float buttonPressValue = cycleTrackActionReference.action.ReadValue<float>();
            if (buttonPressValue > 0)
            {
                // move to start of selected track
                float newPlayerXPosition = trackXPositions[selectedTrack];
                float newPlayerYPosition = trackYPositions[selectedTrack];
                float newPlayerZPostion = gameManager.PlayerStartPosition.z;

                gameManager.player.transform.position = new Vector3(newPlayerXPosition, newPlayerYPosition, newPlayerZPostion);
            }
        }

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
            case LevelState.ReconaissanceFlying:
                {
                    ReconnaissanceFlyingEntered();
                    break;
                }

            case LevelState.FlyingTrack:
                {
                    FlyingTrackEntered();
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

            trackXPositions.Add(currentTrackPosition);

            createTracks.RemoveAt(randomTrack);
            currentTrackPosition += currentLevel.trackSpacing;

        } 

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
        UpdateLevelUIFields();

    }

    public void UpdateLevelUIFields()
    {
        uiDisplay.UpdateLevelTitle(currentLevel.name);
        uiDisplay.UpdateLevelInstructions(currentLevel.instructions);
    }

    public void SendMessageToUI()
    {
        switch (currentFeedbackText)
        {
            case "TrackSelect":
            case "RetryPrompt":
            case "Recon":
            case "FlyPrompt":
                {
                    uiDisplay.PresentFeedback(currentFeedbackText);
                    break;
                }
            case "NoteHit":
                {
                    uiDisplay.PresentFeedback(currentFeedbackText, lastNoteHit);
                    break;
                }

        }


    }
    private void InstantiateTrack(Track track, float trackXPosition)
    {
        float yPosition;
        float zPosition = currentLevel.trackStartDistance - currentLevel.noteSpacing;
        bool needToCollect = track.name == currentLevel.winningTrack.name;
        float firstNotePosition = -999;

        foreach(Note note in track.notes)
        {
            yPosition = note.height;
            if (firstNotePosition == -999)
            {
                firstNotePosition = note.height;
            }
            zPosition += currentLevel.noteSpacing;
            Vector3 notePosition = new Vector3(trackXPosition, yPosition, zPosition);

            Note newNote = Instantiate(note, notePosition, Quaternion.identity);

            // collect and subscribe to the note
            levelNotes.Add(newNote, needToCollect);
            newNote.OnNoteCollected += CheckNote;

        }

        if (track.trackObject != null)
        {
            float trackObjectZPosition = currentLevel.trackStartDistance - currentLevel.noteSpacing;
            Vector3 trackObjectPosition = new Vector3(trackXPosition, currentLevel.trackHeight, trackObjectZPosition);
            trackObjects.Add(Instantiate(track.trackObject, trackObjectPosition, Quaternion.identity));
        }

        trackYPositions.Add(firstNotePosition);
        notesPerTrack = track.notes.Length;

    }

    private void ReconnaissanceFlyingEntered()
    {
        currentFeedbackText = "Recon";
        SendMessageToUI();
        cycleEnabled = true;

    }
    private void CheckNote(Note hitNote)
    {
        notesCollected++;
        lastNoteHit = hitNote.noteName;
        currentFeedbackText = "NoteHit";
        SendMessageToUI();

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


    public IEnumerator PerformTrackHint()
    {
        hintIsPlaying = true;

        yield return new WaitForSeconds(1); 

        // PresentFeedback($"Giving {currentLevel.name} hint");
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

        if(currentState == LevelState.LoadingLevel) GotoState(LevelState.ReconaissanceFlying);
        hintIsPlaying = false;
    }

    public IEnumerator CleanUpAndEndLevel()
    {
        yield return new WaitForSeconds(3); // time for sound from last note collected to decay
        // destroy the notes, thereby also removing the subscriptions to them ;)
        Note[] notes = FindObjectsOfType<Note>();
        foreach (Note note in notes)
        {
            Destroy(note.gameObject);
        }
        if (trackObjects.Count > 0)
        {
            foreach (GameObject track in trackObjects)
            {
                Destroy(track);
            }
        }

        yield return new WaitForSeconds(1);

        Destroy(environmentAsset);

        // pass whether we collected the correct notes
        gameManager.OnLevelCompleted(correctNotesCollected == notesPerTrack, currentLevel.pointValue);

    }

    public void CycleThroughTracks()
    {

        // find out where the player is currently 
        float currentPlayerX = gameManager.player.transform.position.x;
        int positionIndex = 0; // start at the beginning if player is not in front of a track (i.e., was reconnaissance flying)

        // select the next position for the player
        for(int i = 0; i < trackXPositions.Count; i++)
        {
            if(trackXPositions[i] == currentPlayerX)
            {
                positionIndex = i;
            }
        }
        selectedTrack = (positionIndex + 1) % trackXPositions.Count; // wrap around at the end

        // move to next available track
        float newPlayerXPosition = trackXPositions[selectedTrack];
        float newPlayerYPosition = trackYPositions[selectedTrack];
        float newPlayerZPostion = gameManager.PlayerStartPosition.z;

        gameManager.player.transform.position = new Vector3(newPlayerXPosition, newPlayerYPosition, newPlayerZPostion);

        // delay the next cycle
        StartCoroutine(TrackCycleDelay(0.5f));
        
    }

    private IEnumerator TrackCycleDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        cycleEnabled = true;
    }
    public void FlyingTrackEntered()
    {
        currentFeedbackText = "FlyPrompt";
        SendMessageToUI();

        // disable strafe

        // enable note collector
        noteCollector.SetActive(true);

    }
}
