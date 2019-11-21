using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSelect : MonoBehaviour
{
    public List<DeckData> decks;
    public int deckSelected = 0;
    int enemyDeckIndex = 1;

    public void B_Confirm()
    {
        CombatManager.match.SetDecksAndStartMatch(decks, deckSelected, enemyDeckIndex);
        gameObject.SetActive(false);
    }

    public void DeckCombo()
    {
        deckSelected = 0;
        enemyDeckIndex = 1;
    }

    public void DeckBuffs()
    {
        deckSelected = 1;
        enemyDeckIndex = 0;
    }
}
