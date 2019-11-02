using System;
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


    Nota: por ahora solo agregué un tipo de Buff, con el tiempo podriamos ir añadiendo más.
*/

public abstract class Actor : MonoBehaviour
{
    //Eventos Comunes.
    public Action OnStartTurn = delegate { };
    public Action OnUpdateTurn = delegate { };
    public Action<Actor> OnEndTurn = delegate { };
    public Action OnActorDies = delegate { };

    [Header("Estado Comun")]
    public string ActorName;
    public Deck deck;
    public Hand hand;
    public int Health;
    public int maxHealth;
    public int Damage;

    [Header("Turnos Extras")]
    public int extraTurns = 0;
    public int Armour = 0;
    public int DamageIncrease = 0;

    //=============================== Propiedades ==============================================

    public virtual bool CanExecuteActions { get; set; }
    public virtual bool CanEndTurn { get; set; }

    //============================== Turnos ======================================================

    public virtual void StartTurn() { }
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

    //=============================== Efectos Aplicables =========================================

    public void AddBuff(BuffType type, int Ammount)
    {
        switch (type)
        {
            case BuffType.none:
                break;
            case BuffType.ArmourIncrease:
                Armour += Ammount;
                break;
            case BuffType.DamageIncrease:
                DamageIncrease += Ammount;
                break;
            default:
                break;
        }
    }
    public virtual void GetDamage(int damage) { }
    public virtual void heal(int Ammount) { }
    public virtual void RestoreAllHealth() { }
    public virtual void AddExtraEnergy(int Ammount) { }
    public virtual void AddExtraTurn(int Ammount)
    {
        extraTurns += Ammount;
        CombatManager.match.AddExtraTurns(this, Ammount);
    }
    public virtual void ModifyEnergy(int Ammount) { }
    public virtual void DrawCards(int Ammount) { }


}
