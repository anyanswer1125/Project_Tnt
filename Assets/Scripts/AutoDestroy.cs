using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    void DestroyObj()
    {
        //Destroy(gameObject);
        // 메모리 확보를 위해 파괴(Destroy) 대신 풀(Pool) 반환을 위한 비활성화 처리
        gameObject.SetActive(false);
    }
}
