//using NUnit.Framework;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;

//public class PointMove : MonoBehaviour
//{
//    [Header("하이픈 오브젝트들")]
//    public RectTransform leftPointer;  // 왼쪽 하이픈
//    public RectTransform rightPointer; // 오른쪽 하이픈

//    [Header("설정")]
//    public float padding = 20f; // 버튼 글자와 하이픈 사이의 여유 공간
//    public float yOffset = 0f;


//    void Update()
//    {
//        // 현재 선택된 버튼 가져오기
//        GameObject selected = EventSystem.current.currentSelectedGameObject;

//        if (selected != null)
//        {
//            // 1. 버튼의 위치와 크기 정보(RectTransform) 가져오기
//            RectTransform buttonRect = selected.GetComponent<RectTransform>();

//            if (buttonRect != null)
//            {
//                // 2. 부모(SelectPoint)의 위치를 버튼 중앙으로 이동
//                transform.position = buttonRect.position;

//                // 3. 버튼의 가로 길이에 맞춰 하이픈 간격 조절
//                // 버튼 가로 길이의 절반(Half Width)만큼 양옆으로 밀어줍니다.
//                float halfWidth = (buttonRect.rect.width / 2) + padding;

//                leftPointer.anchoredPosition = new Vector2(-halfWidth, yOffset);
//                rightPointer.anchoredPosition = new Vector2(halfWidth, yOffset);

//                // 선택 사항: 부드러운 위아래 움직임 효과
//                float bounce = Mathf.Sin(Time.time * 10f) * 2f;
//                leftPointer.anchoredPosition += new Vector2(0, bounce);
//                rightPointer.anchoredPosition += new Vector2(0, bounce);
//            }
//        }
//    }
//}
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // TMP를 쓰기 위해 추가

public class PointMove : MonoBehaviour
{
    [Header("하이픈 오브젝트들")]
    public RectTransform leftPointer;
    public RectTransform rightPointer;

    [Header("설정")]
    public float padding = 10f; // 글자 끝에서 하이픈까지의 거리
    public float yOffset = 0f;

    private GameObject oldSelected;
    private TextMeshProUGUI btnText;
    void Update()
    {
        // 1. 이벤트 시스템 안전 체크
        if (EventSystem.current == null) return;

        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        // 2. 선택이 바뀌었을 때만 무거운 연산 수행
        if (oldSelected != selected)
        {
            oldSelected = selected;
            btnText = selected.GetComponentInChildren<TextMeshProUGUI>();

            if (btnText != null)
            {
                // 월드 좌표 대신 부모 관계를 고려한 위치 설정
                transform.position = btnText.transform.position;
            }
        }

        // 3. 컴포넌트들이 모두 있을 때만 실행 (매 프레임 실행되는 로직 최적화)
        if (btnText != null && leftPointer != null && rightPointer != null)
        {
            float textWidth = btnText.preferredWidth;
            float halfWidth = (textWidth / 2f) + padding;

            // Time.time 연산은 가볍지만, 혹시 모를 오버플로우 방지
            float bounce = Mathf.Sin(Time.time * 10f) * 2f;

            // Vector2 생성을 최소화하거나 필요한 경우만 할당
            Vector2 pos = new Vector2(halfWidth, yOffset + bounce);
            leftPointer.anchoredPosition = -pos;
            rightPointer.anchoredPosition = pos;
        }
    }
}