using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckSelect : MonoBehaviour
{
    public Image comboImage;
    public Image buffImage;
    public DeckData deckCombo;
    public DeckData deckBuff;
    public DeckData deckSelected;
    public DeckData deckToEnemy;

    public void StartGame()
    {
        CombatManager.match.player.deck.deckSelected = deckSelected;
        CombatManager.match.Enemy.deck.deckSelected = deckToEnemy;
        CombatManager.match.StartGame();
        gameObject.SetActive(false);
    }

    public void DeckSelected(DeckData deck)
    {
        deckSelected = deck;
    }   

    public void DeckCombo()
    {
        comboImage.color = Color.gray;
        buffImage.color = Color.white;
        deckToEnemy = deckBuff;
        DeckSelected(deckCombo);
    }

    public void DeckBuffs()
    {
        buffImage.color = Color.gray;
        comboImage.color = Color.white; 
        deckToEnemy = deckCombo;
        DeckSelected(deckBuff);
    }
}
