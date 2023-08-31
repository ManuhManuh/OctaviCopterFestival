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
        Debug.Log($"Game manager mode: {gameManager.Mode}");
        Debug.Log($"Current locale: {Localization.currentLocale}");
        string modeKey = Localization.FindKeyFromValue(Localization.currentLocale, gameManager.Mode);
        Debug.Log($"Mode: {modeKey}");
        if(modeKey == "ModeBeginner")
        {
            key.GetComponent<MeshRenderer>().material = key.hintMaterial;
        }
    }

    public void OnKeyReleased(Key key)
    {
        string modeKey = Localization.FindKeyFromValue(Localization.currentLocale, gameManager.Mode);

        if (modeKey == "ModeBeginner")
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
