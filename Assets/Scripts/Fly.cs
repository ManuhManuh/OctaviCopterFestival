using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour

    
{
    public bool Flying => Flying;

    [SerializeField] GameObject player;
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
            }
            else
            {
                FlyDown();
            }
        }
    }

    public void FlyUp()
    {
        // Fly up
        player.transform.Translate(Vector3.up * speed * Time.deltaTime);
        gameObject.transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    public void FlyDown()
    {
        // Fly down
        player.transform.Translate(Vector3.down * speed * Time.deltaTime);
        gameObject.transform.Translate(Vector3.up * speed * Time.deltaTime);
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
