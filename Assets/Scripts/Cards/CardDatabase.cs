using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public static class CardDatabase
{
    const string RelativeDataPath = "Cards/";
    /// <summary>
    /// Contiene todos las stats de cada carta del juego.
    /// </summary>
    static Dictionary<int, CardData> CardDatas;
    /// <summary>
    /// Contiene todos los comportamientos de cada carta del juego.
    /// </summary>
    static Dictionary<int, Action<Actor, Actor, CardData>> CardBehaviours;

    static CardDatabase()
    {
        //Al inicializarse la clase va a cargar toda la data respectivo a las cartas.
        CardDatas = new Dictionary<int, CardData>();
        CardBehaviours = new Dictionary<int, Action<Actor, Actor, CardData>>();

        LoadAllCardDatas();
        LoadAllBehaviours();
    }

    private static void LoadAllCardDatas()
    {
        CardData[] data = Resources.LoadAll<CardData>(RelativeDataPath);
        foreach (var cardData in data)
            CardDatas.Add(cardData.ID, cardData);
    }
    private static void LoadAllBehaviours()
    {
        #region Comportamientos.
        //Carta número 1.
        Action<Actor, Actor, CardData> Carta1 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //Modificadores de daño.
            int realDamage = stats.GetDebuffAmmount(DeBuffType.healthReduction) + Owner.DamageIncrease;

            //Inflige 2 puntos de daño al oponente.
            Target.GetDamage(realDamage);
        };

        //Carta número 2.
        Action<Actor, Actor, CardData> Carta2 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //Reduce en 1 de daño el siguiente turno.
            //TODO: futuro esta función va a recibir un valor extra = cantidad de turnos.
            Owner.AddBuff(BuffType.ArmourIncrease, 1);
        };

        //Carta número 3.
        Action<Actor, Actor, CardData> Carta3 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //Restaura 2 de salud en el turno.
            Owner.heal(stats.GetBuffAmmount(BuffType.Heal));
        };

        //Carta número 4.
        Action<Actor, Actor, CardData> Carta4 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            // Roba 1 carta.
            Owner.DrawCards(stats.extraCards);
        };

        //Carta número 5.
        Action<Actor, Actor, CardData> Carta5 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //Carta Categoría Rara.
            //Añade un buffo de Daño +1;
            Owner.AddBuff(BuffType.DamageIncrease, stats.GetBuffAmmount(BuffType.DamageIncrease));
        };

        //Carta número 6.
        Action<Actor, Actor, CardData> Carta6 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //Carta Combo
            List<Card> cantCards = Owner.GetComponent<Player>().SearchCardType(stats);
            Owner.hand.DiscardCard(cantCards);

            Target.GetDamage((stats.GetDebuffAmmount(DeBuffType.healthReduction) * 2) * cantCards.Count);
        };

        //Carta número 8.
        Action<Actor, Actor, CardData> Carta8 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //Owner recibe 4 de daño.
            Owner.GetDamage(stats.GetDebuffAmmount(DeBuffType.healthReduction));

            //Owner gana 1 turno.
            Owner.AddExtraTurn(1);
        };

        //Carta número 9.
        Action<Actor, Actor, CardData> Carta9 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //+2 Vida, +1 carta
            Owner.heal(stats.GetBuffAmmount(BuffType.Heal));
            Owner.DrawCards(1);
        };

        //Carta número 13.
        Action<Actor, Actor, CardData> Carta13 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //Obtenemos toda la vida restante. Perdemos 2 turnos.
            Owner.RestoreAllHealth();
            Target.AddExtraTurn(stats.extraTurns);
        };

        //Carta número 15.
        Action<Actor, Actor, CardData> Carta15 = (Actor Owner, Actor Target, CardData stats) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //+2 Daño (Target), +2 Salud, +2 Reducción de Daño, +1 Carta, -3 Turnos
            Owner.heal(stats.GetDebuffAmmount(DeBuffType.healthReduction));
            Owner.AddBuff(BuffType.ArmourIncrease, stats.GetBuffAmmount(BuffType.ArmourIncrease));
            Owner.DrawCards(1);

            //Modificadores de daño.
            int realDamage = stats.GetDebuffAmmount(DeBuffType.healthReduction) + Owner.DamageIncrease;

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

    /// <summary>
    /// Retorna los datos asociados a la carta identificada por el parámetro ID.
    /// </summary>
    /// <param name="UniqueID">Identificador único de la carta.</param>
    /// <returns>Null si la ID no tiene ningun dato asociado.</returns>
    public static CardData GetCardData(int UniqueID)
    {
        return CardDatas.ContainsKey(UniqueID) ? CardDatas[UniqueID] : null;
    }
    /// <summary>
    /// Retorna el comportamiento asociado a la carta identificada por el parámetro ID.
    /// </summary>
    /// <param name="UniqueID">Identificador único de la carta.</param>
    /// <returns>Null si la ID no tiene ningún comportamiento asociado.</returns>
    public static Action<Actor, Actor, CardData> GetCardBehaviour(int UniqueID)
    {
        return CardBehaviours.ContainsKey(UniqueID) ? CardBehaviours[UniqueID] : null;
    }
}
