using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeftMenuButton : MonoBehaviour
{
    [SerializeField] InputActionReference returnToMainAction;
    [SerializeField] Animator windowAnimation;
    [SerializeField] Animator roofAnimation;

    private bool open = false;

    private void Start()
    {
        returnToMainAction.action.performed += MainMenuButtonPress;
    }

    private void MainMenuButtonPress(InputAction.CallbackContext obj)
    {
        // Debug.Log($"MainMenu pressed by {this.gameObject.name}");

        if (!this.gameObject.activeInHierarchy) return;

        Debug.Log("Main menu button pushed - add code to open the main menu once there is one");

        if (open)
        {
            windowAnimation.SetTrigger("CloseWindows");
            roofAnimation.SetTrigger("CloseRoof");
            open = false;
        }
        else
        {
            windowAnimation.SetTrigger("OpenWindows");
            roofAnimation.SetTrigger("OpenRoof");
            open = true;
        }
        
        
    }
    
}
