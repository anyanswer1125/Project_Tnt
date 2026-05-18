using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Saved Value Shortcuts (외부 대입용 변수)")]
    public int sfxSavedIndex = 0;          // 저장된 SFX value 값
    public int bgmSavedIndex = 0;          // 저장된 BGM value 값
    public int displayModeSavedIndex = 0;  // 저장된 화면모드 값


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
