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
        HomeButton,
        MenuButton,
        LastState
    }

    [SerializeField] private InputActionReference primaryButtonPress;
    [SerializeField] private InputActionReference secondaryButtonPress;
    [SerializeField] private InputActionReference joystickButtonPress;
    [SerializeField] private InputActionReference triggerButtonPress;
    [SerializeField] private InputActionReference homeButtonPress;
    [SerializeField] private InputActionReference menuButtonPress;

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
    private GameObject homeButton;
    private GameObject menuButton;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        currentClipArray = Localization.currentVOTable;
        uiDisplay = GameObject.FindObjectOfType<UIDisplay>();

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
                        GotoTourState(TourState.HomeButton);                          
                    }
                    break;                    
                }
            
            case TourState.HomeButton:
                {
                    /*
                    float buttonPressValue = homeButtonPress.action.ReadValue<float>();
                    if(buttonPressValue > 0 && !clipsPlaying)
                    {
                        Destroy(homeButton);
                        GotoTourState(TourState.MenuButton);
                    }
                    */

                    if (!clipsPlaying)
                    { 
                        Destroy(homeButton);
                        GotoTourState(TourState.MenuButton);
                    }                   
                    break;
                }
            
            case TourState.MenuButton:
                {
                    float buttonPressValue = menuButtonPress.action.ReadValue<float>(); 
                    if(buttonPressValue > 0 && !clipsPlaying)
                    {
                        Destroy(menuButton);
                        GotoTourState(TourState.None);
                       
                    }
                    break;
                }

            case TourState.LastState:
                {
                    float buttonPressValue = triggerButtonPress.action.ReadValue<float>();
                    if (buttonPressValue > 0 && !clipsPlaying)
                    {
                        Destroy(menuButton);
                        GotoTourState(TourState.None);
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
            case TourState.HomeButton:
                {
                    HomeButtonEntered();
                    break;
                }
            case TourState.MenuButton:
                {
                    MenuButtonEntered();
                    break;
                }       
        }
    }

    private void XAStartEntered()
    {
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
        
        // Destroy(xaStart.gameObject);
        // GotoTourState(TourState.YBChoose);
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

    private void HomeButtonEntered()
    {
        StartCoroutine(HomeButtonSection());
    }
    private IEnumerator HomeButtonSection()
    {
        homeButton = Instantiate(csTourPrefabs[4], Vector3.zero, Quaternion.identity);
        string[] clips = { "35" };
        StartCoroutine(PlayAndDisplay(clips));
        while (clipsPlaying)
        { 
            yield return null;
        }            
    }

    private void MenuButtonEntered()
    {
        StartCoroutine(MenuButtonSection());
    }

    private IEnumerator MenuButtonSection()
    {
        menuButton = Instantiate(csTourPrefabs[5], Vector3.zero, Quaternion.identity);
        string[] clips = { "36" };
        StartCoroutine(PlayAndDisplay(clips));
        while (clipsPlaying)
        {
            yield return null;
        }      
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
}