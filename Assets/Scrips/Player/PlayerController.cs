using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;          // 기본 이동 속도
    public float jumpHeight = 1.2f;       // 점프 높이
    public float gravity = -9.81f;        // 중력 값

    [Header("대쉬 설정")]
    public float dashSpeed = 15f;         // 대쉬 속도
    public float dashDuration = 0.2f;     // 대쉬 지속 시간
    public float dashCooldown = 1f;       // 대쉬 쿨타임
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;

    [Header("공격 설정")]
    public float attackRange = 2f;        // 공격 범위
    public int attackDamage = 1;          // 공격 데미지
    public LayerMask attackLayer;         // 공격 대상 레이어

    [Header("카메라")]
    public Transform playerCamera;        // WorldManager에서 Main Camera 자동 연결

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 카메라가 비어 있으면 Main Camera 자동 연결
        if (playerCamera == null && Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        // 땅 체크
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 이동 입력
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // 대쉬 처리
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && dashCooldownTimer <= 0f)
        {
            StartCoroutine(Dash(move));
        }

        if (!isDashing)
        {
            controller.Move(move * moveSpeed * Time.deltaTime);
        }

        // 점프
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 공격 입력
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        // 대쉬 쿨타임 감소
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    // 공격 기능
    void Attack()
    {
        //Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        //if (Physics.Raycast(ray, out RaycastHit hit, attackRange, attackLayer))
        //{
        //    Debug.Log("Hit: " + hit.collider.name);

        //    // 공격 대상에 데미지 주기 (예시: Health 컴포넌트)
        //    Health target = hit.collider.GetComponent<Health>();
        //    if (target != null)
        //    {
        //        target.TakeDamage(attackDamage);
        //    }
        //}
    }

    // 대쉬 기능
    System.Collections.IEnumerator Dash(Vector3 moveDir)
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        while (dashTimer > 0f)
        {
            controller.Move(moveDir.normalized * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }
}