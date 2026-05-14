using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public static MapController instance;
    //public bool isStarted = false; // 시작 여부
    private string stageName = "";
    [SerializeField] public TextMeshProUGUI textMesh;
    //[SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        //DontDestroyOnLoad(transform.root.gameObject);
        instance = this;
    }
    public void Word()
    {
        if (textMesh == null) return;
        string fullText = textMesh.text;
        string[] lines = fullText.Split('\n');

        if (lines.Length > 0)
        {
            string firstLine = lines[0].Trim(); // 공백 제거
            string stageName = firstLine.Replace("-", "");//씬 + stageName으로 받기
        }
    }
    void Update()
    {
        Word();
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && !string.IsNullOrEmpty(stageName))
        {
            SceneManager.LoadScene("Map1-" + stageName);//현재 Map1-1, 1-2 이렇게 나올것 같아 아니라면 stageName으로 변경
        }

    }
}
