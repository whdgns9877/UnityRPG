using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private Slider PlayerHPBar;     // 플레이어의 체력을 표시하는 슬라이더
    [SerializeField] private Slider PlayerEXPBar;    // 플레이어의 경험치를 표시하는 슬라이더
    [SerializeField] private TMP_Text PlayerLvTxt;   // 플레이어의 레벨을 표시하는 텍스트
    [SerializeField] private GameObject DamageTxt;   // 데미지 텍스트 프리팹
    [SerializeField] private Canvas canvas = null;   // 데미지 텍스트를 품을 캔버스

    private Camera mainCam; // 메인 카메라의 참조

    private void Start()
    {
        // 메인 카메라의 참조를 가져옴
        mainCam = Camera.main;

        // UI 요소들이 항상 화면에 보이도록 카메라를 향하도록 설정
        PlayerHPBar.transform.LookAt(mainCam.transform);
        PlayerEXPBar.transform.LookAt(mainCam.transform);
        PlayerLvTxt.transform.LookAt(mainCam.transform);
    }

    // 플레이어의 체력 바를 주어진 값(val)으로 업데이트하는 메서드
    public void UpdateHPBar(float val)
    {
        PlayerHPBar.value = val;
    }

    // 플레이어의 경험치 바를 주어진 값(val)으로 업데이트하는 메서드
    // 만약 lvUp이 true이면 경험치 바를 0으로 초기화 (레벨업 표시)
    public void UpdateExpBar(float val, bool lvUp = false)
    {
        if (lvUp == true)
        {
            PlayerEXPBar.value = 0f;
            return;
        }
        PlayerEXPBar.value = val;
    }

    // 플레이어의 레벨 텍스트를 현재 레벨(curLevel)로 업데이트하는 메서드
    public void UpdateLevel(int curLevel)
    {
        PlayerLvTxt.text = $"LV {curLevel}";
    }

    public void ShowDamageText(float damageValue, Vector3 outputPos, Color color)
    {
        // 입력된 outputPos는 3D 공간 상의 좌표이므로
        // 2D UI 공간 상의 좌표로 변환

        // 카메라 기준으로 화면 좌표계로 변경
        Vector3 screenPos = mainCam.WorldToScreenPoint(outputPos);

        // outputPos 매개변수로 받은 위치를 카메라 기준 좌표로 받아 해당 위치에 회전 변환 없이 캔버스에 띄움
        GameObject damageTxtObj = Instantiate(DamageTxt, canvas.transform);
        damageTxtObj.transform.position = screenPos;
        damageTxtObj.transform.rotation = Quaternion.identity;
        TMP_Text instTxt = damageTxtObj.GetComponent<TMP_Text>();
        instTxt.text = damageValue.ToString();
        instTxt.color = color;
    }
}
