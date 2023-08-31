using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class LeftMenuButton : MonoBehaviour
{
    [SerializeField] InputActionReference returnToMainAction;
    //[SerializeField] PlayableDirector menuOpenDirector;
    //[SerializeField] PlayableDirector menuCloseDirector;

    //[SerializeField] private Animator menuAnimation;
    //[SerializeField] private Animator keyboardAnimation;
    //[SerializeField] private Animator windowAnimation;
    //[SerializeField] private Animator roofAnimation;

    [SerializeField] private Animator menuAnimation;

    [SerializeField] private AudioSource audioSourceExterior;

    private bool menuOpen = true;
    //private GameObject mainMenu;

    private void Start()
    {
        returnToMainAction.action.performed += MainMenuButtonPress;
        //mainMenu = GameObject.Find("MainMenu");
    }

    private void MainMenuButtonPress(InputAction.CallbackContext obj)
    {
        // Debug.Log($"MainMenu pressed by {this.gameObject.name}");

        if (!this.gameObject.activeInHierarchy) return;

        //Debug.Log("Main menu button pushed - add code to open the main menu once there is one");

        if (menuOpen)
        {

            //menuCloseDirector.Play();
            //menuOpen = false;
            StartCoroutine(CloseMenu());
        }
        else
        {
            //menuOpenDirector.Play();
            //menuOpen = true;
            StartCoroutine(OpenMenu());
        }
        
        
    }


    private IEnumerator OpenMenu()
    {
        // yield return new WaitForSeconds(0.1f);

        //menuAnimation.SetTrigger("OpenMenu");
        //keyboardAnimation.SetTrigger("PauseUI_OFF");

        //windowAnimation.SetTrigger("CloseWindows");
        //roofAnimation.SetTrigger("CloseRoof");

        menuAnimation.SetBool("MenuIsOpen", true);

        yield return new WaitForSeconds(0.65f);
        audioSourceExterior.PlayOneShot(audioSourceExterior.clip);
        //mainMenu.SetActive(true);

        menuOpen = true;

    }

    private IEnumerator CloseMenu()
    {
        //menuAnimation.SetTrigger("CloseMenu");
        //keyboardAnimation.SetTrigger("PauseUI_ON");

        //windowAnimation.SetTrigger("OpenWindows");
        //roofAnimation.SetTrigger("OpenRoof");

        menuAnimation.SetBool("MenuIsOpen", false);

        yield return new WaitForSeconds(0.25f);
        audioSourceExterior.PlayOneShot(audioSourceExterior.clip);

        //yield return new WaitForSeconds(1.0f);
        //mainMenu.SetActive(false);

        menuOpen = false;
    }



}
