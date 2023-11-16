using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;


public class SCTour : MonoBehaviour
{
    public enum TourState
    {
        None,
        XAStart,
        YBChoose,
        JoystickForward,
        TriggerTrick,
        LastState
    }

    [SerializeField] private InputActionReference primaryButtonPress;
    [SerializeField] private InputActionReference secondaryButtonPress;
    [SerializeField] private InputActionReference joystickButtonPress;
    [SerializeField] private InputActionReference triggerButtonPress;

    [SerializeField] AudioSource audioSource;
    [SerializeField] List<GameObject> csTourAnimations;
    [SerializeField] List<GameObject> csTourPrefabs;

    [SerializeField] float narrationPauseInterval = 0.5f;

    private GameManager gameManager;
    public TourState currentState;

    private bool clipsPlaying = true;

    private AudioClip[] currentClipArray;
    private UIDisplay uiDisplay;

    private GameObject xaStart;
    private GameObject ybChoose; 
    private GameObject joystickPrefab;
    private GameObject triggerTrickPrefab;

    private GameObject locomotionControls;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        currentClipArray = Localization.currentVOTable;
        uiDisplay = GameObject.FindObjectOfType<UIDisplay>();
        locomotionControls = GameObject.Find("Locomotion");

        currentState = TourState.None;
        GotoTourState(TourState.XAStart);
    }

    
    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case TourState.XAStart:
                {
                    float buttonPressValue = primaryButtonPress.action.ReadValue<float>();
                    if (buttonPressValue > 0 && !clipsPlaying)
                    {
                        Destroy(xaStart);
                        GotoTourState(TourState.YBChoose);
                    }
                }
                break;

            case TourState.YBChoose:
                {
                    float buttonPressValue = secondaryButtonPress.action.ReadValue<float>();
                    if (buttonPressValue > 0 && !clipsPlaying)
                    {
                        Destroy(ybChoose);
                        GotoTourState(TourState.JoystickForward);
                    }
                }
                break;

            case TourState.JoystickForward:
                {
                    float buttonPressValue = joystickButtonPress.action.ReadValue<float>();
                    if (buttonPressValue > 0 && !clipsPlaying)
                    {
                        Destroy(joystickPrefab);
                        GotoTourState(TourState.TriggerTrick);
                    }
                }
                break;

            case TourState.TriggerTrick:
                {
                    float buttonPressValue = triggerButtonPress.action.ReadValue<float>();
                    if(buttonPressValue > 0 && !clipsPlaying)
                    {
                        Destroy(triggerTrickPrefab);
                        GotoTourState(TourState.LastState);                          
                    }
                    break;                    
                }
            

            
        }
    }

    public void GotoTourState(TourState newState)
    {
        currentState = newState;
        OnStateEntered(currentState);
    }

    private void OnStateEntered(TourState state)
    {
        switch (state)
        {
            case TourState.None:
                {
                    break;
                }
            case TourState.XAStart:
                {      
                    XAStartEntered();
                    break;
                }
            case TourState.YBChoose:
                {
                    YBChooseEntered();
                    break;
                }
            case TourState.JoystickForward:
                {
                    JoystickEntered();
                    break;
                }
            case TourState.TriggerTrick:
                {
                    TriggerTrickEntered();
                    break;
                }
            case TourState.LastState:
                {
                    // may use this to remind player to use menu button to return to menu
                    LastStateEntered();
                    break;
                }
        }
    }

    private void XAStartEntered()
    {
        DisableFlying();
        StartCoroutine(XAStartSection());
    }

    private IEnumerator XAStartSection()
    {
        xaStart = Instantiate(csTourPrefabs[0], Vector3.zero, Quaternion.identity);

        string[] clips = { "30" };
        StartCoroutine(PlayAndDisplay(clips));
        while(clipsPlaying)
        {
            yield return null;
        }
        
    }

    private void YBChooseEntered()
    {
        StartCoroutine(YBChooseSection());
    }

    private IEnumerator YBChooseSection()
    {
        ybChoose = Instantiate(csTourPrefabs[1], Vector3.zero, Quaternion.identity);

        string[] clips = { "31" };
        StartCoroutine(PlayAndDisplay(clips));
        while (clipsPlaying)
        {
            yield return null;
        }
    }

    private void JoystickEntered()
    {
        StartCoroutine(JoystickSection());
    }

    private IEnumerator JoystickSection()
    {
        joystickPrefab = Instantiate(csTourPrefabs[2], Vector3.zero, Quaternion.identity);

        string[] clips = { "32", "33" };
        StartCoroutine(PlayAndDisplay(clips));
        while (clipsPlaying)
        {
            yield return null;
        }
    }

    private void TriggerTrickEntered()
    {
        StartCoroutine(TriggerSection());
    }

    private IEnumerator TriggerSection()
    {
        triggerTrickPrefab = Instantiate(csTourPrefabs[3], Vector3.zero, Quaternion.identity);

        string[] clips = { "34" };
        StartCoroutine(PlayAndDisplay(clips));
        while (clipsPlaying)
        {
            yield return null;
        }
    }

    private void LastStateEntered()
    {
        uiDisplay.PresentFeedback("PressMenuButton");
        EnableFlying();
    }

    private IEnumerator PlayAndDisplay(string[] clips)
    {
        clipsPlaying = true;

        yield return new WaitForSeconds(narrationPauseInterval);

        foreach (string clip in clips)
        {
            int clipNumber = Int32.Parse(clip);
            // update UI with text
            uiDisplay.PresentFeedback("TutorialClip" + clip);
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