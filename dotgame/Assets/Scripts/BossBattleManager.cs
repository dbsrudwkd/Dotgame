using System.Collections;
using UnityEngine;

public class BossBattleManager : MonoBehaviour
{
    public static BossBattleManager Instance;

    [Header("보스 스펙")]
    public int bossHP = 500;
    public GameObject coverWall;
    public GameObject bossObj;

    [Header("상태 카운터")]
    public int totalFiredCount = 0;
    public int parryHitCount = 0;
    public bool isGroggy = false;

    public float groggyDuration = 7.0f;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterBallFired()
    {
        if (isGroggy) return;

        totalFiredCount++;
        Debug.Log($"[보스 공격] 발사된 공 수: {totalFiredCount}/10");

        if (totalFiredCount >= 10)
        {
            StartGroggy();
        }
    }

    public void RegisterParryHit()
    {
        TakeDamage(30);
        parryHitCount++;
        Debug.Log($"[패링 성공!] 보스 피격 / 패링 적중 횟수: {parryHitCount}/5, 남은 HP: {bossHP}");

        if (parryHitCount >= 5 && !isGroggy)
        {
            StartGroggy();
        }
    }

    public void TakeDamage(int damage)
    {
        bossHP -= damage;
        if (bossHP <= 0)
        {
            bossHP = 0;
            Debug.Log("★ 보스 처치 완료! 승리! ★");
        }
    }

    public void StartGroggy()
    {
        StartCoroutine(GroggyRoutine());
    }

    IEnumerator GroggyRoutine()
    {
        isGroggy = true;
        totalFiredCount = 0;
        parryHitCount = 0;

        if (coverWall != null) coverWall.SetActive(false); // 벽 숨기기
        Debug.Log("!!! 보스 그로기 상태 발생 !!! 벽이 무너졌습니다! 근접 공격 가능!");

        // 그로기 시간 대기 (7초)
        yield return new WaitForSeconds(groggyDuration);

        // ★ 핵심: 벽이 생기기 전, 플레이어를 원래 자리로 3배 속도로 끌어옴
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.ReturnToInitialPosition();
        }

        // 복귀하는 데 걸리는 약간의 딜레이 후 벽 재생성
        yield return new WaitForSeconds(0.5f);

        if (coverWall != null) coverWall.SetActive(true); // 벽 다시 생성
        isGroggy = false;
        Debug.Log("보스가 정신을 차렸습니다. 벽이 다시 세워집니다.");
    }
}