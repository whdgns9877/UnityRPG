using System.IO;
using UnityEngine;
using OfficeOpenXml;

public class ExcelDataReader : MonoBehaviour
{
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private PlayerData playerData;

    // 엑셀 파일의 경로
    private string excelFilePath = "Assets/Resources/DataTable.xlsx";

    private void Start()
    {
        // 엑셀 파일 읽기
        ReadExcelFile();
    }

    private void ReadExcelFile()
    {
        FileInfo fileInfo = new FileInfo(excelFilePath);
        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
            // 몬스터 데이터 테이블 시트 읽기
            ReadMonsterData(package.Workbook.Worksheets["MonsterDataTable"]);

            // 플레이어 데이터 테이블 시트 읽기
            ReadPlayerData(package.Workbook.Worksheets["PlayerDataTable"]);
        }

        Debug.Log("엑셀 데이터 읽기 완료");
    }

    private void ReadMonsterData(ExcelWorksheet worksheet)
    {
        // 데이터가 있는 첫 번째 행 (1행은 헤더이므로 2행부터 읽어옴)
        int startRow = 2;
        int endRow = worksheet.Dimension.End.Row; // 데이터가 끝나는 행

        for (int row = startRow; row <= endRow; row++)
        {
            float.TryParse(worksheet.Cells[row, 1].Value?.ToString(), out float spawnTime);
            int.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out int maxMonsterCount);
            int.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out int maxHp);
            int.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out int attackPower);
            float.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out float attackRate);
            float.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out float attackRange);

            // 스크립터블 오브젝트의 값을 변경
            monsterData.SpawnTime = spawnTime;
            monsterData.MaxMonsterCount = maxMonsterCount;
            monsterData.MaxHp = maxHp;
            monsterData.AttackPower = attackPower;
            monsterData.AttackRate = attackRate;
            monsterData.AttackRange = attackRange;
        }
    }

    private void ReadPlayerData(ExcelWorksheet worksheet)
    {
        // 데이터가 있는 첫 번째 행 (1행은 헤더이므로 2행부터 읽어옴)
        int startRow = 2;
        int endRow = worksheet.Dimension.End.Row; // 데이터가 끝나는 행

        for (int row = startRow; row <= endRow; row++)
        {
            float.TryParse(worksheet.Cells[row, 1].Value?.ToString(), out float hp);
            int.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out int attackPower);
            int.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out int attackRate);
            float.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out float attackRange);
            float.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out float skillAttackRange);

            // 스크립터블 오브젝트의 값을 변경
            playerData.Hp = hp;
            playerData.AttackPower = attackPower;
            playerData.AttackRate = attackRate;
            playerData.AttackRange = attackRange;
            playerData.SkillAttackRange = skillAttackRange;
        }
    }
}
