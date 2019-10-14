using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public event Action OnDefeat = delegate { };

    public PlayerHUD HUD;
    public Deck deck;

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
        HUD.SetPlayerName(ActorName);
        HUD.PlayerLife = _health;
        HUD.RemainingCards = deck.DeckCards.Count;
        HUD.UsedCards = 0;

        //Llamar funciones relevantes.
        //deck.LoadAllCards();

        //Suscribirse a eventos.
        

        //Primer update del estado.
        UpdateCombatInterface();
    }

    /// <summary>
    /// Inicia el turno del jugador.
    /// </summary>
    public override void StartTurn()
    {
        //Barajo/Saco cartas del Deck.
        HUD.ShowEndTurnButton(true);
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
        HUD.ShowEndTurnButton(false);

        //print("El jugador finalizo el turno.");
        //Hasta este punto vamos Bien :D

        //LLamo el evento de Actor
        OnEndTurn(this);
    }

    //-----------------------------------------------------------------------------------------------------

    void UpdateCombatInterface()
    {
        if (HUD != null)
        {
            HUD.PlayerLife = _health;
            HUD.RemainingActions = RemainingActions;

            //Acá falta que el deck esté funcionando.
            //combatInterface.RemainingCards = deck.RemainngCardsAmmount.ToString();
        }
    }

    void Defeat()
    {
        OnDefeat();
    }
}
