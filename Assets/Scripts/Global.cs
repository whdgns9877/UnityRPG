using System.Collections.Generic;
using UnityEngine;

public class Global : MonoSingleton<Global>
{
    // �÷��̾ ����� Ȱ��ȭ�� ���� ��Ʈ��
    public List<GameObject> ActiveTargets;

    private void Awake()
    {
        ActiveTargets = new List<GameObject> ();
    }

    // ���͸� ����Ʈ�� �߰�
    public void AddTarget(GameObject target) => ActiveTargets.Add(target);
    // ���͸� ����Ʈ���� ����
    public void RemoveTarget(GameObject target) => ActiveTargets.Remove(target);
}
