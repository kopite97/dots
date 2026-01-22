using UnityEngine;
using UnityEngine.InputSystem; // ★ 필수 네임스페이스

public class CameraMover : MonoBehaviour
{
    [Header("설정")]
    public float moveSpeed = 10f;
    public float shiftMultiplier = 2f;
    public float mouseSensitivity = 0.5f; // 새 시스템은 감도가 다르니 좀 낮춥니다

    void Update()
    {
        // 키보드나 마우스가 연결 안 되어있으면 실행 안 함
        if (Keyboard.current == null || Mouse.current == null) return;

        // 1. 이동 (WASD)
        float h = 0f;
        float v = 0f;
        float y = 0f;

        // Input.GetKey -> Keyboard.current.xKey.isPressed
        if (Keyboard.current.wKey.isPressed) v = 1f;
        if (Keyboard.current.sKey.isPressed) v = -1f;
        if (Keyboard.current.aKey.isPressed) h = -1f;
        if (Keyboard.current.dKey.isPressed) h = 1f;
        if (Keyboard.current.eKey.isPressed) y = 1f;
        if (Keyboard.current.qKey.isPressed) y = -1f;

        // Shift 가속
        float currentSpeed = moveSpeed;
        if (Keyboard.current.shiftKey.isPressed) currentSpeed *= shiftMultiplier;

        Vector3 moveDir = transform.right * h + transform.forward * v + transform.up * y;
        transform.position += moveDir * currentSpeed * Time.deltaTime;

        // 2. 회전 (마우스 우클릭)
        if (Mouse.current.rightButton.isPressed)
        {
            // Input.GetAxis -> Mouse.current.delta.x.ReadValue()
            float mouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity;
            float mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity;

            Vector3 rot = transform.localEulerAngles;
            rot.y += mouseX;
            rot.x -= mouseY;
            transform.localEulerAngles = rot;
        }
    }
}