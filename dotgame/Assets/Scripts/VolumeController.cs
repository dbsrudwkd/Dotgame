using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeController : MonoBehaviour
{
    private Slider volumeSlider;
    private const string VOLUME_KEY = "BGMVolume";

    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 1f;
            volumeSlider.maxValue = 100f;
            volumeSlider.wholeNumbers = true; // 정수 단위

            // 기존에 저장되어 있던 음량을 불러와서 슬라이더 위치를 맞춤 (기본값 50)
            float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 50f);
            volumeSlider.value = savedVolume;

            // 슬라이더 값이 변경될 때 실행될 이벤트 등록
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    private void OnVolumeChanged(float value)
    {
        // 1. 변경된 값을 저장소에 기록 (모든 씬이 공유함)
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        PlayerPrefs.Save(); // 즉시 저장

        // 2. 현재 설정 씬에 BGMManager가 존재한다면 실시간으로 소리 크기 반영
        BGMManager currentManager = FindFirstObjectByType<BGMManager>();
        if (currentManager != null)
        {
            currentManager.SetVolume(value);
        }
    }
}