using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : Actor
{
    public PlayerHUD HUD;

    public cameraShaker shake;
    public GameObject turn;

    [Header("Audio")]
    public AudioSource ad;
    public AudioClip life;
    public AudioClip hit;
    public AudioClip extraTurn;
    public AudioClip armor;
    public AudioClip bufdamage;
    public AudioClip endturn;

    //Propios del Combate.
    bool _interactable = false;

    //Timer - Me rindo del SET/GET 
    public float time;
    public Text timers;


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
        timers.enabled = false;
        OnBuffAdded += UpdateBuffDisplay;


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
        //UpdateBuffDisplay();
    }

    //========================================= OVERRIDES =============================================================

    #region Turnos

    /// <summary>
    /// Inicia el turno del jugador.
    /// </summary>
    public override void StartTurn(int turnEnergy)
    {
        OnStartTurn(this);
        turn.SetActive(enabled);
        Energy = turnEnergy;
        UpdateCombatInterface();
        hand.GetDrawedCards(deck, hand.maxCardsInHand - hand.hand.Count);
        HUD.ShowEndTurnButton(true);
        time = 60f;
        timers.enabled = true;
    }
    /// <summary>
    /// Se llama en vez de Update.
    /// </summary>
    public override void UpdateTurn()
    {
        time = time - 1 * Time.deltaTime;
        if(time <= 15f)
        {
            timers.enabled = true;
            timers.color = Color.white;
        }
        if(time <= 5f)
        {
            StartCoroutine(Colors());
        }
        if(time <= 0f)
        {
            EndTurn();
            timers.enabled = false;
        }
        timers.text = "Time: " + time.ToString("f0");
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
        timers.enabled = false;

        HUD.ShowEndTurnButton(false);
        turn.SetActive(!enabled);

        Energy = 0;
        UpdateCombatInterface();

        //Reduzco la cantidad de turnos de mis buffs y debuffs.
        ReduxActiveEffectsDuration();
        UpdateBuffDisplay();

        //LLamo el evento de Actor
        StopCoroutine(Colors());
        OnEndTurn(this);
        ad.clip = endturn;
        ad.Play();
    }

    IEnumerator Colors()
    {
        for (float i = 1; i >= -0.7f; i = 0.7f)
        {
        timers.color = Color.red;
        yield return new WaitForSeconds(0.08f);
        timers.color = Color.white;
        yield return new WaitForSeconds(0.08f);
        if (time <= 0)
                break;
        }
    }

    #endregion

    //Sistema de Daño.
    public override void GetDamage(int damage)
    {

        if (!IsCurrentlyInvulnerable())
        {
            //Calculo del daño real recibido.
            int damageReduction = GetActiveBuffAmmount(BuffType.ArmourIncrease);
            int realDamage = damage - damageReduction;
            realDamage = Mathf.Clamp(realDamage, 0, 100);
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
        else
        {
            //Acá va el feedback de cuando el daño es 0;
            CombatManager.match.FeedbackHUD.SetDamage("Daño recibido:", 0);
            CombatManager.match.HUDAnimations.SetTrigger("PlayerGetsDamage");
        }
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
    public override void RestoreAllHealth()
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
    public override void ModifyEnergy(int Ammount, bool Increase = false)
    {
        Energy += Ammount;

        string message = string.Format("Energía:{0}", Ammount > 0 ? " + " : "");

        CombatManager.match.FeedbackHUD.SetEnergy(message, Ammount);
        CombatManager.match.HUDAnimations.SetTrigger("PlayerUsedCard");
        UpdateCombatInterface();
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Refresca el Display del HUD para este jugador.
    /// </summary>
    void UpdateCombatInterface()
    {
        if (HUD != null)
        {
            HUD.PlayerLife = Health;
            HUD.RemainingActions = Energy;
        }
    }

    void UpdateBuffDisplay()
    {
        if (HUD != null)
        {
            //Buffo Armadura.
            int buffArmour = GetActiveBuffAmmount(BuffType.ArmourIncrease);
            if (buffArmour > 0)
            {
                HUD.SetBuffArmor = buffArmour;
                HUD.SetBuffDisplay(BuffType.ArmourIncrease, true);
                ad.clip = armor;
                ad.Play();
            }
            else
                HUD.SetBuffDisplay(BuffType.ArmourIncrease, false);

            //Buffo Aumento de Daño.
            int buffDamage = GetActiveBuffAmmount(BuffType.DamageIncrease);
            if (buffDamage > 0)
            {
                HUD.SetBuffDamage = buffDamage;
                HUD.SetBuffDisplay(BuffType.DamageIncrease, true);
                ad.clip = bufdamage;
                ad.Play();
            }
            else
                HUD.SetBuffDisplay(BuffType.DamageIncrease, false);
        }
    }
}
