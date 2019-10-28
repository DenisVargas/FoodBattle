﻿using System;
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
    public AudioSource ad;
    public AudioClip life;
    public AudioClip hit;

    //Propios del Combate.
    public int maxActionsPosible;
    public int RemainingActions;
    bool _interactable = false;

    /// <summary>
    /// Determina si este Actor puede intereactuar con el juego en este momento.
    /// [True] Blockea el uso de las cartas.
    /// [False] Habilita el uso de las cartas.
    /// </summary>
    public override bool CanExecuteActions
    {
        get => _interactable;
        set
        {
            _interactable = value;
            if (_interactable)
                EnableInteraction();
            else
                DisableInteractions();
        }
    }

    private void Awake()
    {
        //Obtener y setear referencias.
        deck.Owner = this;
        ad = GetComponent<AudioSource>();
        //Inicializar cosas
        Health = maxHealth;
        RemainingActions = maxActionsPosible;

        HUD.SetPlayerName(ActorName);
        HUD.PlayerLife = Health;
        HUD.RemainingCards = deck.DeckCards.Count;
        HUD.UsedCards = 0;

        //Llamar funciones relevantes.
        //deck.LoadAllCards();

        //Primer update del estado.
        UpdateCombatInterface();
    }

    //========================================= OVERRIDES =============================================================

    #region Turnos

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

    #endregion

    public override void GetDamage(int damage)
    {
        int realDamage = damage - DamageReduction;
        DamageReduction = 0;
        Health -= realDamage;
        StartCoroutine(shake.Shake(.30f, 0.9f));
        UpdateCombatInterface();
        CombatManager.match.FeedbackHUD.SetDamage("Daño recibido:", realDamage);
        CombatManager.match.HUDAnimations.SetTrigger("PlayerGetsDamage");
        ad.clip = hit;
        ad.Play();

        if (Health <= 0)
            OnPlayerDie();
    }

    public override void DrawCards(int Ammount)
    {
        var cardsDraws = deck.DrawCards(Ammount);
        foreach (var item in cardsDraws)
        {
            item.transform.SetParent(hand.transform);
        }
        hand.AlingCards();
    }

    public override void RestoreAllHealth()
    {
        Health = maxHealth;
    }

    public override void AddExtraEnergy(int Ammount)
    {
        RemainingActions++;
        UpdateCombatInterface();
        CombatManager.match.FeedbackHUD.SetEnergy("Energía: +", Ammount);
        CombatManager.match.HUDAnimations.SetTrigger("PlayerUsedCard");
    }

    public override void heal(int Ammount)
    {
        Health += Ammount;
        UpdateCombatInterface();
        CombatManager.match.FeedbackHUD.SetHeal("Recuperación:", Ammount);
        CombatManager.match.HUDAnimations.SetTrigger("PlayerGetsHealed");
        ad.clip = life;
        ad.Play();
    }

    public override void AddExtraTurn(int Ammount)
    {
        base.AddExtraTurn(Ammount);
        CombatManager.match.FeedbackHUD.SetTurns("Turnos:", Ammount > 0 ? Ammount : -Ammount);
    }

    public override void ModifyEnergy(int Ammount)
    {
        reduxActions(Ammount);
        CombatManager.match.FeedbackHUD.SetEnergy("Energía:", -Ammount);
        CombatManager.match.HUDAnimations.SetTrigger("PlayerUsedCard");
    }

    //-----------------------------------------------------------------------------------------------------


    /// <summary>
    /// Deshabilita la interacción.
    /// </summary>
    void DisableInteractions()
    {
        // No puedo seleccionar cartas.
    }

    /// <summary>
    /// Habilita la interacción.
    /// </summary>
    void EnableInteraction()
    {
        //Puedo seleccionar las cartas.
    }

    /// <summary>
    /// Reduce las acciones posibles consumiento puntos de Energía.
    /// </summary>
    /// <param name="cost">Los puntos de energía que consume la acción.</param>
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
