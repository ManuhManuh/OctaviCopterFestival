using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class LeftMenuButton : MonoBehaviour
{
    [SerializeField] private InputActionReference returnToMainAction;
    [SerializeField] private Animator menuAnimation;
    [SerializeField] private AudioSource audioSourceExterior;

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
            StartCoroutine(CloseMenu());
        }
        else
        {
            StartCoroutine(OpenMenu());
        }
        
    }


    private IEnumerator OpenMenu()
    {
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

        menuOpen = false;
    }



}
