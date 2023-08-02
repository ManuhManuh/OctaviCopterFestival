using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fly : MonoBehaviour

    
{
    public bool Flying => flying;

    [SerializeField] GameObject octaviCopter;
    //[SerializeField] HingeJoint hinge;
    [SerializeField] GameObject stick;
    [SerializeField] float speed;
    [SerializeField] float angleCoefficient;
 
    private bool flying;

    // Start is called before the first frame update
    void Start()
    {
        flying = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (flying)
        {
            float stickAngle = stick.transform.rotation.eulerAngles.x;

            // if(hinge.angle < 0)
            if(stickAngle < 180)     // pushing away
            {
                 FlyUp(stickAngle);
                //Debug.Log("Flying up!");
            }
            if(stickAngle > 180)
            {
                 FlyDown(stickAngle - 360);
                //Debug.Log("Flying down!");
            }
        }
    }

    public void FlyUp(float stickAngle)
    {
        // Fly up

        // float speedModifier = hinge.angle * angleCoefficient;
        float speedModifier = stickAngle * angleCoefficient;
        octaviCopter.transform.Translate(Vector3.up * speed * speedModifier * Time.deltaTime);
    
    }

    public void FlyDown(float stickAngle)
    {
        // Fly down
    
        // float speedModifier = hinge.angle * -angleCoefficient;
        if(stickAngle > 270)
        {

        }
        float speedModifier = stickAngle * -angleCoefficient;
        octaviCopter.transform.Translate(Vector3.down * speed * speedModifier * Time.deltaTime);
      
    }

    public void OnStickHeld()
    {

        flying = true;

    }

    public void OnStickReleased()
    {
        flying = false;
        stick.transform.rotation = Quaternion.identity;

    }

   
}
