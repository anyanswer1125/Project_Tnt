// 1. 데이터 클래스 (별도 파일 혹은 JsonManager 위에 선언)
[System.Serializable]
public class MapData
{
    public string MapName;  // 하이어라키 오브젝트 이름과 일치해야 함
    public string SceneName;// 실제 로드할 씬 이름
    public bool isCleared;
}