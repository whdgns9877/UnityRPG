using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private Image PlayerHPBarBackGround;
    [SerializeField] private Image PlayerHPBar;
    [SerializeField] private Image PlayerEXPBar;
    [SerializeField] private TMP_Text PlayerLvTxt;

    private void Start()
    {
        PlayerHPBarBackGround.transform.LookAt(Camera.main.transform);
        PlayerHPBar.transform.LookAt(Camera.main.transform);
        PlayerEXPBar.transform.LookAt(Camera.main.transform);
        PlayerLvTxt.transform.LookAt(Camera.main.transform);
    }

    public void UpdateHPBar(float val)
    {
        PlayerHPBar.fillAmount = val;
    }

    public void UpdateExpBar(float val, bool lvUp = false)
    {
        if(lvUp == true)
        {
            PlayerEXPBar.fillAmount = 0f;
            return;
        }
        PlayerEXPBar.fillAmount = val;
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
        Vector3 screenPos = Camera.main.WorldToScreenPoint(outputPos);

        // outputPos �Ű������� ���� ��ġ�� ī�޶� ���� ��ǥ�� �޾� �ش���ġ�� ȸ������ ���� ĵ������ ���
        GameObject damageTxtObj = ObjectPool.Instacne.GetDamageTextFromPool();
        damageTxtObj.transform.position = screenPos;
        damageTxtObj.transform.rotation = Quaternion.identity;
        TMP_Text instTxt = damageTxtObj.GetComponent<TMP_Text>();
        instTxt.text = damageValue.ToString();
        instTxt.color = color;
    }
}
