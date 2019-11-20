using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Deck", menuName = "Crear Deck")]
public class DeckData : ScriptableObject
{
    public string DeckName = "New Deck";
    public int DeckID;
    public List<CardTypes> IncludedCardsInDeck;
}

[Serializable]
public struct CardTypes
{
    //Path, FileName, Cantidad.

    /// <summary>
    /// La ruta en donde guardamos el scriptable object.
    /// </summary>
    [Tooltip("El Unique ID de la carta incluída en este mazo.")]
    public int IncludedCardID;
    [Tooltip("Cantidad de cartas de dicho tipo que va a haber en el mazo")]
    public int AmmountInDeck;
}
