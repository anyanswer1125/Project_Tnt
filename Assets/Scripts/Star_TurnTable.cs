using Newtonsoft.Json;

[System.Serializable]
public class Star_TurnTable
{
    public int Round;
    [JsonProperty("2Star_turn")] // JSON의 키값과 변수명을 매핑해줍니다.
    public int twoStarTurn;
    [JsonProperty("3Star_turn")]
    public int threeStarTurn;
}
