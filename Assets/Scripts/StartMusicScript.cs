using UnityEngine;

public class StartMusicScript : MonoBehaviour
{

    void Awake()
    {

        SoundManager.Instance.PlayBgm(119);
            
    }
}
