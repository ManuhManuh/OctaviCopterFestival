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

    private GameManager gameManager;
    private TutorialState currentState;
    private GameObject locomotionControls;
    private Key[] keys;

    private bool hintStarted = false;
    private bool firstAttemptAtLevel = true;
    private AudioClip[] currentClipArray;
    private Note firstExampleNote;
    private Note secondExampleNote;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        keys = FindObjectsOfType<Key>();
        locomotionControls = GameObject.Find("Locomotion");
        currentClipArray = Localization.currentVOTable;

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
                    foreach (Key key in keys)
                    {
                        if (key.BeenPlayed)
                        {
                            GotoTutorialState(TutorialState.WaitingForHintPlay);
                        }
                    }
                        
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
        // clip 1a
        audioSource.PlayOneShot(currentClipArray[0]);
        while (audioSource.isPlaying)
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

        // clip 1b
        audioSource.PlayOneShot(currentClipArray[1]);
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        // remove the C Major scale and go to Flying state
        Destroy(cMajorScale.gameObject);
        GotoTutorialState(TutorialState.FlyingFirstNote);
    }

    private void FlyingFirstNoteEntered()
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

        // clip 2
        audioSource.PlayOneShot(currentClipArray[2]);

        EnableFlying();

    }

    private void OnFirstNoteHit(Note hitNote)
    {
        GotoTutorialState(TutorialState.FlyingSecondNote);
        
    }

    private void FlyingSecondNoteEntered()
    {
        // play Clip 3
        audioSource.PlayOneShot(currentClipArray[3]);
    }

    private void OnSecondNoteHit(Note hitNote)
    {
        // play clip 4
        audioSource.PlayOneShot(currentClipArray[4]);
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
        // play clip 5a
        audioSource.PlayOneShot(currentClipArray[5]);
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        gameManager.StartLevel(tutorialLevel);  // hint notes are not played on instantiation for tutorial level

        yield return new WaitForSeconds(2.0f);

        // play clip 5b
        audioSource.PlayOneShot(currentClipArray[6]);
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        // play hint tones
        StartCoroutine(gameManager.CurrentLevelManager.PerformTrackHint());
        while (gameManager.CurrentLevelManager.HintIsPlaying)
        {
            yield return null;
        }

        // play clip 5c
        audioSource.PlayOneShot(currentClipArray[7]);
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        GotoTutorialState(TutorialState.WaitingForKeyboardPlay);

    }

    private void KeyboardEntered()
    {
        // play clip 6
        audioSource.PlayOneShot(currentClipArray[8]);

    }

    private void HintEntered()
    {
        // play clip 7
        audioSource.PlayOneShot(currentClipArray[9]);
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
        // play clip 8
        audioSource.PlayOneShot(currentClipArray[10]);

    }

    private void TrackSelectionEntered()
    {
        // play Clip 9
        audioSource.PlayOneShot(currentClipArray[11]);
    }

    private void TestDriveEntered()
    {
        // play clip 10
        if (firstAttemptAtLevel)
        {
            firstAttemptAtLevel = false;
            audioSource.PlayOneShot(currentClipArray[12]);
        }
  
    }

    private void SuccessFeedbackEntered()
    {
        audioSource.PlayOneShot(currentClipArray[14]);
        // Game Manager should now be waiting for X or A to start game
        Debug.Log("Game Manager should now be waiting for X or A to start game");
    }

    private void TryAgainFeedbackEntered()
    {
        StartCoroutine(TryAgainFeedbackSection());
    }

    private IEnumerator TryAgainFeedbackSection()
    {
        audioSource.PlayOneShot(currentClipArray[13]);
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        GotoTutorialState(TutorialState.WaitingForLevelRestart);
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
