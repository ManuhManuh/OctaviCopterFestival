using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour
{
    public enum TutorialState
    {
        None,
        Introduction,
        Flying,
        Tracks,
        Keyboard,
        TrackSelection,
        TestDrive
    }

    [SerializeField] public InputActionReference primaryButtonPress;
    [SerializeField] public InputActionReference secondaryButtonPress;
    
    [SerializeField] AudioSource audioSource;
    [SerializeField] private AudioClip[] englishClips;
    [SerializeField] private AudioClip[] germanClips;
    [SerializeField] private AudioClip[] spanishClips;

    [SerializeField] GameObject scalePrefab;
    [SerializeField] GameObject examplePrefab;

    private GameManager gameManager;
    private TutorialState currentState;
    private GameObject cMajorScale;
    private GameObject simpleExample;

    private bool firstNoteCollected = false;
    private bool secondNoteCollected = false;
    private bool keyboardPlayed = false;
    private bool hintPlayed = false;
    private bool cycleAttempted = false;
    private AudioClip[] currentClipArray;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        switch (Localization.currentLocale)
        {
            case Locale.en:
                currentClipArray = englishClips;
                break;
            case Locale.de:
                currentClipArray = germanClips;
                break;
            case Locale.es:
                currentClipArray = spanishClips;
                break;
        }

        currentState = TutorialState.None;
        GotoTutorialState(TutorialState.Introduction);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case TutorialState.Flying:
            {
                if (firstNoteCollected && secondNoteCollected)
                    {
                        float buttonPressValue = secondaryButtonPress.action.ReadValue<float>();
                        if (buttonPressValue > 0)
                        {
                            GotoTutorialState(TutorialState.Tracks);
                        }
                        
                    }
                break;
            }


            case TutorialState.Keyboard:
                {
                    if (keyboardPlayed && hintPlayed)
                    {
                        float buttonPressValue = primaryButtonPress.action.ReadValue<float>();
                        if (buttonPressValue > 0)
                        {
                            GotoTutorialState(TutorialState.TrackSelection);
                        }

                    }
                    break;
                }

            case TutorialState.TrackSelection:
                {
                    if (!cycleAttempted)
                    {
                        float buttonPressValue = secondaryButtonPress.action.ReadValue<float>();
                        if (buttonPressValue > 0)
                        {
                            cycleAttempted = true;
                            // play trackselection timeline
                                // play clip 10
                                // when clip is done, change state to TestDrive
                                
                        }

                    }
                    break;
                }

        }
    }

    public void GotoTutorialState(TutorialState newState)
    {
        OnStateExited(currentState);
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
            case TutorialState.Flying:
                {
                    FlyingEntered();
                    break;
                }
            case TutorialState.Tracks:
                {
                    TracksEntered();
                    break;
                }
            case TutorialState.Keyboard:
                {
                    FlyingEntered();
                    break;
                }


        }
    }

    private void OnStateExited(TutorialState state)
    {
        switch (state)
        {
            case TutorialState.None:
                {
                    break;
                }
            case TutorialState.Introduction:
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
        // clip 1a
        audioSource.PlayOneShot(currentClipArray[0]);
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        // show C Major Scale
        cMajorScale = Instantiate(scalePrefab, Vector3.zero, Quaternion.identity);
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

        // Replace C Major scale with simple example
        Destroy(cMajorScale.gameObject);
        simpleExample = Instantiate(examplePrefab, Vector3.zero, Quaternion.identity);
        GotoTutorialState(TutorialState.Flying);

    }

    private void FlyingEntered()
    {
        // clip 2
        // subscribe OnFirstNoteHit to C3 hit
    }

    private void OnFirstNoteHit()
    {
        firstNoteCollected = true;
        // play Clip 3
        // subscribe OnSecondNoteHit to E3 hit
    }

    private void OnSecondNoteHit()
    {
        secondNoteCollected = true;
        // play clip 4
    }

    private void TracksEntered()
    {
        // play Tracks timeline
            // play clip 5a
            // display 3 2-note tracks, one track at a time
            // play clip 5b
            // play hint tones
            // play clip 5c
        // when timeline finished, change state to Keyboard
    }

    private void KeyboardEntered()
    {
        // play clip 6

    }

    private void OnKeyboardPlayed(Key key)
    {
        keyboardPlayed = true;
        // play clip 7
        // make green buttons glow?

    }

    private void OnHintPlayed()
    {
        hintPlayed = true;
        // play clip 8
    }

    private void TrackSelectionEntered()
    {
        // play Clip 9
    }

    private void TestDriveEntered()
    {
        // all the things normally done when starting a level
            // check 
            // play 11a if incorrect
        // once over
            //gameManager.runningTutorial = false;
            // play clip 11b
    }

}
