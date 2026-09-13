using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위해 추가

public class BossEntranceTrigger : MonoBehaviour
{
    public GameObject dialogPanel; // UI 팝업창 연동
    public string battleSceneName = "03_BattleScene";

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 트리거 영역에 들어왔을 때
        if (other.CompareTag("Player"))
        {
            OpenDialog();
        }
    }

    public void OpenDialog()
    {
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(true);
        }
    }

    public void CloseDialog()
    {
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
    }

    // 확인 버튼 눌렀을 때 호출
    public void OnConfirm()
    {
        // 싱글톤 매니저 없이 direct로 씬 이동 처리
        SceneManager.LoadScene(battleSceneName);
    }
}