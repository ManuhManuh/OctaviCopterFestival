using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class Level : ScriptableObject
{
    public int levelID;
    public string levelName;
    public Note[] requiredNotes;
    public float minXNotePosition = -3;
    public float maxXNotePosition = 3;
    public float minYNotePosition = -3;
    public float maxYNotePosition = 3;
    public float minZNotePosition = -3;
    public float maxZNotePosition = 3;
}
