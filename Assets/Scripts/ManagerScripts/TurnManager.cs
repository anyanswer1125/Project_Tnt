using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    // 남은 턴
    [SerializeField] private int turnCount = 10;
    [SerializeField] private TMP_Text turnText;

    [SerializeField] private int currentStageIndex;

    public static TurnManager instance;
    public int TurnCount => turnCount;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    // 초기화 함수
    private void Initialize()
    {
        // TMP_Text 할당
        turnText = transform.Find("TurnText").GetComponent<TMP_Text>();
        currentStageIndex = SceneManager.GetActiveScene().buildIndex;
        if (JsonManager.Instance.StageTurnDict.TryGetValue(currentStageIndex, out var data))
        {
            turnCount = data.Turns;
        }
        // 턴 UPdata 메서드
        UpdateTurnDisplay();
    }


    // 턴수를 증가시키는 메서드
    public void SetTurnCount()
    {
        turnCount--;
        UpdateTurnDisplay();
        Debug.Log(turnCount);
    }

    // 턴 UI 및 상태 갱신
    private void UpdateTurnDisplay()
    {
        if (turnCount > 5)
        {
            turnText.text = $"<size=120%>{turnCount}</size> 턴 남음";
        }
        else
            turnText.text = $"<color=red><size=120%>{turnCount}</size></color> 턴 남음";

    }
}
