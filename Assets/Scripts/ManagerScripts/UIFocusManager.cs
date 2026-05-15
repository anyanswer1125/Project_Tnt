using UnityEngine;
using UnityEngine.EventSystems;

public class UIFocusManager : MonoBehaviour
{
    void Update()
    {
        // 아무것도 선택되어 있지 않은데 키보드 입력이 들어오면
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (Input.anyKeyDown)
            {
                // 다시 첫 번째 버튼을 강제로 잡아줌
                EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
            }
        }
    }
}
