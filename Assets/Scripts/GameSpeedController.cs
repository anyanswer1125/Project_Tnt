using UnityEngine;

public class GameSpeedController : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private float speedStep = 0.25f; // 증감폭
    [SerializeField] private float minSpeed = 0f;    // 최소 속도 (정지)
    [SerializeField] private float maxSpeed = 3f;    // 최대 속도 (권장)

    void Update()
    {
        // '>' 키 (실제로는 Shift + . 이지만 유니티는 KeyCode.Greater 등으로 인식하거나 일반 문자로 처리 가능)
        // 여기서는 직관적으로 콤마(,)와 마침표(.) 키를 기준으로 작성했습니다.
        
        // 속도 감소 (< 키 또는 , 키)
        if (Input.GetKeyDown(KeyCode.Comma) || Input.GetKeyDown(KeyCode.Less))
        {
            AdjustSpeed(-speedStep);
        }

        // 속도 증가 (> 키 또는 . 키)
        if (Input.GetKeyDown(KeyCode.Period) || Input.GetKeyDown(KeyCode.Greater))
        {
            AdjustSpeed(speedStep);
        }

        // 숫자 키 1을 누르면 정상 속도(1.0)로 초기화 (편의 기능)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetSpeed(1.0f);
        }
    }

    private void AdjustSpeed(float amount)
    {
        float newSpeed = Time.timeScale + amount;
        SetSpeed(newSpeed);
    }

    private void SetSpeed(float speed)
    {
        // 속도 범위를 minSpeed와 maxSpeed 사이로 제한
        Time.timeScale = Mathf.Clamp(speed, minSpeed, maxSpeed);
        
        // 소수점 오차 방지를 위해 반올림 후 로그 출력
        Debug.Log($"현재 게임 속도: {Time.timeScale:F2}x");
    }
}