using UnityEngine;

public class Maps : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject startPointObject; // 에디터에서 파란색 지점 오브젝트를 연결
    [SerializeField] private GameObject playerPrefab;     // 생성할 캐릭터 프리팹

    void Start()
    {
        // 시작 위치를 가져와서 캐릭터 생성
        Vector3 spawnPos = GetStartPosition();

        if (playerPrefab != null)
        {
            Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        }
    }

    // GameObject의 위치를 반환하는 함수
    public Vector3 GetStartPosition()
    {
        if (startPointObject != null)
        {
            // 등록된 오브젝트의 세계 좌표(World Position)를 반환
            return startPointObject.transform.position;
        }
        return Vector3.zero;
    }
}