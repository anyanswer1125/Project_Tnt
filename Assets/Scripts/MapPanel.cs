using UnityEngine;
using TMPro;

public class MapStage : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject starUI; // 클리어 시 켜줄 별 이미지/오브젝트
    public TextMeshProUGUI stageText; // "- 1-1 -" 등이 적힌 텍스트

    private string stageID; // 데이터 키로 사용할 순수 이름 (예: 1-1)

    // 오브젝트가 켜지는 순간(SetActive(true)) 자동으로 실행됨
    private void OnEnable()
    {
        UpdateStageInfo();
    }

    public void UpdateStageInfo()
    {
        if (stageText == null) return;

        // 1. 텍스트에서 순수 이름 추출 (예: "- 1-1 -" -> "1-1")
        string[] lines = stageText.text.Split('\n');
        stageID = lines[0].Replace("-", "").Trim();

        // 2. 저장된 데이터 확인 (PlayerPrefs 사용 예시)
        // 키값 예: "Clear_1-1", "Clear_1-2"
        int isCleared = PlayerPrefs.GetInt("Clear_" + stageID, 0);

        // 3. 별 UI 활성화 여부 결정
        if (starUI != null)
        {
            starUI.SetActive(isCleared == 1);
        }

        Debug.Log($"스테이지 {stageID} 로드 완료. 클리어 여부: {isCleared}");
    }
}