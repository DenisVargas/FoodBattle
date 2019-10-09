using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public CombatInterface combatInterface;
    public Deck deck;
    public Hand hand;
    public Card Selected;

    //Estadísticas del jugador.
    public int Health;

    //Propios del Combate.
    public int RemainingActions;

    private void Awake()
    {
        //Obtener referencias.

        //Inicializar cosas

        //Llamar funciones relevantes.
        deck.LoadAllCards();

        //Primer update del estado.
        if (combatInterface != null)
        {
            combatInterface.PlayerLife = Health.ToString();
            combatInterface.RemainingActions = RemainingActions.ToString();
            combatInterface.RemainingCards = deck.RemainngCardsAmmount.ToString();
        }
    }

    /// <summary>
    /// Inicia el turno del jugador.
    /// </summary>
    public override void StartTurn()
    {
        //Barajo/Saco cartas del Deck.
    }

    /// <summary>
    /// Se llama en vez de Update.
    /// </summary>
    public override void UpdateTurn()
    {
        
    }

    /// <summary>
    /// Termina el turno del jugador.
    /// </summary>
    public override void EndTurn()
    {
        //Animo la interfaz para mostrar que Terminó el turno del jugador.
    }

    //-----------------------------------------------------------------------------------------------------

    // Esto lo llamamos desde afuera como por el canvas por ejemplo.
    // Básicamente descarta la carta.
    /// <summary>
    /// 
    /// </summary>
    public void ConsumeSelectedCard()
    {
        if (Selected != null)   deck.UseCard(Selected.UniqueID);
    }

}
