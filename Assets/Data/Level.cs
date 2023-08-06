using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class Level : ScriptableObject
{
    public int levelID;
    public string levelName;
    public Track[] tracks;
    public Track winningTrack;
    public int pointValue;
    public float trackSpacing;
    public float noteSpacing;
    public float trackStartDistance;
    public float clueTiming;
}
