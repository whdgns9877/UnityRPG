using UnityEngine;

public class DamageText : MonoBehaviour
{
    // AnimatorCurve ������Ʈ�� �̿��Ͽ� UI�ؽ�Ʈ�� �ִϸ��̼�ó�� ���
    [SerializeField] private AnimationCurve offsetCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 40f) });

    // ���� �ð� �� ���� ��ġ�� ������, �ٲ� ��ġ�� �������� ���� �ʱ�ȭ���ش�.
    private float startTime = 0f;
    private Vector3 oriPos = Vector3.zero;
    private Vector3 curOffset = Vector3.one;

    private void Start()
    {
        oriPos = transform.position; // ���� ��ġ�� �ڱ� �ڽ��� ��ġ��
        startTime = 0f; // ���۽ð��� 0
    }

    private void Update()
    {
        // ������ �ð��� ����(�׷��� ����) �����´�
        curOffset.y = offsetCurve.Evaluate(startTime); // 0����

        // ��ġ ����
        transform.position = oriPos + curOffset;

        // �ð� ����
        startTime += Time.deltaTime;

        // ������ �ð���ŭ �ݺ�
        if (offsetCurve.keys[offsetCurve.keys.Length - 1].time <= startTime)
        {
            startTime = 0f; // �ð��ʱ�ȭ
            ObjectPool.Instacne.ReturnDamageTextToPool(gameObject); // Ǯ�� ��ȯ
        }
    }

    private void OnDisable()
    {
        transform.position = oriPos; // ��ġ �ʱ�ȭ
    }
}
