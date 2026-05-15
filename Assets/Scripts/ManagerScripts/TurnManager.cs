using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    // 현재 턴
    [SerializeField] private int turnCount = 0;
    // 최대 턴
    [SerializeField] private int maxTurn = 0;
    // Text
    [SerializeField] private TMP_Text turnText;
    // 별 리스트
    [SerializeField] private List<Image> stars;
    // 별 이펙트 리스트
    [SerializeField] private List<Image> starEffects;
    // 별 밝은 이미지
    [SerializeField] private Sprite starSprite;

    // 실질적인 스테이지 턴 데이터 테이블
    //[SerializeField] private List<Star_TurnTable> star_turnTable;

    // 현재 별 인덱스
    private int currentStarIndex = 0;
    // 별의 꺼짐 컬러값
    Color endColor = new Color32(58, 58, 58, 128);

    // 현재 별의 갯수
    private int currentStarCount = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    // 초기화 함수
    private void Initialize()
    {
        // 현재 라운드(Round Index)를 받아 해당 별에 대한 최대 턴 수를 받음.
        UpdateStageLimit(1);

        // TMP_Text 할당
        turnText = transform.Find("TurnText").GetComponent<TMP_Text>();
        // 턴 UPdata 메서드
        UpdateTurnDisplay();

        foreach (Transform child in transform)
        {
            if (child.name == "Star")
            {
                Image starImg = child.GetComponent<Image>();
                if (starImg != null)
                {
                    stars.Add(starImg);
                    // 별 바로 아래의 이펙트도 동시에 찾아서 리스트에 추가
                    Transform effectTransform = child.Find("StarEffect");
                    if (effectTransform != null)
                    {
                        starEffects.Add(effectTransform.GetComponent<Image>());
                    }
                }
            }
        }
    }

    // 현재 스테이지(Round Index) 별에 대한 최대 턴수 (Json 데이터를 받아서 쓰는 형식)
    private void UpdateStageLimit(int round)
    {

        // JsonManager의 딕셔너리에서 round(Key)으로 데이터(Value)를 즉시 찾음
        if (JsonManager.Instance.StarTurnDict.TryGetValue(round, out var data))
        {
            if (currentStarCount == 3)
                maxTurn = data.threeStarTurn;
            else
                maxTurn = data.twoStarTurn;
        }
    }


    // 플레이어 행동 기준으로 턴수를 증가시키는 메서드
    public void SetTurnCount()
    {
        turnCount++;
        UpdateTurnDisplay();
    }

    // 턴 UI 및 상태 갱신
    private void UpdateTurnDisplay()
    {
        // 별이 깎여야 하는 상황 체크 (현재 턴이 최대치를 넘고 별이 1개보다 많을 때)
        if (turnCount > maxTurn && currentStarCount > 1)
        {
            StartCoroutine(TurnStarUpdata());
            UpdateStageLimit(0); // 다음 별 등급 목표로 갱신
        }

        // UI 텍스트 업데이트
        if (currentStarCount > 1)
        {
            turnText.text = $"{turnCount} / {maxTurn}";
        }
        else
        {
            // 1성만 남았을 때는 분모(maxTurn)의 의미가 없으므로 빨간색 강조
            turnText.text = $"<color=red>{turnCount}</color> / {maxTurn}";
        }



    }
    // 별 애니메이션
    private IEnumerator TurnStarUpdata()
    {
        currentStarIndex = currentStarCount - 1;
        float elapsedTime = 0f; // 경과 시간 초기화
        float fadeDuration = 0.5f; // 개별 별이 사라지는 시간

        Color startColor = stars[currentStarIndex].color; // 컬러의 시작값

        Vector3 endScale = Vector3.one * 1.1f;  //커지는 스케일 값
        Sprite s = stars[currentStarIndex].sprite;  // 기존 Sprite 

        stars[currentStarIndex].sprite = starSprite;    // 밝은 Sprite 적용

        StarEffectOff();    // 별 이펙트 off

        currentStarCount--; // 현재 별의 갯수를 줄임

        // 커지는 단계
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            stars[currentStarIndex].transform.localScale = Vector3.Lerp(Vector3.one, endScale, elapsedTime);
            yield return null;
        }

        stars[currentStarIndex].sprite = s;
        // 작아지는 단계
        elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            stars[currentStarIndex].transform.localScale = Vector3.Lerp(endScale, Vector3.one, elapsedTime);
            stars[currentStarIndex].color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration); //시간에 따라 컬러를 서서히 변경(보간)

            yield return null;
        }

        stars[currentStarIndex].transform.localScale = Vector3.one;

    }

    // 별 이펙트 일괄 제거
    private void StarEffectOff()
    {
        if (starEffects.Count == 0) return;

        // 첫 번째 요소가 켜져 있을 때만 루프 실행 (최적화)
        if (starEffects[0].gameObject.activeSelf)
        {
            foreach (var effect in starEffects)
            {
                if (effect != null) effect.gameObject.SetActive(false);
            }
        }
    }
}
