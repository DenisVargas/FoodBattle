using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Deck", menuName = "Crear Deck")]
public class DeckData : ScriptableObject
{
    public string deckName;
    public int deckID;
    public List<CardsAmmount> cantCards;
}
[Serializable]
public struct CardsAmmount
{
    public int cardID;
    public int ammountOfCards;
}
