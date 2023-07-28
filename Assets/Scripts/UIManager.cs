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
        // �Էµ� outputPos�� 3D ���� ���� ��ǥ �̹Ƿ�
        // 2D UI ���� ���� ��ǥ�� ��ȯ

        // ī�޶� �������� ȭ�� ��ǥ��� ����
        Vector3 screenPos = mainCam.WorldToScreenPoint(outputPos);

        // outputPos �Ű������� ���� ��ġ�� ī�޶� ���� ��ǥ�� �޾� �ش���ġ�� ȸ������ ���� ĵ������ ���
        GameObject damageTxtObj = ObjectPool.Instacne.GetDamageTextFromPool();
        damageTxtObj.transform.position = screenPos;
        damageTxtObj.transform.rotation = Quaternion.identity;
        TMP_Text instTxt = damageTxtObj.GetComponent<TMP_Text>();
        instTxt.text = damageValue.ToString();
        instTxt.color = color;
    }
}
