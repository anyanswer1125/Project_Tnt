using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InGameClearHandler : MonoBehaviour
{
    [Header("--- Clear UI ---")]
    [SerializeField] private GameObject clearPanel; // 제작 예정인 클리어 패널
    [SerializeField] private float delayTime = 2.0f;

    private bool _isClearActive = false;

    // 이미 제작된 컷씬 애니메이션이 끝날 때 이 메서드를 호출하도록 설정
    public void StartClearSequence()
    {
        StartCoroutine(ClearRoutine());
    }

    private IEnumerator ClearRoutine()
    {
        yield return new WaitForSeconds(delayTime);
        clearPanel.SetActive(true);
        _isClearActive = true;
    }

    private void Update()
    {
        if (!_isClearActive) return;

        // Enter: 다음 씬 로드
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            LoadNextLevel();
        }
        // Escape: 맵 패널로 돌아가기 (로직상 메인 메뉴 씬으로 이동)
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPrefs.SetInt("StartInMapPanel", 1);
            SceneManager.LoadScene("MainMenuSceneName");
            // 주의: 메인 메뉴 씬이 로드될 때 '맵 패널'이 즉시 뜨게 하려면 
            // PlayerPrefs 등을 활용해 상태를 저장하고 MainMenuController에서 체크해야 합니다.
        }
    }

    private void LoadNextLevel()
    {
        string currentName = SceneManager.GetActiveScene().name; // 예: "1-1"

        // 1. '-'를 기준으로 문자열을 나눕니다. (1-1 -> [1, 1])
        string[] parts = currentName.Split('-');

        if (parts.Length != 2) // 이름 규칙이 틀릴 경우를 대비한 예외처리
        {
            SceneManager.LoadScene("MainMenuScene");
            return;
        }

        // 2. 숫자로 변환
        int world = int.Parse(parts[0]);
        int stage = int.Parse(parts[1]);

        // 3. 로직 적용 (세부 스테이지가 2까지 있다면)
        if (stage < 2)
        {
            stage++; // 1-1 -> 1-2
        }
        else
        {
            world++; // 1-2 -> 2-1
            stage = 1;
        }

        string nextSceneName = $"{world}-{stage}";

        // 4. 예외처리: 다음 씬이 빌드 설정에 있는지 확인 후 로드
        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            // 다음 월드가 없으면 (예: 4-2 클리어 후) 메인으로
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}