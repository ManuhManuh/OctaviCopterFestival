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
    public float noteSpacing;
    public float xPosition;
    public bool randomNoteSpawn;
}
