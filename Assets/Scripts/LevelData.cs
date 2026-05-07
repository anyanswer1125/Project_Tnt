using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "GameData/LevelData")]
public class LevelData : ScriptableObject
{
    public string levelName;      // 화면에 표시될 스테이지 이름 (예: 1-1)
    public string sceneName;      // 실제로 이동할 유니티 씬 이름
    public bool isUnlocked;       // 잠금 해제 여부
}

