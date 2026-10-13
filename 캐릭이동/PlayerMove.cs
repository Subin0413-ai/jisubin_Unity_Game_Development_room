using UnityEngine;
using UnityEngine.InputSystem; // 키보드 입력(Keyboard)을 쓰기 위해 필요

// Player 오브젝트에 붙여서 사용합니다.
// 역할: WASD 키로 "한 칸씩" 이동 (쯔꾸르 방식, 4방향)
public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5.5f;          // 걷는 속도 (Inspector에서 변경 가능)

    private bool isMoving = false;          // 지금 한 칸 걷는 중인가?
    private Vector3 targetPos;              // 도착할 목표 칸의 좌표
    private Vector2 bufferedDir = Vector2.zero; // 걷는 도중 눌린 키를 잠깐 기억

    // 매 프레임 자동으로 실행됨
    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return; // 키보드가 없으면 아무것도 안 함

        // 걷는 중: 새로 눌린 키를 기억해 두고, 목표 칸으로 한 걸음 더 간다
        if (isMoving)
        {
            Vector2 pressed = GetNewlyPressedDir(kb);
            if (pressed != Vector2.zero) bufferedDir = pressed;

            MoveToTarget();
        }

        // 멈춰 있음 (방금 도착한 경우 포함): 다음 칸으로 갈지 결정
        if (!isMoving)
        {
            Vector2 dir = GetHeldDir(kb);               // 누르고 있는 키 우선
            if (dir == Vector2.zero) dir = bufferedDir; // 없으면 기억해 둔 키
            bufferedDir = Vector2.zero;                 // 기억은 한 번 쓰면 비움

            if (dir != Vector2.zero)
            {
                targetPos = transform.position + (Vector3)dir; // 한 칸 앞이 목표
                isMoving = true;
            }
        }
    }

    // 지금 "누르고 있는" 키의 방향 (else 없이 return이라 한 방향만 -> 대각선 불가)
    Vector2 GetHeldDir(Keyboard kb)
    {
        if (kb.wKey.isPressed) return Vector2.up;
        if (kb.sKey.isPressed) return Vector2.down;
        if (kb.aKey.isPressed) return Vector2.left;
        if (kb.dKey.isPressed) return Vector2.right;
        return Vector2.zero;
    }

    // 이번 프레임에 "막 눌린" 키의 방향 (톡 누른 입력을 놓치지 않기 위함)
    Vector2 GetNewlyPressedDir(Keyboard kb)
    {
        if (kb.wKey.wasPressedThisFrame) return Vector2.up;
        if (kb.sKey.wasPressedThisFrame) return Vector2.down;
        if (kb.aKey.wasPressedThisFrame) return Vector2.left;
        if (kb.dKey.wasPressedThisFrame) return Vector2.right;
        return Vector2.zero;
    }

    // 목표 칸을 향해 조금씩 이동
    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (transform.position == targetPos)
        {
            isMoving = false; // 도착 -> 다음 입력을 받을 수 있게 됨
        }
    }
}
