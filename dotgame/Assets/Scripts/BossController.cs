using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform playerTransform;
    public Transform spawnPoint;
    public float attackInterval = 3.0f;
    public float arcHeight = 3.0f;

    void Start()
    {
        // Player를 인스펙터에서 안 넣었을 경우 자동으로 찾기
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj == null) playerObj = GameObject.Find("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);
            ShootBall();
        }
    }

    void ShootBall()
    {
    // 그로기 상태일 때는 공격 멈춤
        if (BossBattleManager.Instance != null && BossBattleManager.Instance.isGroggy) return;

        if (ballPrefab == null || playerTransform == null) return;

        Vector3 spawnPos = (spawnPoint != null) ? spawnPoint.position : transform.position;
        GameObject ball = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
        
        BossProjectile projectile = ball.GetComponent<BossProjectile>();
        if (projectile != null)
        {
            projectile.Launch(playerTransform.position, arcHeight);

            // ★ 매니저에 공 발사 카운트 전달!
            if (BossBattleManager.Instance != null)
            {
                BossBattleManager.Instance.RegisterBallFired();
            }
        }
    }
    // {
    //     if (ballPrefab == null)
    //     {
    //         Debug.LogError("[에러] BossController에 Ball Prefab이 연결되지 않았습니다!");
    //         return;
    //     }

    //     if (playerTransform == null)
    //     {
    //         Debug.LogError("[에러] PlayerTransform을 찾을 수 없습니다! Player 오브젝트의 이름이나 태그를 확인하세요.");
    //         return;
    //     }

    //     Vector3 spawnPos = (spawnPoint != null) ? spawnPoint.position : transform.position;
    //     GameObject ball = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
        
    //     BossProjectile projectile = ball.GetComponent<BossProjectile>();
    //     if (projectile != null)
    //     {
    //         projectile.Launch(playerTransform.position, arcHeight);
    //     }
    //     else
    //     {
    //         Debug.LogError("[에러] BossBall 프리팹에 BossProjectile 스크립트가 붙어있지 않습니다!");
    //     }
    // }
}