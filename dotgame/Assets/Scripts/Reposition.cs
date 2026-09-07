using UnityEngine;

public class Reposition : MonoBehaviour
{
    private Collider coll;

    void Awake()
    {
        coll = GetComponent<Collider>();
    }

    // 플레이어 전방 위치로 몬스터 재배치
    public void RePositionEnemy(Vector3 playerPosition, Vector3 playerDirection)
    {
        // 몬스터가 죽은 상태(콜라이더 비활성화)라면 재배치하지 않음
        if (coll != null && !coll.enabled) return;

        Vector3 oldPos = transform.position;

        // ★ 테스트를 위해 재배치 거리를 7m(가까운 거리)로 변경
        // 무작위 오프셋 범주도 ±1.5m로 축소
        Vector3 randomOffset = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0f);

        // 새로운 재배치 위치 = 플레이어 위치 + (진행 방향 * 7m) + 랜덤 위치
        Vector3 newPos = playerPosition + (playerDirection * 7f) + randomOffset;

        // 1. 순간이동 처리
        transform.position = newPos;

        // 2. [테스트용] Scene 뷰에서 이전 위치 -> 새 위치로 빨간선 그리기 (3초간 유지)
        Debug.DrawLine(oldPos, newPos, Color.red, 3.0f);

        // 3. [테스트용] 콘솔 창에 메시지 출력
        Debug.Log($"[재배치 완료] {gameObject.name} 이(가) 플레이어 근처({newPos})로 이동했습니다.");
    }
}