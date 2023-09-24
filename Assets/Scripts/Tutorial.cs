using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class Tutorial : MonoBehaviour
{
    public enum TutorialState
    {
        None,
        Introduction,
        FlyingFirstNote,
        FlyingSecondNote,
        WaitingForReset,
        Tracks,
        WaitingForKeyboardPlay,
        WaitingForHintPlay,
        Experimenting,
        TrackSelection,
        TestDrive,
        TryAgainFeedback,
        SuccessFeedback,
        WaitingForLevelRestart
    }

    [SerializeField] public InputActionReference primaryButtonPress;
    [SerializeField] public InputActionReference secondaryButtonPress;

    [SerializeField] AudioSource audioSource;

    [SerializeField] GameObject scalePrefab;
    [SerializeField] Note firstExampleNotePrefab;
    [SerializeField] Note secondExampleNotePrefab;
    [SerializeField] Level tutorialLevel;

    [SerializeField] float narrationPauseInterval = 0.5f;

    private GameManager gameManager;
    private TutorialState currentState;
    private GameObject locomotionControls;
    private Key[] keys;
    
    private bool hintStarted = false;
    private bool clipsPlaying = true;
    private bool firstLevelAttempt = true;

    private AudioClip[] currentClipArray;
    private UIDisplay uiDisplay;

    private Note firstExampleNote;
    private Note secondExampleNote;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        keys = FindObjectsOfType<Key>();
        locomotionControls = GameObject.Find("Locomotion");
        currentClipArray = Localization.currentVOTable;
        uiDisplay = GameObject.FindObjectOfType<UIDisplay>();

        currentState = TutorialState.None;
        GotoTutorialState(TutorialState.Introduction);
    }

    // Update is called once per frame
    void Update()
    {
        // pieces triggered by controller input
        switch (currentState)
        {
            case TutorialState.WaitingForReset:
            {
                float buttonPressValue = secondaryButtonPress.action.ReadValue<float>();
                if (buttonPressValue > 0)
                {
                    gameManager.player.transform.position = gameManager.PlayerStartPosition;
                    DisableFlying();
                    GotoTutorialState(TutorialState.Tracks);
                }

                break;
            }

            case TutorialState.WaitingForKeyboardPlay:
                {
                    bool keyPlayed = false;
                    foreach (Key key in keys)
                    {
                        Debug.Log($"Checking key {key.name}");
                        if (key.BeenPlayed)
                        {
                            keyPlayed = true;
                        }
                    }
                    
                    if(keyPlayed) GotoTutorialState(TutorialState.WaitingForHintPlay);
                    break;
                }

            case TutorialState.WaitingForHintPlay:
                {
                    if (!hintStarted && gameManager.CurrentLevelManager.HintIsPlaying)
                    {
                        hintStarted = true;
                        StartCoroutine(MonitorHint());
                    }
                    break;
                }

            case TutorialState.Experimenting:
                {
                    float buttonPressValue = primaryButtonPress.action.ReadValue<float>();
                    if (buttonPressValue > 0)
                    {
                        GotoTutorialState(TutorialState.TrackSelection);
                    }
                    break;
                }

            case TutorialState.TrackSelection:
                {

                    float buttonPressValue = secondaryButtonPress.action.ReadValue<float>();
                    if (buttonPressValue > 0)
                    {
                        EnableFlying();
                        GotoTutorialState(TutorialState.TestDrive);
                        
                    }

                    break;
                }
            case TutorialState.TestDrive:
                {
                    if (gameManager.TutorialCompleted)
                    {
                        if (gameManager.TutorialSuccessful)
                        {
                            GotoTutorialState(TutorialState.SuccessFeedback);
                        }
                        else
                        {
                            GotoTutorialState(TutorialState.TryAgainFeedback);
                        }
                        
                    }
                    break;
                }
            case TutorialState.WaitingForLevelRestart:
                {
                    float buttonPressValue = primaryButtonPress.action.ReadValue<float>();
                    if (buttonPressValue > 0)
                    {
                        gameManager.StartLevel(tutorialLevel);
                        GotoTutorialState(TutorialState.TestDrive);
                    }
                    break;
                }

        }
    }

    public void GotoTutorialState(TutorialState newState)
    {
        currentState = newState;
        OnStateEntered(currentState);
    }

    private void OnStateEntered(TutorialState state)
    {
        switch (state)
        {
            case TutorialState.None:
                {
                    break;
                }
            case TutorialState.Introduction:
                {
                    IntroductionEntered();
                    break;
                }
            case TutorialState.FlyingFirstNote:
                {
                    FlyingFirstNoteEntered();
                    break;
                }
            case TutorialState.FlyingSecondNote:
                {
                    FlyingSecondNoteEntered();
                    break;
                }
            case TutorialState.Tracks:
                {
                    TracksEntered();
                    break;
                }
            case TutorialState.WaitingForKeyboardPlay:
                {
                    KeyboardEntered();
                    break;
                }
            case TutorialState.WaitingForHintPlay:
                {
                    HintEntered();
                    break;
                }
            case TutorialState.Experimenting:
                {
                    ExperimentingEntered();
                    break;
                }
            case TutorialState.TrackSelection:
                {
                    TrackSelectionEntered();
                    break;
                }
            case TutorialState.TestDrive:
                {
                    TestDriveEntered();
                    break;
                }
            case TutorialState.SuccessFeedback:
                {
                    SuccessFeedbackEntered();
                    break;
                }
            case TutorialState.TryAgainFeedback:
                {
                    TryAgainFeedbackEntered();
                    break;
                }
            case TutorialState.WaitingForLevelRestart:
                {
                    break;
                }
        }
    }

    private void IntroductionEntered()
    {
        StartCoroutine(IntroductionSection());

    }

    private IEnumerator IntroductionSection()
    {
        DisableFlying();

        string[] clips1 = { "00", "01" };
        StartCoroutine(PlayAndDisplay(clips1));
        while (clipsPlaying)
        {
            yield return null;
        }

        // show C Major Scale
        GameObject cMajorScale = Instantiate(scalePrefab, Vector3.zero, Quaternion.identity);
        Animator animator = cMajorScale.GetComponent<Animator>();
        yield return new WaitForEndOfFrame();

        float NTime = 0;
        AnimatorStateInfo animatorStateInfo;

        while (NTime < 1.0f)
        {
            animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            NTime = animatorStateInfo.normalizedTime;
            yield return null;
        }
        //

        string[] clips2 = { "02", "03"};
        StartCoroutine(PlayAndDisplay(clips2));
        while (clipsPlaying)
        {
            yield return null;
        }

        // remove the C Major scale and go to FlyingFirstNote state
        Destroy(cMajorScale.gameObject);
        GotoTutorialState(TutorialState.FlyingFirstNote);
    }

    private void FlyingFirstNoteEntered()
    {
        StartCoroutine(FlyingFirstNoteSection());
    }

    private IEnumerator FlyingFirstNoteSection()
    {
        // Show the simple example and subscribe to their hit events
        firstExampleNote = Instantiate(firstExampleNotePrefab, Vector3.zero, Quaternion.identity);
        Vector3 firstNotePosition = new Vector3(0, firstExampleNote.height, 15.0f);
        firstExampleNote.transform.position = firstNotePosition;
        firstExampleNote.OnNoteCollected += OnFirstNoteHit;

        secondExampleNote = Instantiate(secondExampleNotePrefab, Vector3.zero, Quaternion.identity);
        Vector3 secondNotePosition = new Vector3(0, secondExampleNote.height, 40.0f);
        secondExampleNote.transform.position = secondNotePosition;
        secondExampleNote.OnNoteCollected += OnSecondNoteHit;

        string[] clips = { "04", "05" };
        StartCoroutine(PlayAndDisplay(clips));
        while (clipsPlaying)
        {
            yield return null;
        }

        EnableFlying();

        yield return new WaitForSeconds(narrationPauseInterval);

    }
    private void OnFirstNoteHit(Note hitNote)
    {
        GotoTutorialState(TutorialState.FlyingSecondNote);
        
    }

    private void FlyingSecondNoteEntered()
    {
        string[] clips = { "06" };
        StartCoroutine(PlayAndDisplay(clips));

    }

    private void OnSecondNoteHit(Note hitNote)
    {
        StartCoroutine(FlyingSecondNoteSection());
    }

    private IEnumerator FlyingSecondNoteSection()
    {
        string[] clips = { "07" };
        StartCoroutine(PlayAndDisplay(clips));
        while (clipsPlaying)
        {
            yield return null;
        }

        GotoTutorialState(TutorialState.WaitingForReset);
    }

    private void TracksEntered()
    {
        // destroy the simple example
        Destroy(firstExampleNote.gameObject);
        Destroy(secondExampleNote.gameObject);

        StartCoroutine(TrackSection());
            
    }

    private IEnumerator TrackSection()
    {
        string[] clips1 = { "08" };
        StartCoroutine(PlayAndDisplay(clips1));
        while (clipsPlaying)
        {
            yield return null;
        }

        gameManager.StartLevel(tutorialLevel);  // hint notes are not played on instantiation for tutorial level

        string[] clips2 = { "09" };
        StartCoroutine(PlayAndDisplay(clips2));
        while (clipsPlaying)
        {
            yield return null;
        }

        // play hint tones
        StartCoroutine(gameManager.CurrentLevelManager.PerformTrackHint());
        while (gameManager.CurrentLevelManager.HintIsPlaying)
        {
            yield return null;
        }

        string[] clips3 = { "10" };
        StartCoroutine(PlayAndDisplay(clips3));
        while (clipsPlaying)
        {
            yield return null;
        }

        GotoTutorialState(TutorialState.WaitingForKeyboardPlay);
    }

    private void KeyboardEntered()
    {
        string[] clips = { "11", "12" };
        StartCoroutine(PlayAndDisplay(clips));

    }

    private void HintEntered()
    {
        string[] clips = { "13", "14", "15" };
        StartCoroutine(PlayAndDisplay(clips));
    }

    private IEnumerator MonitorHint()
    {
        // wait for hint to finish
        while (gameManager.CurrentLevelManager.HintIsPlaying)
        {
            yield return null;

        }

        GotoTutorialState(TutorialState.Experimenting);

    }

    private void ExperimentingEntered()
    {
        string[] clips = { "16", "17" };
        StartCoroutine(PlayAndDisplay(clips));

    }

    private void TrackSelectionEntered()
    {
        string[] clips = { "18", "19" };
        StartCoroutine(PlayAndDisplay(clips));
    }

    private void TestDriveEntered()
    {
        if (firstLevelAttempt)
        {
            string[] clips = { "20" };
            StartCoroutine(PlayAndDisplay(clips));
        }
        else
        {
            uiDisplay.PresentFeedback("TrackSelect");
        }
        
    }

    private void TryAgainFeedbackEntered()
    {
        firstLevelAttempt = false;
        StartCoroutine(TryAgainFeedbackSection());

    }

    private IEnumerator TryAgainFeedbackSection()
    {
        string[] clips = { "21" };
        StartCoroutine(PlayAndDisplay(clips));
        while (clipsPlaying)
        {
            yield return null;
        }

        GotoTutorialState(TutorialState.WaitingForLevelRestart);
    }

    private void SuccessFeedbackEntered()
    {
        string[] clips = { "22","23", "24", "25"};
        StartCoroutine(PlayAndDisplay(clips));

    }

    private IEnumerator PlayAndDisplay(string[] clips)
    {
        clipsPlaying = true;

        yield return new WaitForSeconds(narrationPauseInterval);

        foreach(string clip in clips)
        {
            int clipNumber = Int32.Parse(clip);

            // update UI with text
            uiDisplay.PresentFeedback("TutorialClip"+clip);
            yield return new WaitForEndOfFrame();

            // play audio clip
            audioSource.PlayOneShot(currentClipArray[clipNumber]);
            while (audioSource.isPlaying)
            {
                yield return null;
            }

            yield return new WaitForSeconds(narrationPauseInterval);

        }

        clipsPlaying = false;

    }

    private void EnableFlying()
    {
        locomotionControls.SetActive(true);

    }

    private void DisableFlying()
    {
        locomotionControls.SetActive(false);

    }


}
