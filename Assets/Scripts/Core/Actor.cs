using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
    Actor debería tener todas las funciones y accesos a información común tanto para el Player 
    Como para el Enemigo a los que una Carta debe tener acceso para funcionar.
    La firma del Behaviour de una carta es (Actor Owner, Actor Target/Rival, CardData)
    Donde: 
        *[Actor] Owner: es el dueño de la carta.
        *[Actor] Target/Rival: es el oponente del Owner.

        *[CardData] CardData: Son las Stats de la carta, guardados en un Scriptable Object.
     Si queremos tener acceso al Deck lo hacemos vía el Owner.
     Si queremos tener acceso al Manager de Turnos lo hacemos mediante el Singleton -> CombatManager.match

     Añadido el apartado de Buffs y Debuffs. Ahora se pueden añadir Buffs/Debuffs, activarlos y restar turnos a aquellos que tienen 
     efectos duraderos.
*/

public abstract class Actor : MonoBehaviour
{
    //Eventos Comunes.
    public Action<Actor> OnStartTurn = delegate { };
    public Action OnUpdateTurn = delegate { };
    public Action<Actor> OnEndTurn = delegate { };
    public Action OnActorDies = delegate { };

    [Header("Estado Comun")]
    public string ActorName;
    public Deck deck;
    public Hand hand;
    public int Health;
    public int maxHealth;
    public int Energy;
    public int Damage;

    [Header("Buffs")]
    public Dictionary<EffectDurationType, List<Buff>> ActiveBuffs = new Dictionary<EffectDurationType, List<Buff>>();

    [Header("Debuffs")]
    public Dictionary<EffectDurationType, List<DeBuff>> ActiveDebuffs = new Dictionary<EffectDurationType, List<DeBuff>>();

    [Header("Turnos Extras")]
    public int extraTurns = 0;

    private bool _invulnerable;

    //=============================== Propiedades ==============================================

    public virtual bool CanExecuteActions { get; set; }
    public virtual bool CanEndTurn { get; set; }

    //============================== UNITY FUNCTIONS =============================================

    public virtual void Awake()
    {
        //Inicializaciones.
        if (!ActiveBuffs.ContainsKey(EffectDurationType.Limited))
            ActiveBuffs.Add(EffectDurationType.Limited, new List<Buff>());
        if (!ActiveBuffs.ContainsKey(EffectDurationType.Permanent))
            ActiveBuffs.Add(EffectDurationType.Permanent, new List<Buff>());
        if (!ActiveDebuffs.ContainsKey(EffectDurationType.Limited))
            ActiveDebuffs.Add(EffectDurationType.Limited, new List<DeBuff>());
        if (!ActiveDebuffs.ContainsKey(EffectDurationType.Permanent))
            ActiveDebuffs.Add(EffectDurationType.Permanent, new List<DeBuff>());
    }

    //============================== Turnos ======================================================

    public virtual void StartTurn(int turnEnergy) { }
    public virtual void UpdateTurn() { }
    public virtual void EndTurn() { }

    //============================ Interacción ===================================================

    /// <summary>
    /// Deshabilita la interacción.
    /// </summary>
    public virtual void DisableInteractions()
    {
        // No puedo seleccionar cartas.
    }
    /// <summary>
    /// Habilita la interacción.
    /// </summary>
    public virtual void EnableInteraction()
    {
        //Puedo seleccionar las cartas.
    }

    //=============================== Buffs & Debuffs ============================================

    /// <summary>
    /// Añade un buffo a este jugador. Si el bufo es Inmediato, activa el buffo. Si el buffo es limitado o permanente se encola y se ejecuta por primera vez.
    /// </summary>
    /// <param name="buff">El buffo a añadir.</param>
    public void AddBuff(Buff buff)
    {
        EffectDurationType duration = buff.durationType;

        //Activo la función inmediatamente.
        if (buff.durationType == EffectDurationType.Inmediate)
            ActivateBuff(buff);
        else
        {
            ActiveBuffs[duration].Add(buff);
            ActivateBuff(buff);
        }
    }
    public void AddBuffs(List<Buff> buffs)
    {
        foreach (var buff in buffs)
            AddBuff(buff);
    }
    public void AddDebuff(DeBuff deBuff)
    {
        EffectDurationType duration = deBuff.durationType;

        //Activo inmediatamente la función;
        if (duration == EffectDurationType.Inmediate)
            ActivateDebuff(deBuff);
        else
        {
            ActiveDebuffs[duration].Add(deBuff);
            ActivateDebuff(deBuff);
        }
    }
    public void AddDebuffs(List<DeBuff> debuffs)
    {
        foreach (var debuff in debuffs)
            AddDebuff(debuff);
    }

