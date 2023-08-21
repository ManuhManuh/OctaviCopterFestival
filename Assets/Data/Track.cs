using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Track", menuName = "Track")]
public class Track : ScriptableObject
{
    public int trackID;
    public string trackDescription;
    public Note[] notes;
    public GameObject trackObject;
}
