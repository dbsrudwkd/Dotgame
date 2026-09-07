using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("몬스터 능력치")]
    public float speed = 3f;          // 이동 속도
    public bool isLive = true;         // 생존 여부

    [Header("추적 및 재배치 설정")]
    public Rigidbody target;          // 플레이어 Rigidbody
    public float despawnDistance = 12f; // 플레이어와 이 거리 이상 벌어지면 재배치 실행

    private Rigidbody rigid;
    private Reposition repo;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        repo = GetComponent<Reposition>();
    }

    void Start()
    {
        // "Player" 태그를 가진 오브젝트의 Rigidbody를 자동으로 찾아서 지정
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.GetComponent<Rigidbody>();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isLive || target == null) return;

        // 물리 충돌 후 속도 잔류로 인한 멈춤 현상 방지
        rigid.velocity = Vector3.zero;

        // 1. 방향 벡터 계산 (목표 위치 - 내 위치)
        Vector3 dirVec = target.position - rigid.position;
        dirVec.z = 0; // Z축 고정

        // 2. 물리 기반 이동
        Vector3 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);

        // 3. 거리 체크 및 재배치 실행 (테스트용)
        CheckDistanceAndReposition(dirVec);
    }

    void LateUpdate()
    {
        if (!isLive || target == null) return;

        // 플레이어 위치에 따른 좌우 시선 회전 (Y축 180도 회전)
        if (target.position.x < rigid.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); // 왼쪽
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);   // 오른쪽
        }
    }

    // 플레이어와의 거리를 측정하여 너무 멀어지면 Reposition 호출
    void CheckDistanceAndReposition(Vector3 dirVec)
    {
        if (repo == null) return;

        // 현재 플레이어와의 직선 거리 측정
        float currentDistance = dirVec.magnitude;

        if (currentDistance > despawnDistance)
        {
            // 플레이어 이동 방향 계산 (플레이어 속도 기반 또는 바라보는 방향)
            Vector3 playerDir = target.velocity.normalized;
            if (playerDir == Vector3.zero) playerDir = Vector3.right; // 멈춰있을 경우 기본 오른쪽 방향

            repo.RePositionEnemy(target.position, playerDir);
        }
    }
}