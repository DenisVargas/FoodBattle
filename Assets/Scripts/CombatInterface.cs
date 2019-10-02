using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatInterface : MonoBehaviour
{
    [SerializeField] TMP_Text _playerLife;
    [SerializeField] TMP_Text _remaingActions;
    [SerializeField] TMP_Text _usedCards;
    [SerializeField] TMP_Text _remainingCards;

    public string PlayerLife
    {
        set { _playerLife.text = "Lifes: " + value; }
    }
    public string RemainingActions
    {
        set { _remaingActions.text = value; }
    }
    public string UsedCards
    {
        set { _usedCards.text = value; }
    }
    public string RemainingCards
    {
        set { _remainingCards.text = value; }
    }
}
