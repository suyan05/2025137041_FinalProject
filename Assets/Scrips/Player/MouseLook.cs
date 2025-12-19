using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("마우스 설정")]
    public float sensitivityX = 2.0f;
    public float sensitivityY = 2.0f;
    public bool invertY = false; // 상하 반전 옵션

    public Transform playerBody; // WorldManager에서 자동 연결

    private float rotationX = 0f; // Pitch

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (playerBody == null) return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

        // 좌우(Yaw) - 플레이어 루트 회전
        playerBody.Rotate(Vector3.up * mouseX);

        // 상하(Pitch) - 카메라 자체 회전
        rotationX -= invertY ? -mouseY : mouseY;
        rotationX = Mathf.Clamp(rotationX, -85f, 85f);

        transform.localEulerAngles = new Vector3(rotationX, 0f, 0f);
    }
}