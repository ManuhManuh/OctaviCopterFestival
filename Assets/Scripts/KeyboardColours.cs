using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardColours : MonoBehaviour
{
    [SerializeField] float colourDelay;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnKeyPlayed(Key key)
    {
        if(gameManager.Mode == "beginner")
        {
            key.GetComponent<MeshRenderer>().material = key.hintMaterial;
        }
    }

    public void OnKeyReleased(Key key)
    {
        if (gameManager.Mode == "beginner")
        {
            StartCoroutine(ColourSustain(key, colourDelay));
        }
        
    }

    private IEnumerator ColourSustain(Key key, float delay)
    {
        yield return new WaitForSeconds(delay);
        key.GetComponent<MeshRenderer>().material = key.originalMaterial;

    }
}
