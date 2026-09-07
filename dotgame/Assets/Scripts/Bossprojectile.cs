using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 targetPos;
    private float arcHeight = 3.0f;
    private float duration = 1.2f;
    private float timer = 0f;
    
    private bool isFlying = false;
    private bool isReflected = false; // 패링되어 보스에게 돌아가는 중인가?

    public void Launch(Vector3 target, float height)
    {
        startPos = transform.position;
        targetPos = target;
        targetPos.z = startPos.z;
        arcHeight = height;
        timer = 0f;
        isFlying = true;
    }

    void Update()
    {
        if (!isFlying) return;

        timer += Time.deltaTime;
        float progress = timer / duration;

        if (progress >= 1.0f)
        {
            // 보스에게 성공적으로 반사되어 도착한 경우
            if (isReflected)
            {
                if (BossBattleManager.Instance != null)
                    BossBattleManager.Instance.RegisterParryHit();
            }
            Destroy(gameObject);
            return;
        }

        Vector3 current = Vector3.Lerp(startPos, targetPos, progress);
        current.y += Mathf.Sin(progress * Mathf.PI) * arcHeight;
        transform.position = current;
    }

    // 플레이어가 패링 키를 눌렀을 때 호출
    public void TryParry(Vector3 playerPos, Transform bossTransform)
    {
        if (isReflected) return; // 이미 반사된 공은 중복 패링 불가

        float distance = Vector3.Distance(transform.position, playerPos);

        // 패링 성공 범위 (거리 1.5 이내)
        if (distance <= 1.5f)
        {
            Debug.Log("[패링 성공!] 공이 보스에게 반사됩니다!");
            isReflected = true;

            // 보스를 향해 다시 날아가도록 목표 및 위치 재설정
            startPos = transform.position;
            targetPos = bossTransform.position;
            targetPos.z = startPos.z;
            timer = 0f;
            duration = 0.6f; // 반사 공은 더 빠르게 날아감
            arcHeight = 2.0f;
        }
        else if (distance <= 3.0f)
        {
            // 빗맞은 패링 (엉뚱한 곳으로 날아감)
            Debug.Log("[패링 빗맞음!] 공이 엉뚱한 곳으로 튕깁니다!");
            isFlying = false;
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.velocity = new Vector3(-3f, 8f, 0f); // 대각선 위로 튕겨 나감
            }
            Destroy(gameObject, 1.5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 보스/스폰지점은 평소엔 무시 (단, 패링되어 돌아온 공이 보스에 닿으면 피격)
        if (other.CompareTag("Boss"))
        {
            if (isReflected)
            {
                if (BossBattleManager.Instance != null)
                    BossBattleManager.Instance.RegisterParryHit();
                Destroy(gameObject);
            }
            return;
        }

        if (other.name.Contains("SpawnPoint")) return;

        // 패링된 공은 벽을 무시함! 패링 안 된 기본 공만 벽/플레이어에 부딪혀 삭제
        if (!isReflected && (other.CompareTag("Wall") || other.CompareTag("Player")))
        {
            Destroy(gameObject);
        }
    }
}