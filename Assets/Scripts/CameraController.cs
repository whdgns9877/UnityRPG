using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -10f);
    [SerializeField] private float smoothSpeed = 0.125f;

    private void LateUpdate()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>().transform;
            return;
        }

        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // 카메라가 항상 플레이어의 정면을 바라보도록 설정
        Vector3 lookAtPosition = player.position + player.forward * 5f; // 5f는 플레이어와의 거리를 나타냅니다. 조정 가능
        transform.LookAt(lookAtPosition);
    }
}
