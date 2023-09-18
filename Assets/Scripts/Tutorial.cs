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

    private GameManager gameManager;
    private TutorialState currentState;

    private bool firstNoteCollected = false;
    private bool secondNoteCollected = false;
    private bool keyboardPlayed = false;
    private bool hintPlayed = false;
    private bool cycleAttempted = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

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
        // play introduction timeline
            // clip 1a
            // display C Major scale
            // clip 1b
            // display C3, D3
        // when timeline finished, change state to Flying

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
