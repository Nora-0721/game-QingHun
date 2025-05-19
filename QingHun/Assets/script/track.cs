// track.cs
using System.Collections.Generic;
using UnityEngine;

public class track : MonoBehaviour
{
    public TrackTimerLists_DicY trackTimerLists_Dic1;
    public AudioSource bgm;
    private List<PointGameObjectY> tempTrackList = new List<PointGameObjectY>();
    private const float MOVE_SPEED = 15f;

    private void Start() => CreateNecks();

    public void CreateNecks()
    {
        if (!trackTimerLists_Dic1) return;

        foreach (var item in trackTimerLists_Dic1.trackTimerLists)
        {
            var obj = Instantiate(
                item.gameObject,
                new Vector3(item.trackId, item.startY, 0),
                Quaternion.identity
            );

            // ”¶”√Y÷·Àı∑≈
            var newScale = obj.transform.localScale;
            newScale.y = item.scaleY;
            obj.transform.localScale = newScale;

            tempTrackList.Add(new PointGameObjectY(
                item.startY,
                item.trackId,
                item.scaleY)
            {
                gameObject = obj
            });
        }
    }

    void Update()
    {
        foreach (var point in tempTrackList)
        {
            float newY = point.startY - (bgm.time * MOVE_SPEED);
            point.gameObject.transform.position = new Vector3(
                point.trackId,
                newY,
                0
            );
        }
    }
}