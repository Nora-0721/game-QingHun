// TrackTimerLists_DicY.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newTrackTimer", menuName = "CreateData/Create New TrackTimerDataY")]
public class TrackTimerLists_DicY : ScriptableObject
{
    public List<PointGameObjectY> trackTimerLists;
}

[System.Serializable]
public class PointDataY
{
    public float startY;
    public int trackId;
    public float scaleY = 1f; // ÐÂÔöYÖáËõ·Å×Ö¶Î

    public PointDataY(float startY, int trackId, float scaleY)
    {
        this.startY = startY;
        this.trackId = trackId;
        this.scaleY = scaleY;
    }
}

[System.Serializable]
public class PointGameObjectY : PointDataY
{
    public GameObject gameObject;

    public PointGameObjectY(float startY, int trackId, float scaleY)
        : base(startY, trackId, scaleY) { }
}