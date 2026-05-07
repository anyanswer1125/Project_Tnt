using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField] private string LoadToSecne = "PlayScene";
    public static bool isDeath = false;//죽었는지
    public static bool isStart = false;//살았는지
    void Start()
    {
        if(isStart)//게임시작
        {

        }
    }
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.KeypadEnter))
        {
            SceneManager.LoadScene("LoadScene");
        }
    }
    public void Gamestart()
    {
        PlayerPrefs.DeleteAll();
    }
    public void Gameover()
    {
        return;
    }
    public void Option()
    {
        //조작키 변경
        //사운드 변경
    }
}
