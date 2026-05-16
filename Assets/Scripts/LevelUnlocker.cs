using UnityEngine;

public class LevelUnlocker : MonoBehaviour
{
    public LevelData nextLevelData;

    public void UnLockNextLevel()
    {
        if(nextLevelData !=null)
        {
            nextLevelData.isUnlocked = true; //잠금해제
            Debug.Log(nextLevelData.levelName + "가 열렸습니다");
        }
    }

}
