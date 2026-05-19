using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// JsonManager는 JsonData를 전체적으로 관리하며 로드 및 저장 기능을 담당함
public class JsonManager : MonoBehaviour
{
    // 정적 싱글톤 인스턴스
    public static JsonManager Instance { get; private set; }

    // 에디터에서 보기위한 변수들 ([HideInInspector] {변수 변수명} 를 써서 보기 편하게 함)
    [SerializeField] private List<TextAsset> textAsset;
    [SerializeField] private List<StageTurns> stageTurnDebugList;
    [SerializeField] private List<SoundTable> soundDebugList;
    [SerializeField] private List<StageCharacterCount> stageCharacterCount;

    // 딕셔너리를 사용한 실질적인 스테이지 턴 데이터 테이블
    public Dictionary<int, StageTurns> StageTurnDict { get; private set; }
    // 딕셔너리를 사용한 실질적인 스테이지 사운드 테이블
    public Dictionary<int, SoundTable> SoundDict { get; private set; }
    public Dictionary<int, StageCharacterCount> StageCharacterCount { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitAllData(); // 모든 데이터 초기화
        }
        else { Destroy(gameObject); }
    }

    // 모든 데이터 초기화
    private void InitAllData()
    {
        StageTurnDict = LoadDataDictionary<StageTurns>(nameof(StageTurns), data => data.StageID);
        stageTurnDebugList = new List<StageTurns>(StageTurnDict.Values);

        SoundDict = LoadDataDictionary<SoundTable>(nameof(SoundTable), data => data.Index);
        soundDebugList = new List<SoundTable>(SoundDict.Values);

        StageCharacterCount = LoadDataDictionary<StageCharacterCount>(nameof(StageCharacterCount), data => data.StageID);
        stageCharacterCount = new List<StageCharacterCount>(StageCharacterCount.Values);
    }

    /// <summary>
    /// JSON 파일을 읽어 리스트로 변환한 뒤, 지정한 키 값을 기준으로 딕셔너리에 담아 반환합니다.
    /// </summary>
    /// <typeparam name="T">데이터 클래스 타입</typeparam>
    /// <param name="fileName">JsonData 폴더 내 파일 이름</param>
    /// <param name="keySelector">T 객체에서 키(int)로 사용할 값을 뽑아내는 규칙 (예: x => x.id)</param>
    /// 대입 : LoadDataDictionary<T>(nameof(T), data => data.Index);
    /// 사용 : if (JsonManager.Instance.T.TryGetValue(Key, out var data))
    private Dictionary<int, T> LoadDataDictionary<T>(string fileName, System.Func<T, int> keySelector) where T : class
    {
        // 1. 파일 로드 (Resources 폴더 기준)
        TextAsset asset = Resources.Load<TextAsset>($"JsonData/{fileName}");

        // asset이 없다면 에러 로그를 남기고 빈 데이터를 반환(오류 방지 및 추적)
        if (asset == null)
        {
            Debug.LogError($"[JsonManager] {fileName} 파일을 찾을 수 없습니다.");
            return new Dictionary<int, T>();
        }

        textAsset.Add(asset);

        // 2. JSON 문자열을 리스트 형태로 역직렬화
        List<T> list = JsonConvert.DeserializeObject<List<T>>(asset.text);

        // 3. 데이터를 빠르게 찾기 위한 딕셔너리 생성
        Dictionary<int, T> dict = new Dictionary<int, T>();

        if (list != null)
        {
            foreach (var item in list)
            {
                // [중요] keySelector(item)을 실행하면 
                // 호출할 때 넘겨준 규칙(예: x => x.stageId)에 따라 실제 ID 값을 뽑아옵니다.
                int key = keySelector(item);

                // 키 값이 중복되는지 체크 (데이터 실수 방지)
                if (!dict.ContainsKey(key))
                {
                    dict[key] = item;
                }
                else
                {
                    Debug.LogWarning($"[JsonManager] 중복된 키 발견: {key} (파일: {fileName})");
                }
            }
        }

        return dict;
    }


    // 데이터 저장 하는 메서드
    public void SaveDataToJson<T>(T data, string fileName)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        string path = Path.Combine(Application.persistentDataPath, fileName + ".json");
        File.WriteAllText(path, json);
    }
}
