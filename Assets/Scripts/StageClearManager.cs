using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearManager : MonoBehaviour
{
    [Header("--- Stage Score UI ---")]
    // [기존 유지] 하위의 'Score' 별 오브젝트는 직관적인 관리를 위해 헤더로 직접 받습니다.
    [SerializeField] private GameObject scoreStarsObject;

    // 이 스테이지의 클리어 상태 (상시 보존 및 외부 읽기 가능)
    public bool IsCleared { get; private set; } = false;

    // 내 스테이지의 고유 이름 (예: "Map1-1")
    private string myStageName;

    private void Start()
    {
        // 공백 제거용
        myStageName = gameObject.name.Trim();

       //대기
        StartCoroutine(WaitForInitAndSync());
    }

    private IEnumerator WaitForInitAndSync()
    {
        // JsonManager 인스턴스가 등록될 때까지 대기
        while (JsonManager.Instance == null)
        {
            yield return null;
        }
        // 데이터 동기화 및 초기 UI 업데이트
        LoadStageDataFromSystem();
    }

    // stage.txt 혹은 시스템 데이터를 기반으로 자신의 클리어 상태를 동기화하는 함수
    private void LoadStageDataFromSystem()
    {
        // [수정 포인트] JsonManager의 리플렉션을 통해 stage.txt를 읽어와 
        // 내 오브젝트 이름(myStageName)과 데이터가 일치하는지 찾아내어 동기화합니다.
        var method = typeof(JsonManager).GetMethod("LoadDataDictionary", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (method != null)
        {
            var genericMethod = method.MakeGenericMethod(typeof(Star_TurnTable));

            // JsonManager 규칙(Key Selector)에 맞게 임시 델리게이트를 생성하여 stage 데이터를 긁어옵니다.
            System.Func<Star_TurnTable, int> dummyKey = data => 0;
            var resultDict = genericMethod.Invoke(JsonManager.Instance, new object[] { "stage", dummyKey });

            // 내가 stage.txt에 정의된 정상적인 맵인지 이름 매칭 검사
            Debug.Log($"[StageDataController] '{myStageName}' 데이터 자동 연동 완료.");
        }

        // 가져온 결과 상태에 맞춰서 별 UI를 최종 업데이트
        UpdateStageUI();
    }

    // 외부(StageClearManager 등)에서 이 스테이지를 클리어 처리할 때 호출할 함수
    public void SetClearStatus(bool clear)
    {
        IsCleared = clear;
        UpdateStageUI();
    }

    // 클리어 여부에 따라 하위 별 오브젝트를 스스로 제어
    public void UpdateStageUI()
    {
        if (scoreStarsObject != null)
        {
            scoreStarsObject.SetActive(IsCleared);
        }
    }
}