using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using static Tutorial;

public class SCTour : MonoBehaviour
{
    public enum TourState
    {
        None,
        XAStart,
        YBChoose
    }

    [SerializeField] AudioSource audioSource;
    //[SerializeField] List<GameObject> csTourAnimations;
    [SerializeField] List<GameObject> csTourPrefabs;

    [SerializeField] float narrationPauseInterval = 0.5f;

    private GameManager gameManager;
    public TourState currentState;

    private bool clipsPlaying = true;

    private AudioClip[] currentClipArray;
    private UIDisplay uiDisplay;



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
                    GameObject xaStart = Instantiate(csTourPrefabs[0], Vector3.zero, Quaternion.identity);
                }
                break;
        }
    }

    public void GotoTourState(TourState newState)
    {
        currentState = newState;
        OnStayedEntered(currentState);
    }

    private void OnStayedEntered(TourState state)
    {
        switch (state)
        {
            case TourState.None:
                {
                    break;
                }
            case TourState.XAStart:
                {
                    PlayAnimation();
                    break;
                }
            case TourState.YBChoose:
                {
                    PlayAnimation();
                    break;
                }
        }
    }

    private void XAStartEntered()
    {
        StartCoroutine(XAStart());
    }

    private IEnumerator XAStart()
    {
        string[] clips = { "30" };
        StartCoroutine(PlayAndDisplay(clips));
        while(clipsPlaying)
        {
            yield return null;
        }

        GameObject XAStart = Instantiate(csTourPrefabs[0], Vector3.zero, Quaternion.identity);
        Animator animator = XAStart.GetComponent<Animator>();
        yield return new WaitForEndOfFrame();

        float nTime = 0;
        AnimatorStateInfo animatorStateInfo;

        // wait for animation to complete
        while (nTime < 1.0f)
        {
            animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            nTime = animatorStateInfo.normalizedTime;
            yield return null;
        }

        Destroy(XAStart.gameObject);
        GotoTourState(TourState.YBChoose);
    }

    private IEnumerator YBChoose()
    {
        string[] clips = { "31" };
        StartCoroutine(PlayAndDisplay(clips));
        while (clipsPlaying)
        {
            yield return null;
        }

        GameObject YBChoose = Instantiate(csTourPrefabs[1], Vector3.zero, Quaternion.identity);
        Animator animator = YBChoose.GetComponent<Animator>();
        yield return new WaitForEndOfFrame();

        float nTime = 0;
        AnimatorStateInfo animatorStateInfo;

        // wait for animation to complete
        while (nTime < 1.0f)
        {
            animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            nTime = animatorStateInfo.normalizedTime;
            yield return null;
        }

        Destroy(YBChoose.gameObject);
        GotoTourState(TourState.YBChoose);
    }


    private string PlayAndDisplay(string[] clips3)
    {
        throw new NotImplementedException();
    }
    
    private void PlayAnimation()
    {

        PlayAnimation();

    }


}
