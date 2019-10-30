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
            //El player consume Energía.
            Owner.ModifyEnergy(stats.cost);

            //Modificadores de daño.
            int realDamage = stats.damage + Owner.DamageIncrease;

            //Inflige 2 puntos de daño al oponente.
            Target.GetDamage(realDamage);
        };

        //Carta número 2.
        Action<Actor, Actor, CardData> Carta2 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.cost);

            //Reduce en 1 de daño el siguiente turno.
            //TODO: futuro esta función va a recibir un valor extra = cantidad de turnos.
            Owner.GetBuff(BuffType.DamageReduction, 1);
        };

        //Carta número 3.
        Action<Actor, Actor, CardData> Carta3 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.cost);

            //Restaura 2 de salud en el turno.
            Owner.heal(stats.buffAmmount);
        };

        //Carta número 4.
        Action<Actor, Actor, CardData> Carta4 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.cost);

            // Roba 1 carta.
            Owner.DrawCards(1);
        };

        //Carta número 5.
        Action<Actor, Actor, CardData> Carta5 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.cost);

            //Carta Categoría Rara.
            //Añade un buffo de Daño +1;
            Owner.GetBuff(BuffType.DamageIncrease, stats.buffAmmount);
        };

        //Carta número 6.
        Action<Actor, Actor, CardData> Carta6 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.cost);

            //Carta Combo
            var cantCards = Owner.GetComponent<Player>().SearchCardType(stats);
            Debug.Log(cantCards.Count);
            Target.GetDamage(stats.damage + stats.damage * cantCards.Count);
            Owner.GetComponent<Player>().DiscardCard(cantCards);
        };

        //Carta número 8.
        Action<Actor, Actor, CardData> Carta8 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.cost);

            //Owner recibe 4 de daño.
            Owner.GetDamage(stats.damage);

            //Owner gana 1 turno.
            Owner.AddExtraTurn(1);
        };

        //Carta número 9.
        Action<Actor, Actor, CardData> Carta9 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.cost);

            //+2 Vida, +1 carta
            Owner.heal(stats.HealAmmount);
            Owner.DrawCards(1);
        };

        //Carta número 13.
        Action<Actor, Actor, CardData> Carta13 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.cost);

            //Obtenemos toda la vida restante. Perdemos 2 turnos.
            Owner.RestoreAllHealth();
            Target.AddExtraTurn(stats.damage);
        };

        //Carta número 15.
        Action<Actor, Actor, CardData> Carta15 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.cost);

            //+2 Daño (Target), +2 Salud, +2 Reducción de Daño, +1 Carta, -3 Turnos
            Owner.heal(stats.damage);
            Owner.GetBuff(BuffType.DamageReduction, stats.damage);
            Owner.DrawCards(1);

            //Modificadores de daño.
            int realDamage = stats.damage + Owner.DamageIncrease;

            Target.GetDamage(realDamage);
            Target.AddExtraTurn(3);
        };
        #endregion

        #region Utility
        ////Carta número {}.
        //Action<Actor, Actor, CardData> Carta{ } = (Actor Owner, Actor Target, CardData stats) =>
        //{
        //    //El player consume Energía.
        //    Owner.ModifyEnergy(stats.cost);
        //}; 
        #endregion

        //Añado los comportamientos y los almaceno en el diccionario en orden.
        CardBehaviours.Add(1, Carta1);
        CardBehaviours.Add(2, Carta2);
        CardBehaviours.Add(3, Carta3);
        CardBehaviours.Add(4, Carta4);
        CardBehaviours.Add(5, Carta5);
        CardBehaviours.Add(6, Carta6);
        CardBehaviours.Add(8, Carta8);
        CardBehaviours.Add(9, Carta9);
        CardBehaviours.Add(13, Carta13);
        CardBehaviours.Add(15, Carta15);
    }
}
