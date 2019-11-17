using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSelect : MonoBehaviour
{
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
        deckToEnemy = deckBuff;
        DeckSelected(deckCombo);
    }

    public void DeckBuffs()
    {
        deckToEnemy = deckCombo;
        DeckSelected(deckBuff);
    }
}
