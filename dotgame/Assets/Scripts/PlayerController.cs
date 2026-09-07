using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 및 대시 설정")]
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;       // 대시 속도
    public float dashDuration = 0.2f;    // 대시 지속 시간
    public float doubleTapTime = 0.3f;   // 연타 인정 시간 (0.3초 이내 두 번 눌러야 대시)

    private Rigidbody rb;
    private Vector3 moveInput;
    private bool isDashing = false;
    private bool isDefending = false;   // 방어 상태 변수 추가

    // A/D 키 연타 감지용 변수
    private float lastTapTimeA = 0f;
    private float lastTapTimeD = 0f;

    [Header("보스전 설정")]
    public Transform bossTransform;
    private Vector3 initialPosition;
    private bool isBeingPulled = false;

    [Header("애니메이션 설정")]
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;

        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (isBeingPulled)
        {
            if (anim != null) anim.SetBool("isMoving", false);
            return;
        }

        // 대시 중일 때는 이동 및 다른 키 입력 차단
        if (isDashing) return;

        // 1. X키: 방어 상태 체크 (누르고 있는 동안)
        isDefending = Input.GetKey(KeyCode.X);
        if (anim != null)
        {
            anim.SetBool("isDefending", isDefending);
        }

        // ★ 방어 중일 때는 이동을 멈추고 공격/대시 등 다른 행동 차단
        if (isDefending)
        {
            moveInput = Vector3.zero;
            if (anim != null) anim.SetBool("isMoving", false);
            return; // 아래의 이동/대시/공격 코드를 실행하지 않고 리턴
        }

        // 2. 방향키 이동 입력
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(moveX, moveY, 0).normalized;

        // 3. A/D 키 연타(Double Tap) 대시 체크
        CheckDashInput();

        // 4. Z키: 공격 (공격 애니메이션 + 패링 + 보스 데미지)
        if (Input.GetKeyDown(KeyCode.Z))
        {
            AttackAction();
        }

        // 5. 애니메이션 처리
        HandleAnimation(moveX, moveY);
    }

    void FixedUpdate()
    {
        // 강제 복귀 중, 대시 중, 또는 방어 중일 때는 Rigidbody 이동 차단
        if (isBeingPulled || isDashing || isDefending) return;

        rb.velocity = moveInput * moveSpeed;
    }

    // --- 대시 감지 및 실행 ---
    void CheckDashInput()
    {
        // A 키 두 번 연타 (왼쪽 대시)
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Time.time - lastTapTimeA <= doubleTapTime)
            {
                StartCoroutine(DashRoutine(Vector3.left));
            }
            lastTapTimeA = Time.time;
        }

        // D 키 두 번 연타 (오른쪽 대시)
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (Time.time - lastTapTimeD <= doubleTapTime)
            {
                StartCoroutine(DashRoutine(Vector3.right));
            }
            lastTapTimeD = Time.time;
        }
    }

    IEnumerator DashRoutine(Vector3 dashDirection)
    {
        isDashing = true;

        // 대시 애니메이션 재생
        if (anim != null) anim.SetTrigger("doDash");

        // 대시 방향으로 반전(Flip) 처리
        if (spriteRenderer != null)
        {
            if (dashDirection.x > 0) spriteRenderer.flipX = false;
            else if (dashDirection.x < 0) spriteRenderer.flipX = true;
        }

        // 순간적인 속도 부여
        rb.velocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector3.zero;
        isDashing = false;
    }

    // --- 공격 / 패링 처리 ---
    void AttackAction()
    {
        if (anim != null) anim.SetTrigger("doAttack");

        Parry();
        MeleeAttack();
    }

    // --- 애니메이션 제어 함수 ---
    void HandleAnimation(float moveX, float moveY)
    {
        if (anim == null) return;

        bool isMoving = (moveX != 0 || moveY != 0);
        anim.SetBool("isMoving", isMoving);

        anim.SetFloat("moveX", moveX);
        anim.SetFloat("moveY", moveY);
        anim.SetFloat("absX", Mathf.Abs(moveX));

        if (spriteRenderer != null)
        {
            if (moveX > 0) spriteRenderer.flipX = false;
            else if (moveX < 0) spriteRenderer.flipX = true;
        }
    }

    // --- 강제 복귀 기능 ---
    public void ReturnToInitialPosition()
    {
        StartCoroutine(PullBackRoutine());
    }

    IEnumerator PullBackRoutine()
    {
        isBeingPulled = true;
        rb.velocity = Vector3.zero;

        float pullSpeed = moveSpeed * 3.0f;

        while (Vector3.Distance(transform.position, initialPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, pullSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = initialPosition;
        isBeingPulled = false;
    }

    // --- 패링 & 보스 공격 ---
    void Parry()
    {
        BossProjectile[] balls = FindObjectsOfType<BossProjectile>();
        foreach (BossProjectile ball in balls)
        {
            ball.TryParry(transform.position, bossTransform);
        }
    }

    void MeleeAttack()
    {
        if (BossBattleManager.Instance == null || bossTransform == null) return;

        if (BossBattleManager.Instance.isGroggy)
        {
            float distToBoss = Vector3.Distance(transform.position, bossTransform.position);
            if (distToBoss <= 3.0f)
            {
                BossBattleManager.Instance.TakeDamage(50);
            }
        }
    }
}