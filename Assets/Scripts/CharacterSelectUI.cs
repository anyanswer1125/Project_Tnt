using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private List<Image> icons; // 전체 캐릭터 아이콘
    [SerializeField] private List<Image> outline; // 전체 아웃 라인 이미지
    [SerializeField] private List<Image> lookingIcon;
    [SerializeField] private Sprite currentSelect;   //선택 된 아웃라인
    [SerializeField] private Sprite noneSelect;  // 선택 안된 아웃라인

    string iconListPath = "CharacterIcon/IconList";
    string outlineListPath = "CharacterIcon/OutlineList";
    string LockingIconListPath = "CharacterIcon/LockingIconList";
    Color32 color = new Color32(125, 125, 125, 255);

    public void Initialize()
    {
        icons.AddRange(transform.Find(iconListPath).GetComponentsInChildren<Image>());
        outline.AddRange(transform.Find(outlineListPath).GetComponentsInChildren<Image>());
        lookingIcon.AddRange(transform.Find(LockingIconListPath).GetComponentsInChildren<Image>());

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;


        if (JsonManager.Instance.StageCharacterCount.TryGetValue(sceneIndex, out var data))
        {
            int count = 0;
            count += data.Warrior ? 1 : 0;
            count += data.Thief ? 1 : 0;
            count += data.Wizard ? 1 : 0;

            for(int i = 0; i < count; i++)
            {
                lookingIcon[i].color = new Color(1, 1, 1, 0);
            }
        }
    }

    public void SelectUI(Character character)
    {
        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].color = this.color;
            outline[i].sprite = noneSelect;
        }

        int index = (int)character;
        icons[index].color = Color.white;
        outline[index].sprite = currentSelect;
    }
}
