using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeftMenuButton : MonoBehaviour
{
    [SerializeField] InputActionReference returnToMainAction;
    [SerializeField] Animator windowAnimation;
    [SerializeField] Animator roofAnimation;
    // [SerializeField] Animator pauseUIAnimation;
    [SerializeField] AudioSource audioSourceRoof;
    
    private bool open = false;

    private void Start()
    {
        returnToMainAction.action.performed += MainMenuButtonPress;
    }

    private void MainMenuButtonPress(InputAction.CallbackContext obj)
    {
        // Debug.Log($"MainMenu pressed by {this.gameObject.name}");

        if (!this.gameObject.activeInHierarchy) return;

        //Debug.Log("Main menu button pushed - add code to open the main menu once there is one");

        if (open)
        {
            windowAnimation.SetTrigger("CloseWindows");
            roofAnimation.SetTrigger("CloseRoof");
            // pauseUIAnimation.SetTrigger("PauseUI_OFF");
            open = false;
            StartCoroutine(DoorSwish(0.65f));
        }
        else
        {
            windowAnimation.SetTrigger("OpenWindows");
            roofAnimation.SetTrigger("OpenRoof");
            // pauseUIAnimation.SetTrigger("PauseUI_ON");
            open = true;
            StartCoroutine(DoorSwish(0.25f));
        }
        
        
    }

    private IEnumerator DoorSwish(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSourceRoof.PlayOneShot(audioSourceRoof.clip);
    }
    
}
