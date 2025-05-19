using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrumBeat : MonoBehaviour
{
    private int currentTrackId = -999;
    public Image[] tracksIMgs;
    public Transform[] tracksPoses;
    public Transform line;
    public GameObject pointPre;
    public AudioSource bgm;
    public Slider bgmslider;
    private bool _dragSlider = false;
    private float _cooldown = 0.05f;
    public TrackTimerLists_Dic TrackTimerLists_Dic;
    public int currentTrackCounts = 6;
    string isonString = "";
    // Start is called before the first frame update
    void Start()
    {
        bgm.Pause();
    }
    void OnClickPlay()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(bgm.isPlaying)
            {
                bgm.Pause();
                Debug.Log(TrackTimerLists_Dic.trackTimerLists.ToString());
            }
            else
            {
                bgm.Play();
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        OnClickPlay();
        if(!_dragSlider)
        {
            bgmslider.value = bgm.time / bgm.clip.length;
        }
        else
        {
            bgm.time = bgmslider.value * bgm.clip.length;
        }
        foreach (var point in TrackTimerLists_Dic.trackTimerLists)
        {
            point.gameObject.transform.position = new Vector3(point.trackId, (bgm.time - (point.timer)) * 10, 0);
        }

        _cooldown -= Time.deltaTime;
        if(_cooldown<=0)
        {
            _cooldown = 0.05f;
            AddPointFromKeyCode(KeyCode.Q);
            AddPointFromKeyCode(KeyCode.W);
            AddPointFromKeyCode(KeyCode.E);
            AddPointFromKeyCode(KeyCode.B);
            AddPointFromKeyCode(KeyCode.N);
            AddPointFromKeyCode(KeyCode.M);
        }
    }

    private void AddPoint(int trackId, float currentTime)
    {
        PointGameObject pNode = new PointGameObject();
        pNode.timer = currentTime;
        pNode.trackId = trackId;
        pNode.gameObject = Instantiate(pointPre);
        TrackTimerLists_Dic.trackTimerLists.Add(pNode);
    }
    public void AddPointFromKeyCode(KeyCode keyCode)
    {
        if(Input.GetKey(keyCode))
        {
            if(currentTrackCounts==6)
            {
                switch (keyCode)
                {
                    case KeyCode.Q:
                        currentTrackId = -5;
                        Debug.Log("w");
                        break;
                    case KeyCode.W:
                        currentTrackId = -3;
                        break;
                    case KeyCode.E:
                        currentTrackId = -1;
                        break;
                    case KeyCode.B:
                        currentTrackId = 1;
                        break;
                    case KeyCode.N:
                        currentTrackId = 3;
                        break;
                    case KeyCode.M:
                        currentTrackId = 5;
                        break;
                }
            }
            AddPoint(currentTrackId, bgm.time);
        }
    }
}
