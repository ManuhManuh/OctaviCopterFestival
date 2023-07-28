using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour

    
{
    public bool Flying => Flying;

    [SerializeField] GameObject UFO;
    [SerializeField] float speed;
    [SerializeField] HingeJoint hinge;

    private float stickOffset;
    private bool flying;

    // Start is called before the first frame update
    void Start()
    {
        
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
        UFO.transform.Translate(Vector3.up * speed * Time.deltaTime);
        //gameObject.transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    public void FlyDown()
    {
        // Fly down
        UFO.transform.Translate(Vector3.down * speed * Time.deltaTime);
        //gameObject.transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    public void OnStickHeld()
    {

        flying = true;

        // remove constraints on the stick so it can come along
        //hinge.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | 
        //                                                         RigidbodyConstraints.FreezeRotationZ;
    }

    public void OnStickReleased()
    {
        flying = false;

        // replace stick constraints
        //hinge.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | 
        //                                                         RigidbodyConstraints.FreezeRotationY | 
        //                                                         RigidbodyConstraints.FreezeRotationZ;
    }
}
