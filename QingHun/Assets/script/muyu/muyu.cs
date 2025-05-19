using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class muyu : MonoBehaviour
{
    [Header("轨道时刻表文件")]
    public TrackTimerLists_Dic trackTimerLists_Dic;
    public AudioSource bgm;
    public GameObject pointPre;
    private List<PointGameObject> tempTrackList = new List<PointGameObject>();
    // Start is called before the first frame update
    private void Start()
    {
        CreateNecks();
    }

    public void CreateNecks()
    {
        if (trackTimerLists_Dic)
        {
            foreach (var item in trackTimerLists_Dic.trackTimerLists)
            {
                AddPoint(item.trackId, item.timer);
            }
        }
    }
    private void AddPoint(int trackId, float currentTime)
    {
        PointGameObject pNode = new PointGameObject();
        pNode.timer = currentTime;
        pNode.trackId = trackId;
        pNode.gameObject = Instantiate(pointPre);
        tempTrackList.Add(pNode);
    }
    // Update is called once per frame
    void Update()
    {
        foreach (var point in tempTrackList)
        {
            point.gameObject.transform.position = new Vector3(point.trackId, (bgm.time - point.timer) * -15, 0);
        }
    }
}
