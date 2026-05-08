using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private int turnCount = 0;
    [SerializeField] private int maxTurn = 0;
    [SerializeField] private TMP_Text turnText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }


    private void Initialize()
    {
        turnText = transform.Find("TurnText").GetComponent<TMP_Text>();
        TurnTextUpdata();
    }

    public void SetTurnCount()
    {
        turnCount++;
        TurnTextUpdata();
    }

    private void TurnTextUpdata()
    {
        if (turnCount <= maxTurn)
            turnText.text = $"{turnCount} / {maxTurn}";
        else
            turnText.text = $"<color=red>{turnCount}</color> / {maxTurn}";
    }
}
