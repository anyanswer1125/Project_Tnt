using UnityEngine;
using UnityEngine.SceneManagement;

public class StageDataController : MonoBehaviour
{
    //[Header("--- Stage Configuration ---")]
    //[Tooltip("현재 클리어한 스테이지의 이름을 오브젝트명과 똑같이 적어주세요. (예: Map1-1)")]
    //[SerializeField] private string currentStageName = "Map1-1";

    //[Tooltip("엔터를 누르면 바로 실행될 다음 스테이지 씬 이름을 적어주세요. (예: Map1-2)")]
    //[SerializeField] private string nextStageName = "Map1-2";
    private MapController mapController;
    private string currentStageName;

    private void Start()
    {
        // 1. [자동화] 이 스크립트(또는 부모 UI)가 속한 오브젝트의 이름을 가져와 공백을 제거합니다.
        // 만약 이 스크립트가 클리어 팝업창 자식에 붙어있다면, 최상위 Map 오브젝트 이름을 가져오기 위해 
        // 필요에 따라 gameObject.name 대신 transform.root.name 등을 사용할 수도 있습니다.
        currentStageName = gameObject.name.Trim();

        // 씬 안에 살아있는 MapController를 탐색하여 가져옵니다.
        mapController = FindFirstObjectByType<MapController>();
    }

    private void Update()
    {
        // 1. 엔터키 입력 시 -> 다음 스테이지 씬 바로 실행
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // [자동화] 현재 활성화된 인게임 씬의 빌드 인덱스를 가져옵니다.
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // Build Profiles 상의 다음 씬 인덱스 계산 (current + 1)
            int nextSceneIndex = currentSceneIndex + 1;

            // 다음 인덱스가 총 등록된 씬 개수보다 작은지 안전하게 체크 후 로드
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                Debug.Log($"다음 씬(Index: {nextSceneIndex})으로 바로 넘어갑니다.");
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("Build Profiles에 더 이상 등록된 다음 씬이 없습니다!");
                // 마지막 씬일 경우 안전하게 메인 메뉴로 보내는 예외 처리
                if (mapController != null) mapController.ReturnToMain();
            }
        }

        // 2. ESC키 입력 시 -> 데이터를 넣고 메인화면으로 복귀
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log($"{currentStageName} 클리어 데이터를 저장하고 메인으로 돌아갑니다.");

            if (mapController != null)
            {
                // MapController에게 이 스테이지가 깨졌다고 통보합니다.
                mapController.RegisterStageClear(currentStageName);

                // 메인 메뉴 화면으로 복구시킵니다.
                mapController.ReturnToMain();
            }
            else
            {
                // 만약 타이틀/메인메뉴가 다른 씬으로 완전히 분리되어 있다면 해당 씬 로드
                SceneManager.LoadScene("MainMenuScene");
            }
        }
    }
}