    /// <summary>
    /// Permite activar el efecto de un buffo, solo funciona con buffos de duración Inmediata.
    /// </summary>
    /// <param name="buff"></param>
    protected void ActivateBuff(Buff buff)
    {
        //LLama a la función correspondiente dependiendo del tipo de buffo.
        switch (buff.BuffType)
        {
            case BuffType.none:
                break;
            case BuffType.Heal:
                heal(buff.Ammount);
                break;
            case BuffType.FullHealthRestore:
                RestoreAllHealth();
                break;
            case BuffType.NullyfyCardCost:
                NullifyCardCost(buff.Duration);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Permite activar el efecto de un grupo de buffos, solo funciona con buffos de duración Inmediata.
    /// </summary>
    /// <param name="buffs"></param>
    protected void ActivateBuffs(List<Buff> buffs)
    {
        foreach (var buff in buffs)
            ActivateBuff(buff);
    }
    /// <summary>
    /// Permite activar el efecto de un Debuff.
    /// </summary>
    /// <param name="deBuff"></param>
    protected void ActivateDebuff(DeBuff deBuff)
    {
        switch (deBuff.DebuffType)
        {
            case DeBuffType.healthReduction:
                Health -= deBuff.Ammount;
                break;
            case DeBuffType.nullify:
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Permite activar el efecto de un grupo de Debuff.
    /// </summary>
    /// <param name="deBuffs"></param>
    protected void ActivateDebuffs(List<DeBuff> deBuffs)
    {
        foreach (var debuff in deBuffs)
            ActivateDebuff(debuff);
    }

    /// <summary>
    /// Retorna la cantidad acumulada de un tipo de buffo.
    /// </summary>
    /// <param name="buffType">El tipo de buffo del cual queremos sacar la cantidad acumulada.</param>
    public int GetActiveBuffAmmount(BuffType buffType, bool LimitedAndPermanent = false)
    {
        //Primero chequeo que los buffos esten inicializados.
        if (ActiveBuffs.Keys.Count == 0)
        {
            ActiveBuffs.Add(EffectDurationType.Limited, new List<Buff>());
            ActiveBuffs.Add(EffectDurationType.Permanent, new List<Buff>());
        }

        int limited_AcumulatedAmmount = ActiveBuffs[EffectDurationType.Limited]
                                        .Where( b => b.BuffType == buffType)
                                        .Select( b => b.Ammount)
                                        .Sum();

        if (LimitedAndPermanent)
        {
            int permanent_AcumulatedAmmount = ActiveBuffs[EffectDurationType.Permanent]
                                              .Where(b => b.BuffType == buffType)
                                              .Select(b => b.Ammount)
                                              .Sum();
            limited_AcumulatedAmmount += permanent_AcumulatedAmmount;
        }

        return limited_AcumulatedAmmount;
    }

    /// <summary>
    /// Retorna la cantidad acumulada de un tipo de Debuff.
    /// </summary>
    /// <param name="type">El tipo de debuff del cual queremos sacar la cantidad acumulada.</param>
    public int GetActiveDebuffAmmount(DeBuffType type, bool LimitedAndPermanent = false)
    {
        if (ActiveDebuffs.Keys.Count == 0)
        {
            ActiveDebuffs.Add(EffectDurationType.Limited, new List<DeBuff>());
            ActiveDebuffs.Add(EffectDurationType.Permanent, new List<DeBuff>());
        }

        int Acumulated = ActiveDebuffs[EffectDurationType.Limited]
                                        .Where(db => db.DebuffType == type)
                                        .Select(db => db.Ammount)
                                        .Sum();

        if (LimitedAndPermanent)
        {
            int permanent = ActiveDebuffs[EffectDurationType.Permanent]
                            .Where(db => db.DebuffType == type)
                            .Select(db => db.Ammount)
                            .Sum();
            Acumulated += permanent;
        }

        return Acumulated;
    }


    /// <summary>
    /// Reduce en 1 la duración de todos los buffos y debuffos x cada turno que termina.
    /// </summary>
    protected void ReduxActiveEffectsDuration()
    {
        //Necesito todos los buffos que sean de tipo limitado.
        var limitedBuffs = ActiveBuffs[EffectDurationType.Limited];
        var limitedDebuffs = ActiveDebuffs[EffectDurationType.Limited];

        //Le resto 1 a la duración.
        List<Buff> remainingBuffs = new List<Buff>();
        foreach (var buff in limitedBuffs)
        {
            Buff Current = buff;
            Current.Duration--;
            if (Current.Duration > 0) remainingBuffs.Add(Current);
        }
        ActiveBuffs[EffectDurationType.Limited] = remainingBuffs;

        List<DeBuff> remainigDebuffs = new List<DeBuff>();
        foreach (var debuff in limitedDebuffs)
        {
            DeBuff current = debuff;
            current.Duration--;
            if (current.Duration > 0) remainigDebuffs.Add(current);
        }
        ActiveDebuffs[EffectDurationType.Limited] = remainigDebuffs;
    }

    //==================================== Combate ===============================================

    // Sistema de Daño.
    /// <summary>
    /// Aplica daño al jugador.
    /// </summary>
    /// <param name="damage">Cantidad de daño a recibir.</param>
    public virtual void GetDamage(int damage) { }

    //=============================== Turnos y Energía ===========================================

    public virtual void ModifyEnergy(int Ammount) { }
    public virtual void DrawCards(int Ammount) { }
    public virtual void AddExtraTurn(int Ammount)
    {
        extraTurns += Ammount;
        CombatManager.match.AddExtraTurns(this, Ammount);
    }

    //=============================== Efectos Aplicables =========================================

    protected virtual void heal(int Ammount) { }
    protected virtual void RestoreAllHealth() { }
    protected virtual void AddExtraEnergy(int Ammount) { }
    protected virtual void NullifyCardCost(int turns) { }
}
