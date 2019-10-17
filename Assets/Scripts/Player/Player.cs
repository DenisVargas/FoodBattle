using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public event Action OnPlayerStartedHisTurn = delegate { };
    public event Action OnPlayerEndedHisTurn = delegate { };
    public event Action OnPlayerDie = delegate { };

    public PlayerHUD HUD;

    public cameraShaker shake;

    //Propios del Combate.
    public int maxActionsPosible;
    public int RemainingActions;

    private void Awake()
    {
        //Obtener y setear referencias.
        deck.Owner = this;

        //Inicializar cosas
        Health = maxHealth;
        RemainingActions = maxActionsPosible;

        HUD.SetPlayerName(ActorName);
        HUD.PlayerLife = Health;
        HUD.RemainingCards = deck.DeckCards.Count;
        HUD.UsedCards = 0;

        //Llamar funciones relevantes.
        //deck.LoadAllCards();

        //Suscribirse a eventos.
        deck.OnCardUsed += reduxActions;

        //Primer update del estado.
        UpdateCombatInterface();
    }

    /// <summary>
    /// Inicia el turno del jugador.
    /// </summary>
    public override void StartTurn()
    {
        OnPlayerStartedHisTurn();
        var test = deck.DrawCards(3);
        foreach (var item in test)
        {
            item.transform.SetParent(hand.transform);
            item.inHand = true;
        }
        hand.AlingCards();
        /*foreach (var item in test)
        {
            item.starPos = item.transform.position;
        }*/
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
        OnPlayerEndedHisTurn();
        HUD.ShowEndTurnButton(false);

        RemainingActions = maxActionsPosible;
        UpdateCombatInterface();

        //print("El jugador finalizo el turno.");
        //Hasta este punto vamos Bien :D

        //LLamo el evento de Actor
        OnEndTurn(this);
    }

    public override void GetDamage(int damage)
    {
        Health -= damage;
        StartCoroutine(shake.Shake(.30f, 0.9f));
        UpdateCombatInterface();

        if (Health <= 0)
            OnPlayerDie();
    }

    //-----------------------------------------------------------------------------------------------------

    void reduxActions(int cost)
    {
        RemainingActions -= cost;
        UpdateCombatInterface();
    }

    void UpdateCombatInterface()
    {
        if (HUD != null)
        {
            HUD.PlayerLife = Health;
            HUD.RemainingActions = RemainingActions;

            //Acá falta que el deck esté funcionando.
            //combatInterface.RemainingCards = deck.RemainngCardsAmmount.ToString();
        }
    }

    void Defeat()
    {
        OnPlayerDie();
    }
}
