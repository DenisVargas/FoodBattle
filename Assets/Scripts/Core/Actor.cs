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

*/
public abstract class Actor : MonoBehaviour
{
    public enum BuffType
    {
        DamageReduction
    }


    //Estado común
    public string ActorName;
    public Deck deck;
    public int Health;
    public int Damage;

    //Turnos Extas.
    public int extraTurns = 0;
    public int DamageReduction = 0;

    //Eventos Comunes.
    public Action<Actor> OnStartTurn = delegate { };
    public Action OnUpdateTurn = delegate { };

    public void GetBuff(BuffType type, int Ammount)
    {
        switch (type)
        {
            case BuffType.DamageReduction:
                DamageReduction += Ammount;
                break;
            default:
                break;
        }
    }

    public Action<Actor> OnEndTurn = delegate { };

    public virtual void StartTurn() { }
    public virtual void UpdateTurn() { }
    public virtual void EndTurn() { }
    public virtual void GetDamage(int damage)
    {
        //Acá van los cálulos de reducción de daño.
    }
}
