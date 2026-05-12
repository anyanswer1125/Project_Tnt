using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEditor.Experimental.GraphView.GraphView;

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
    private Dictionary<string, Player> playerDictionary = new Dictionary<string, Player>();
    // 현재 Player의 포지션
    [SerializeField] private Vector3 currentPlayerPos;
    // 현재 캐릭터
    [SerializeField] private Character currentCharacter;

    void Start()
    {
        Initialize();
    }

    // 초기화 메서드
    private void Initialize()
    {
        players.AddRange(GetComponentsInChildren<Player>(true));

        for(int i = 0; i < players.Count; i++)
        {
            playerDictionary.Add(players[i].gameObject.name, players[i]);
        }
    }

    // 현재 포지션값을 받을 메서드
    private void SetPlayerPos(Player player)
    {
        currentPlayerPos = player.Pos;
    }

    private void Characters(Character c)
    {
        // 예외처리
        if (currentCharacter == c) return;

        Player currentPlayer = playerDictionary[currentCharacter.ToString()];

        SetPlayerPos(currentPlayer);

        Player nextPlayer = playerDictionary[c.ToString()];

        foreach(Player player in  players)
        {
            player.gameObject.SetActive(false);  
        }
        nextPlayer.gameObject.SetActive(true);

        currentCharacter = c;
    }

    private void CharacterChange()
    {
        string input = Input.inputString;

        if (!string.IsNullOrEmpty(input))
        {
            switch (input)
            {
                case "1":
                    Characters(Character.Warrior);
                    break;
                case "2":
                    Characters(Character.Thief);
                    break;
                case "3":
                    Characters(Character.Wizard);
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CharacterChange();
    }
}
