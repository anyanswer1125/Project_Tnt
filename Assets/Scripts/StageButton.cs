using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필수
using TMPro;
public class StageButton : MonoBehaviour
{
    public LevelData data; // 아까 만든 데이터를 넣을 칸
    public TextMeshProUGUI levelText; // 스테이지 번호를 표시할 텍스트

    void Start()
    {
        if (data != null)
        {
            levelText.text = data.levelName; // 데이터에 적힌 이름을 텍스트에 적용
        }
    }

    // 버튼을 눌렀을 때 실행될 함수
    public void OnClickButton()
    {
        if (data.isUnlocked)
        {
            SceneManager.LoadScene(data.sceneName); // 데이터에 적힌 신으로 이동
        }
    }
}
