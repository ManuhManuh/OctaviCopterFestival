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
        YBChoose


    }

    [SerializeField] public InputActionReference primaryButtonPress;
    [SerializeField] public InputActionReference secondaryButtonPress;


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

        //switch (currentState)
        //{
        //    case TourState.XAStart:
        //        {
        //            float buttonPressValue = primaryButtonPress.action.ReadValue<float>();
        //            if (buttonPressValue > 0 && !clipsPlaying)
        //            {
        //                //Destroy(xaStart);
        //                GotoTourState(TourState.YBChoose);

        //            }

        //        }
        //        break;

        //    case TourState.YBChoose:
        //        {
        //            float buttonPressValue = secondaryButtonPress.action.ReadValue<float>();
        //            if (buttonPressValue > 0 && !clipsPlaying)
        //            {
        //                //Destroy(ybChoose);
        //                GotoTourState(TourState.None);
        //            }

        //        }
        //        break;


        //}
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
        }
    }

    private void XAStartEntered()
    {
        StartCoroutine(XAStartSection());
    }

    private IEnumerator XAStartSection()
    {
        GameObject xaStart = Instantiate(csTourPrefabs[0], Vector3.zero, Quaternion.identity);

        string[] clips = { "30" };
        StartCoroutine(PlayAndDisplay(clips));
        while(clipsPlaying)
        {
            yield return null;
        }
        
        Destroy(xaStart.gameObject);
        GotoTourState(TourState.YBChoose);
    }

    private void YBChooseEntered()
    {
        StartCoroutine(YBChooseSection());
    }

    private IEnumerator YBChooseSection()
    {
        GameObject ybChoose = Instantiate(csTourPrefabs[1], Vector3.zero, Quaternion.identity);

        string[] clips = { "31" };
        StartCoroutine(PlayAndDisplay(clips));
        while (clipsPlaying)
        {
            yield return null;
        }
      
        Destroy(ybChoose.gameObject);      
        GotoTourState(TourState.None);
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


// gpt change color every second
// To use this extension, you need an OpenAI account and provide it's API key to this extension
// https://platform.openai.com/account/api-keys
// Provide key by one of the below
// * Create an environment variable called OPENAI_API_KEY with the API key
// * Create a file with name .openai under user profile directory with the API key
// * Set API key in options, Tools->Options->Comment2GPT->General->Authentication