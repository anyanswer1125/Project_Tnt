using UnityEngine;

public class UtilManager : MonoBehaviour
{
    private static UtilManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }
}
