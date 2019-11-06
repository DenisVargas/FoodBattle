using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : Actor
{
    public PlayerHUD HUD;

    public cameraShaker shake;

    [Header("Audio")]
    public AudioSource ad;
    public AudioClip life;
    public AudioClip hit;
    public AudioClip extraTurn;

    //Propios del Combate.
    bool _interactable = false;

    //========================================= PROPIEDADES ===========================================================

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

    //========================================= FUNCIONES DE UNITY ====================================================

    public override void Awake()
    {
        base.Awake();

        //Obtener y setear referencias.
        deck.Owner = this;
        ad = GetComponent<AudioSource>();
        //Inicializar cosas
        Health = maxHealth;
        //Energy = 0;                              //Esto no hace falta porque CombatManager lo inicializa.

        HUD.SetPlayerName(ActorName);
        HUD.PlayerLife = Health;
        HUD.RemainingCards = deck.DeckCards.Count;
        HUD.UsedCards = 0;

        //Llamar funciones relevantes.
        //deck.LoadAllCards();

        //Primer update del estado.
        //UpdateCombatInterface();                 //Esto no hace falta porque CombatManager lo inicializa.
    }

    //========================================= OVERRIDES =============================================================

    #region Turnos

    /// <summary>
    /// Inicia el turno del jugador.
    /// </summary>
    public override void StartTurn(int turnEnergy)
    {
        OnStartTurn(this);
        Energy = turnEnergy;
        UpdateCombatInterface();
        hand.GetDrawedCards(deck, hand.maxCardsInHand - hand.hand.Count);
        HUD.ShowEndTurnButton(true);
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
        //print("El jugador finalizo el turno.");
        //Animo la interfaz para mostrar que Terminó el turno del jugador.
        HUD.ShowEndTurnButton(false);

        Energy = 0;
        UpdateCombatInterface();

        //Reduzco la cantidad de turnos de mis buffs y debuffs.
        ReduxActiveEffectsDuration();

        //LLamo el evento de Actor
        OnEndTurn(this);
    }

    #endregion

    //Sistema de Daño.
    public override void GetDamage(int damage)
    {
        //Calculo del daño real recibido.
        int damageReduction = GetActiveBuffAmmount(BuffType.ArmourIncrease);
        int realDamage = damage - damageReduction;
        Health -= realDamage;

        //Feedback.
        StartCoroutine(shake.Shake(.30f, 0.9f));
        UpdateCombatInterface();
        CombatManager.match.FeedbackHUD.SetDamage("Daño recibido:", realDamage);
        CombatManager.match.HUDAnimations.SetTrigger("PlayerGetsDamage");
        ad.clip = hit;
        ad.Play();

        //Condición de Derrota.
        if (Health <= 0)
            OnActorDies();
    }

    #region Interacción

    /// <summary>
    /// Habilita las interacciones de este jugador.
    /// </summary>
    public override void EnableInteraction()
    {
        //Le decimos al deck que todas las cartas deben ser seleccionables.
        base.EnableInteraction();
        hand.HandControl(true);
    }
    /// <summary>
    /// Des-abilita las interacciones de este jugador.
    /// </summary>
    public override void DisableInteractions()
    {
        //Le decimos al deck que todas las cartas deben dejar de ser seleccionables.
        base.DisableInteractions();
        hand.HandControl(false);
    }

    public List<Card> SearchCardType(CardData tipoCarta)
    {
        var cartasDelMismoTipo = hand.hand.Values
                                 .Where(card => card.Stats.ID == tipoCarta.ID)
                                 .ToList();
        return cartasDelMismoTipo;
    }

    #endregion

    #region Efectos Aplicables.

    public override void DrawCards(int Ammount)
    {
        var cardsDraws = deck.DrawCards(Ammount);
        foreach (var item in cardsDraws)
        {
            item.transform.SetParent(hand.transform);
            item.inHand = true;
            hand.hand.Add(item.DeckID, item);
        }
        hand.AlingCards();
    }
    protected override void RestoreAllHealth()
    {
        Health = maxHealth;
    }
    protected override void AddExtraEnergy(int Ammount)
    {
        Energy++;
        UpdateCombatInterface();
        CombatManager.match.FeedbackHUD.SetEnergy("Energía: +", Ammount);
        CombatManager.match.HUDAnimations.SetTrigger("PlayerUsedCard");
    }
    protected override void heal(int Ammount)
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

        ad.clip = extraTurn;
        ad.Play();

        CombatManager.match.FeedbackHUD.SetTurns("Turnos:", Ammount > 0 ? Ammount : -Ammount);
    }
    public override void ModifyEnergy(int Ammount)
    {
        reduxActions(Ammount);
        CombatManager.match.FeedbackHUD.SetEnergy("Energía:", -Ammount);
        CombatManager.match.HUDAnimations.SetTrigger("PlayerUsedCard");
    }

    //public override void AddBuff(BuffType type, int Ammount)
    //{
    //    base.AddBuff(type, Ammount);
    //    switch (type)
    //    {
    //        case BuffType.none:
    //            break;
    //        case BuffType.ArmourIncrease:
    //            Armour += Ammount;
    //            CombatManager.match.FeedbackHUD.SetBuffArmor("Resistencia: ", Ammount);
    //            CombatManager.match.HUDAnimations.SetTrigger("PlayerGetShield");
    //            break;
    //        case BuffType.DamageIncrease:
    //            DamageIncrease += Ammount;
    //            CombatManager.match.FeedbackHUD.SetBuffAttack("Fuerza: ", Ammount);
    //            break;
    //        default:
    //            break;
    //    }
    //}


    #endregion

    //------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Reduce las acciones posibles consumiento puntos de Energía.
    /// </summary>
    /// <param name="cost">Los puntos de energía que consume la acción.</param>
    void reduxActions(int cost)
    {
        Energy -= cost;
        UpdateCombatInterface();
    }

    /// <summary>
    /// Refresca el Display del HUD para este jugador.
    /// </summary>
    void UpdateCombatInterface()
    {
        if (HUD != null)
        {
            HUD.PlayerLife = Health;
            HUD.RemainingActions = Energy;
            HUD.SetBuffArmor = GetActiveBuffAmmount(BuffType.ArmourIncrease);
            HUD.SetBuffDamage = GetActiveBuffAmmount(BuffType.DamageIncrease);
        }
    }
}
