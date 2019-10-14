using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardBehaviour
{
    static Dictionary<int, Action<Actor, Actor, CardData>> CardBehaviours;

    public static Action<Actor, Actor, CardData> GetCardBehaviour(int UniqueID)
    {
        if (CardBehaviours.ContainsKey(UniqueID))
            return CardBehaviours[UniqueID];
        else
            return null;
    }

    public static void InitCardBehaviourDictionary()
    {
        CardBehaviours = new Dictionary<int, Action<Actor, Actor, CardData>>();

        #region Comportamientos.
        //Carta número 1.
        Action<Actor, Actor, CardData> Carta1 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //Inflige 2 puntos de daño al oponente.
            Target.GetDamage(stats.damage);
        };

        //Carta número 2.
        Action<Actor, Actor, CardData> Carta2 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //Reduce en 1 de daño el siguiente turno.
            //TODO: futuro esta función va a recibir un valor extra = cantidad de turnos.
            Owner.GetBuff(BuffType.DamageReduction, stats.healAmmount);
        };

        //Carta número 3.
        Action<Actor, Actor, CardData> Carta3 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //Restaura 2 de salud en el turno.
            Owner.Health += 2;
        };

        //Carta número 4.
        Action<Actor, Actor, CardData> Carta4 = (Actor Owner, Actor Target, CardData stats) =>
        {
            // Roba 1 carta.
            Owner.deck.DrawCards(1);
        };

        //Carta número 5.
        Action<Actor, Actor, CardData> Carta5 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //Carta Categoría Rara.
            // Pierdes 2 turnos.
            CombatManager.match.AddExtraTurns(Target, 2);
        };
        #endregion

        //Añado los comportamientos y los almaceno en el diccionario en orden.
        CardBehaviours.Add(1, Carta1);
        CardBehaviours.Add(2, Carta2);
        CardBehaviours.Add(3, Carta3);
        CardBehaviours.Add(4, Carta4);
        CardBehaviours.Add(5, Carta5);
    }
}
