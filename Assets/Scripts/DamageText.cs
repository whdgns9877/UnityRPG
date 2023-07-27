using UnityEngine;

public class DamageText : MonoBehaviour
{
    // AnimatorCurve 컴포넌트를 이용하여 UI텍스트를 애니메이션처럼 사용
    [SerializeField] private AnimationCurve offsetCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 40f) });

    // 시작 시간 및 원래 위치와 스케일, 바뀔 위치와 스케일을 각각 초기화해준다.
    private float startTime = 0f;
    private Vector3 oriPos = Vector3.zero;
    private Vector3 curOffset = Vector3.one;

    private void Start()
    {
        oriPos = transform.position; // 원래 위치는 자기 자신의 위치값
        startTime = 0f; // 시작시간은 0
    }

    private void Update()
    {
        // 지정된 시간의 값을(그래프 참조) 가져온다
        curOffset.y = offsetCurve.Evaluate(startTime); // 0리턴

        // 위치 변경
        transform.position = oriPos + curOffset;

        // 시간 누적
        startTime += Time.deltaTime;

        // 지정된 시간만큼 반복
        if (offsetCurve.keys[offsetCurve.keys.Length - 1].time <= startTime)
        {
            startTime = 0f; // 시간초기화
            ObjectPool.Instacne.ReturnDamageTextToPool(gameObject); // 풀에 반환
        }
    }

    private void OnDisable()
    {
        transform.position = oriPos; // 위치 초기화
    }
}
