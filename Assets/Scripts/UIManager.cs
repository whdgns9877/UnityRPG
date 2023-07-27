using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    public Image PlayerHPBar;
    public Image PlayerEXPBar;
    public TMP_Text PlayerLvTxt;

    private void Awake()
    {
        PlayerHPBar.transform.LookAt(Camera.main.transform);
        PlayerEXPBar.transform.LookAt(Camera.main.transform);
        PlayerLvTxt.transform.LookAt(Camera.main.transform);
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
}
