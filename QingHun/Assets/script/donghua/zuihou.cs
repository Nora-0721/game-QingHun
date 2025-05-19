using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class VideoController6 : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject rawImage;
    public string targetSceneName = "NextScene";

    void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoController: videoPlayer δ���ã�����Inspector��������Ƶ����������");
            return;
        }

        if (rawImage == null)
        {
            Debug.LogError("VideoController: rawImage δ���ã�����Inspector������RawImage����");
            return;
        }

        rawImage.SetActive(false); // ��ʼʱ������Ƶ����
        // ��ʼЭ�̣�����󲥷���Ƶ
        StartCoroutine(PlayVideoAfterDelay());
    }


    IEnumerator PlayVideoAfterDelay()
    {
        // �ȴ�����
        yield return new WaitForSeconds(0.1f);

        // ������Ƶ������״̬
        videoPlayer.Stop();
        videoPlayer.playOnAwake = false;

        Debug.Log("��ʼ׼����Ƶ... ��ƵURL: " + videoPlayer.url);

        // ׼����Ƶ
        videoPlayer.Prepare();

        // �ȴ���Ƶ׼�����
        while (!videoPlayer.isPrepared)
        {
            Debug.Log("��Ƶ׼����...");
            yield return null;
        }

        Debug.Log("��Ƶ׼����ɣ���ʼ���ţ���Ƶ����: " + videoPlayer.length + "��");

        // ��ʾ���沢������Ƶ
        rawImage.SetActive(true);
        videoPlayer.Play();

        // ������Ƶ���Ž����¼�
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("VideoController: ��Ƶ���Ž���");

        // ���ػ���
        if (rawImage != null)
        {
            rawImage.SetActive(false);
        }

        // �л�����
        SceneManager.LoadScene(targetSceneName);

        // ȡ�������¼�
        videoPlayer.loopPointReached -= OnVideoEnd;
    }
}