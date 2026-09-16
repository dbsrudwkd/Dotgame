using UnityEngine;

public class SceneBGM : MonoBehaviour
{
    [Header("이 씬에서 재생할 오디오 클립 (.mp3 / .wav)")]
    public AudioClip bgmClip;

    // Start 대신 Awake나 OnEnable 시점에 호출하거나 약간의 시간차를 둡니다
    private void Start()
    {
        BGMManager manager = GetComponent<BGMManager>();

        if (manager == null)
        {
            manager = FindFirstObjectByType<BGMManager>();
        }

        if (manager != null && bgmClip != null)
        {
            manager.PlayBGM(bgmClip);
        }
        else
        {
            Debug.LogWarning("[SceneBGM] BGMManager 또는 AudioClip을 찾을 수 없습니다.");
        }
    }
}