using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    private AudioSource audioSource;
    private const string VOLUME_KEY = "BGMVolume"; // 저장소 키 이름

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 0f; // 2D 사운드
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    // 음악 재생
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;

        // 저장된 음량이 있으면 가져오고, 없으면 기본값 50으로 설정
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 50f);

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.volume = savedVolume / 100f; // 1~100 수치를 0.0~1.0으로 변환
        audioSource.Play();
    }

    // 씬 내에서 실시간으로 음량을 바꿀 때 호출할 수 있는 함수
    public void SetVolume(float volume)
    {
        float clampedVol = Mathf.Clamp(volume, 1f, 100f);
        if (audioSource != null)
        {
            audioSource.volume = clampedVol / 100f;
        }
    }
}