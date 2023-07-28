using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private Slider PlayerHPBar;
    [SerializeField] private Slider PlayerEXPBar;
    [SerializeField] private TMP_Text PlayerLvTxt;

    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
        PlayerHPBar.transform.LookAt(mainCam.transform);
        PlayerEXPBar.transform.LookAt(mainCam.transform);
        PlayerLvTxt.transform.LookAt(mainCam.transform);
    }

    public void UpdateHPBar(float val)
    {
        PlayerHPBar.value = val;
    }

    public void UpdateExpBar(float val, bool lvUp = false)
    {
        if(lvUp == true)
        {
            PlayerEXPBar.value = 0f;
            return;
        }
        PlayerEXPBar.value = val;
    }

    public void UpdateLevel(int curLevle)
    {
        PlayerLvTxt.text = $"LV {curLevle}";
    }

    public void ShowDamageText(float damageValue, Vector3 outputPos, Color color)
    {
        // 입력된 outputPos는 3D 공간 상의 좌표 이므로
        // 2D UI 공간 상의 좌표로 변환

        // 카메라 기준으로 화면 좌표계로 변경
        Vector3 screenPos = mainCam.WorldToScreenPoint(outputPos);

        // outputPos 매개변수로 받은 위치를 카메라 기준 좌표로 받아 해당위치에 회전변경 없이 캔버스에 띄움
        GameObject damageTxtObj = ObjectPool.Instacne.GetDamageTextFromPool();
        damageTxtObj.transform.position = screenPos;
        damageTxtObj.transform.rotation = Quaternion.identity;
        TMP_Text instTxt = damageTxtObj.GetComponent<TMP_Text>();
        instTxt.text = damageValue.ToString();
        instTxt.color = color;
    }
}
