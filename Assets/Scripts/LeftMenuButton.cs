using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class LeftMenuButton : MonoBehaviour
{
    [SerializeField] private InputActionReference returnToMainAction;
    [SerializeField] private Animator menuAnimation;
    [SerializeField] private AudioSource audioSourceExterior;

    [SerializeField] private XRPokeInteractor leftPokeInteractor;
    [SerializeField] private XRPokeInteractor rightPokeInteractor;
    [SerializeField] private XRDirectInteractor leftDirectInteractor;
    [SerializeField] private XRDirectInteractor rightDirectInteractor;

    private bool menuOpen = true;

    private void Start()
    {
        returnToMainAction.action.performed += MainMenuButtonPress;

    }

    private void MainMenuButtonPress(InputAction.CallbackContext obj)
    {

        if (!this.gameObject.activeInHierarchy) return;


        if (menuOpen)
        {
            leftPokeInteractor.gameObject.SetActive(false);
            rightPokeInteractor.gameObject.SetActive(false);

            leftDirectInteractor.gameObject.SetActive(true);
            rightDirectInteractor.gameObject.SetActive(true);

            StartCoroutine(CloseMenu());
        }
        else
        {
            leftPokeInteractor.gameObject.SetActive(true);
            rightPokeInteractor.gameObject.SetActive(true);

            leftDirectInteractor.gameObject.SetActive(false);
            rightDirectInteractor.gameObject.SetActive(false);

            StartCoroutine(OpenMenu());
        }
        
    }

    public void CloseWithoutButtonPress()
    {
        StartCoroutine(CloseMenu());
    }

    private IEnumerator OpenMenu()
    {
        menuAnimation.enabled = true;
        menuAnimation.SetBool("MenuIsOpen", true);

        yield return new WaitForSeconds(0.65f);
        audioSourceExterior.PlayOneShot(audioSourceExterior.clip);
        menuOpen = true;

    }

    private IEnumerator CloseMenu()
    {
        menuAnimation.SetBool("MenuIsOpen", false);

        yield return new WaitForSeconds(0.25f);
        audioSourceExterior.PlayOneShot(audioSourceExterior.clip);

        yield return new WaitForSeconds(2.0f);  //wait for animation to finish
        menuAnimation.enabled = false;

        menuOpen = false;
    }



}
