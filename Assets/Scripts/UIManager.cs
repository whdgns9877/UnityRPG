using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private Slider PlayerHPBar;     // �÷��̾��� ü���� ǥ���ϴ� �����̴�
    [SerializeField] private Slider PlayerEXPBar;    // �÷��̾��� ����ġ�� ǥ���ϴ� �����̴�
    [SerializeField] private TMP_Text PlayerLvTxt;   // �÷��̾��� ������ ǥ���ϴ� �ؽ�Ʈ
    [SerializeField] private GameObject DamageTxt;   // ������ �ؽ�Ʈ ������
    [SerializeField] private Canvas canvas = null;   // ������ �ؽ�Ʈ�� ǰ�� ĵ����

    private Camera mainCam; // ���� ī�޶��� ����

    private void Start()
    {
        // ���� ī�޶��� ������ ������
        mainCam = Camera.main;

        // UI ��ҵ��� �׻� ȭ�鿡 ���̵��� ī�޶� ���ϵ��� ����
        PlayerHPBar.transform.LookAt(mainCam.transform);
        PlayerEXPBar.transform.LookAt(mainCam.transform);
        PlayerLvTxt.transform.LookAt(mainCam.transform);
    }

    // �÷��̾��� ü�� �ٸ� �־��� ��(val)���� ������Ʈ�ϴ� �޼���
    public void UpdateHPBar(float val)
    {
        PlayerHPBar.value = val;
    }

    // �÷��̾��� ����ġ �ٸ� �־��� ��(val)���� ������Ʈ�ϴ� �޼���
    // ���� lvUp�� true�̸� ����ġ �ٸ� 0���� �ʱ�ȭ (������ ǥ��)
    public void UpdateExpBar(float val, bool lvUp = false)
    {
        if (lvUp == true)
        {
            PlayerEXPBar.value = 0f;
            return;
        }
        PlayerEXPBar.value = val;
    }

    // �÷��̾��� ���� �ؽ�Ʈ�� ���� ����(curLevel)�� ������Ʈ�ϴ� �޼���
    public void UpdateLevel(int curLevel)
    {
        PlayerLvTxt.text = $"LV {curLevel}";
    }

    public void ShowDamageText(float damageValue, Vector3 outputPos, Color color)
    {
        // �Էµ� outputPos�� 3D ���� ���� ��ǥ�̹Ƿ�
        // 2D UI ���� ���� ��ǥ�� ��ȯ

        // ī�޶� �������� ȭ�� ��ǥ��� ����
        Vector3 screenPos = mainCam.WorldToScreenPoint(outputPos);

        // outputPos �Ű������� ���� ��ġ�� ī�޶� ���� ��ǥ�� �޾� �ش� ��ġ�� ȸ�� ��ȯ ���� ĵ������ ���
        GameObject damageTxtObj = Instantiate(DamageTxt, canvas.transform);
        damageTxtObj.transform.position = screenPos;
        damageTxtObj.transform.rotation = Quaternion.identity;
        TMP_Text instTxt = damageTxtObj.GetComponent<TMP_Text>();
        instTxt.text = damageValue.ToString();
        instTxt.color = color;
    }
}
