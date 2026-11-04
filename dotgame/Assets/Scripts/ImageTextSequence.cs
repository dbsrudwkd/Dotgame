using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ImageTextSequence : MonoBehaviour
{
    [System.Serializable]
    public class ImageTextPair
    {
        public Sprite image;
        public string text;
    }

    [Header("이미지 및 텍스트 페어")]
    public ImageTextPair[] pairs = new ImageTextPair[9];

    [Header("화면에 표시할 UI")]
    public Image displayImage;
    public TMP_Text displayText;

    [Header("다음 씬")]
    public string nextSceneName;

    private int currentIndex = 0;

    void Start()
    {
        ShowCurrentPair();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentIndex == 8)
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                NextPair();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void NextPair()
    {
        if (currentIndex < pairs.Length - 1)
        {
            currentIndex++;
            ShowCurrentPair();
        }
    }

    void ShowCurrentPair()
    {
        displayImage.sprite = pairs[currentIndex].image;
        displayText.text = pairs[currentIndex].text;
    }
}
