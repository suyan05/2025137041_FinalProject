using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivityX = 2.0f;
    public float sensitivityY = 2.0f;

    public Transform playerBody; // WorldManager에서 자동 연결

    private float rotationX = 0f; // Pitch

    void Update()
    {
        if(playerBody == null)
        {
            return; // playerBody가 할당되지 않았으면 종료
        }

        float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

        // 좌우(Yaw)  Player 루트 회전
        playerBody.Rotate(Vector3.up * mouseX);

        // 상하(Pitch)  카메라 자체 회전
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -85f, 85f);

        transform.localEulerAngles = new Vector3(rotationX, 0f, 0f);
    }
}