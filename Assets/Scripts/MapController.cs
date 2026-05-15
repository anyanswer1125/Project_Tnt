using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class MapController : MonoBehaviour
{
    private string stageName = "";
    [SerializeField] public TextMeshProUGUI textMesh;

    private void OnEnable()
    {
        ExtractStageName();
    }

    public void ExtractStageName()
    {
        string rawText = (textMesh != null) ? textMesh.text : gameObject.name;
        if (string.IsNullOrEmpty(rawText)) return;

        Match nameMatch = Regex.Match(rawText, @"\d+-\d+");

        if (nameMatch.Success)
            stageName = nameMatch.Value;
        else
            stageName = rawText.Trim('-', ' ');
    }
    public void LoadThisScene()
    {
        if (string.IsNullOrEmpty(stageName)) ExtractStageName();

        if (!string.IsNullOrEmpty(stageName)) SceneManager.LoadScene("Map" + stageName);
    }
}