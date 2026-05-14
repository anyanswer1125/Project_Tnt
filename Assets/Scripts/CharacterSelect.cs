using System.Collections.Generic;
using UnityEngine;

public enum Character
{
    Warrior,
    Thief,
    Wizard
}

public class CharacterSelect : MonoBehaviour
{
    // 받아올 Player 리스트
    [SerializeField] private List<Player> players;
    // Dictionary로 오브젝트 이름을 받아서 그것의 Player를 받아 쓸 용도
    private Dictionary<Character, Player> playerDictionary = new Dictionary<Character, Player>();
    // 현재 Player의 포지션
    [SerializeField] private Transform currentPlayerTransform;
    // 현재 캐릭터
    [SerializeField] private Character currentCharacter;
    // 캐릭터 선택 UI
    [SerializeField] private CharacterSelectUI characterSelectUI;
    [SerializeField] private GameObject vfx_ChangePlayer;

    [SerializeField] private AudioClip sfx_PlayerChange;
    
    private AudioSource audioSource;
    void Start()
    {
        Initialize();
    }

    // 초기화 메서드
    private void Initialize()
    {
        players.AddRange(GetComponentsInChildren<Player>(true));

        foreach (Player p in players)
        {
            // 플레이어 리스트에 있는 모든 플레이들의 초기화 함수를 여기서 호출
            p.Initialize();
        }

        characterSelectUI = GameObject.FindAnyObjectByType<CharacterSelectUI>();
        characterSelectUI?.Initialize();
        characterSelectUI?.SelectUI(currentCharacter);

        audioSource = GetComponent<AudioSource>();

        for (int i = 0; i < players.Count; i++)
        {
            playerDictionary.Add(players[i].PlayerCharacterType, players[i]);
        }

        Characters(Character.Thief);
    }

    // 현재 포지션값을 받을 메서드
    private Transform SetPos(Player player)
    {
        currentPlayerTransform = player.Transform;

        return currentPlayerTransform;
    }

    private void Characters(Character c)
    {

        // 예외처리
        if (currentCharacter == c) return;
        // 현재 캐릭터
        Player currentPlayer = playerDictionary[currentCharacter];
        // 바꿀 캐릭터
        Player nextPlayer = playerDictionary[c];
        // 캐릭터 전체를 비활성화

        audioSource.PlayOneShot(sfx_PlayerChange);
        //AutoDestroy 확인했습니다, 이 이펙트도 풀링 형식으로 작업하겠습니다.
        vfx_ChangePlayer.transform.position = currentPlayer.transform.position;
        vfx_ChangePlayer.SetActive(true);



        foreach(Player player in  players)
        {
            player.gameObject.SetActive(false);  
        }
        // 다음 캐릭터 활성화
        nextPlayer.gameObject.SetActive(true);
        // 위치 값 초기화
        nextPlayer.SetPlayerTransform(SetPos(currentPlayer));
        // 다음 캐릭터가 마법사라면 스킬을 쓸 수 있는 지 체크
        if(nextPlayer.PlayerCharacterType == Character.Wizard)
        {
            nextPlayer.isCanWizardSkill();
        }
        // 현재 캐릭터 업데이트
        currentCharacter = c;
        // 현재 캐릭터 선택 UI 변경
        characterSelectUI.SelectUI(currentCharacter);

    }

    private void CharacterChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Characters(Character.Warrior);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Characters(Character.Thief);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Characters(Character.Wizard);
    }

    void Update()
    {
        CharacterChange();
    }
}
