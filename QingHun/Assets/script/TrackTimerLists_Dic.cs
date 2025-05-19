using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName ="newTrackTimer",menuName ="CreateData/Create New TrackTimerData")]
public class TrackTimerLists_Dic : ScriptableObject
{
    public List<PointGameObject> trackTimerLists;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class PointData
{
    public float timer;
    public int trackId;
    public PointData()
    {

    }
    public PointData(int timer,int trackId)
    {
        this.timer = timer;
        this.trackId = trackId;
    }
}

[System.Serializable]
public class PointGameObject :PointData
{
    public GameObject gameObject;
}