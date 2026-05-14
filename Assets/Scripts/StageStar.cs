using UnityEngine;

public class StageStar : MonoBehaviour
{
    public string stageKey;

    [Header("설정")]    
    public GameObject[] uiStars;
    void OnEnable()
    {
        stageKey = gameObject.name.Replace(" cleared", "");
        RefreshStars();
    }
    public void RefreshStars()
    {
        int savedStars = PlayerPrefs.GetInt(stageKey + "_Stars", 0);

        for (int i = 0; i < uiStars.Length; i++)
        {
            if (i < savedStars)
                uiStars[i].SetActive(true);
            else
                uiStars[i].SetActive(false);
        }
    }
}
