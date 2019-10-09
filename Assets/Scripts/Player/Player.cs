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
    [SerializeField] int _health;
    public int maxHealth;

    //Propios del Combate.
    public int maxActionsPosible;
    [SerializeField] int RemainingActions;

    private void Awake()
    {
        //Obtener referencias.

        //Inicializar cosas
        _health = maxHealth;
        combatInterface.PlayerLife = _health;

        //Llamar funciones relevantes.
        //deck.LoadAllCards();

        //Primer update del estado.
        UpdateCombatInterface();
    }

    /// <summary>
    /// Inicia el turno del jugador.
    /// </summary>
    public override void StartTurn()
    {
        //Barajo/Saco cartas del Deck.
        combatInterface.ShowEndTurnButton(true);
        RemainingActions = maxActionsPosible;
    }

    /// <summary>
    /// Se llama en vez de Update.
    /// </summary>
    public override void UpdateTurn()
    {
        UpdateCombatInterface();
    }

    /// <summary>
    /// Termina el turno del jugador.
    /// Utilizado por el botón en canvas que termine el turno.
    /// </summary>
    public override void EndTurn()
    {
        //Animo la interfaz para mostrar que Terminó el turno del jugador.
        combatInterface.ShowEndTurnButton(false);

        //print("El jugador finalizo el turno.");
        //Hasta este punto vamos Bien :D

        //LLamo el evento de Actor
        OnEndTurn(this);
    }

    //-----------------------------------------------------------------------------------------------------

    // Esto lo llamamos desde afuera como por el canvas por ejemplo.
    // Básicamente descarta la carta.
    /// <summary>
    /// 
    /// </summary>
    public void ConsumeSelectedCard()
    {
        if (Selected != null) deck.UseCard(Selected.UniqueID);
    }

    //-----------------------------------------------------------------------------------------------------

    void UpdateCombatInterface()
    {
        if (combatInterface != null)
        {
            combatInterface.PlayerLife = _health;
            combatInterface.RemainingActions = RemainingActions.ToString();
            //Acá falta que el deck esté funcionando.
            //combatInterface.RemainingCards = deck.RemainngCardsAmmount.ToString();
        }
    }
}
