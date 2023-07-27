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

        // ī�޶� �׻� �÷��̾��� ������ �ٶ󺸵��� ����
        Vector3 lookAtPosition = player.position + player.forward * 5f; // 5f�� �÷��̾���� �Ÿ��� ��Ÿ���ϴ�. ���� ����
        transform.LookAt(lookAtPosition);
    }
}
