using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

/*
public class TrackPosition : MonoBehaviour
{
    public Vector3 targetPosition;

    public void Start()
    {
        transform.position = targetPosition;
    }
}


*/

public class TrackPosition : MonoBehaviour
{
    public float newYPosition = 0f; // Assign the desired Y position in the Inspector
    public float newZPosition = 0f;

    private void Start()
    {
        // Adjust the Y position of the game object
        Vector3 newPosition = new Vector3(transform.position.x, newYPosition, newZPosition);
        transform.position = newPosition;
    }
}



/*
public class AdjustYPosition : MonoBehaviour
{
    public float newYPosition = -5f; // Assign the desired Y position in the Inspector

    private void Start()
    {
        // Adjust the Y position of the game object
        Vector3 newPosition = transform.position;
        newPosition.y = newYPosition;
        transform.position = newPosition;
    }
}
*/