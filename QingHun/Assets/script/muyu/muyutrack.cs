using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class muyutrack : MonoBehaviour
{
    [Header("轨道时刻表文件")]
    public TrackTimerLists_Dic trackTimerLists_Dic1;
    public AudioSource bgm;
    private List<PointGameObject> tempTrackList = new List<PointGameObject>();
    // Start is called before the first frame update
    private void Start()
    {
        CreateNecks();
    }

    public void CreateNecks()
    {
        if (trackTimerLists_Dic1)
        {
            foreach (var item in trackTimerLists_Dic1.trackTimerLists)
            {
                AddPoint(item.trackId, item.timer, item.gameObject);
            }
        }
    }

    private void AddPoint(int trackId, float currentTime, GameObject prefab)
    {
        PointGameObject pNode = new PointGameObject();
        pNode.timer = currentTime;
        pNode.trackId = trackId;
        pNode.gameObject = Instantiate(prefab);
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
