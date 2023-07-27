using System.IO;
using UnityEngine;
using OfficeOpenXml;

public class ExcelDataReader : MonoBehaviour
{
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private PlayerData playerData;

    // ���� ������ ���
    private string excelFilePath = "Assets/Resources/DataTable.xlsx";

    private void Start()
    {
        // ���� ���� �б�
        ReadExcelFile();
    }

    private void ReadExcelFile()
    {
        FileInfo fileInfo = new FileInfo(excelFilePath);
        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
            // ���� ������ ���̺� ��Ʈ �б�
            ReadMonsterData(package.Workbook.Worksheets["MonsterDataTable"]);

            // �÷��̾� ������ ���̺� ��Ʈ �б�
            ReadPlayerData(package.Workbook.Worksheets["PlayerDataTable"]);
        }

        Debug.Log("Success Read Data");
    }

    private void ReadMonsterData(ExcelWorksheet worksheet)
    {
        // �����Ͱ� �ִ� ù ��° �� (1���� ����̹Ƿ� 2����� �о��)
        int startRow = 2;
        int endRow = worksheet.Dimension.End.Row; // �����Ͱ� ������ ��

        for (int row = startRow; row <= endRow; row++)
        {
            float.TryParse(worksheet.Cells[row, 1].Value?.ToString(), out float spawnTime);
            int.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out int maxMonsterCount);
            int.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out int maxHp);
            int.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out int attackPower);
            float.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out float attackRate);
            float.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out float attackRange);
            int.TryParse(worksheet.Cells[row, 7].Value?.ToString(), out int getExp);

            // ��ũ���ͺ� ������Ʈ�� ���� ����
            monsterData.SpawnTime = spawnTime;
            monsterData.MaxMonsterCount = maxMonsterCount;
            monsterData.MaxHp = maxHp;
            monsterData.AttackPower = attackPower;
            monsterData.AttackRate = attackRate;
            monsterData.AttackRange = attackRange;
            monsterData.GetExp = getExp;
        }
    }

    private void ReadPlayerData(ExcelWorksheet worksheet)
    {
        // �����Ͱ� �ִ� ù ��° �� (1���� ����̹Ƿ� 2����� �о��)
        int startRow = 2;
        int endRow = worksheet.Dimension.End.Row; // �����Ͱ� ������ ��

        for (int row = startRow; row <= endRow; row++)
        {
            float.TryParse(worksheet.Cells[row, 1].Value?.ToString(), out float hp);
            int.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out int attackPower);
            int.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out int attackRate);
            float.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out float attackRange);
            float.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out float skillAttackRange);
            int.TryParse((worksheet.Cells[row, 6].Value)?.ToString(), out int requireExp4LvUp);

            // ��ũ���ͺ� ������Ʈ�� ���� ����
            playerData.Hp = hp;
            playerData.AttackPower = attackPower;
            playerData.AttackRate = attackRate;
            playerData.AttackRange = attackRange;
            playerData.SkillAttackRange = skillAttackRange;
            playerData.RequireEXP4LvUp = requireExp4LvUp;
        }
    }
}
