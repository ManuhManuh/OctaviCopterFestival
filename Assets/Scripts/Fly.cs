using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour

    
{
    public bool Flying => flying;

    [SerializeField] GameObject octaviCopter;
    [SerializeField] HingeJoint hinge;
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
            if(hinge.angle < 0)
            {
                 FlyUp();
                Debug.Log("Flying up!");
            }
            else
            {
                 FlyDown();
                Debug.Log("Flying down!");
            }
        }
    }

    public void FlyUp()
    {
        // Fly up
        Debug.Log($"Angle: { hinge.angle}");
        float speedModifier = hinge.angle * angleCoefficient;
        octaviCopter.transform.Translate(Vector3.up * speed * speedModifier * Time.deltaTime);
    }

    public void FlyDown()
    {
        // Fly down
        Debug.Log($"Angle: { hinge.angle}");
        float speedModifier = hinge.angle * -angleCoefficient;
        octaviCopter.transform.Translate(Vector3.down * speed * speedModifier * Time.deltaTime);
    }

    public void OnStickHeld()
    {

        flying = true;

    }

    public void OnStickReleased()
    {
        flying = false;

    }

   
}
