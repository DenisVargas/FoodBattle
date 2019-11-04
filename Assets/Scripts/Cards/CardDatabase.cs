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
    static Dictionary<int, Action<Actor, Actor, CardData, int>> CardBehaviours;

    static CardDatabase()
    {
        //Al inicializarse la clase va a cargar toda la data respectivo a las cartas.
        CardDatas = new Dictionary<int, CardData>();
        CardBehaviours = new Dictionary<int, Action<Actor, Actor, CardData, int>>();

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
        Action<Actor, Actor, CardData, int> Carta1 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //Modificadores de daño.
            int realDamage = stats.GetDebuff(DeBuffType.healthReduction).Ammount + Owner.GetActiveBuffAmmount(BuffType.DamageIncrease);

            //Inflige 2 puntos de daño al oponente.
            Target.GetDamage(realDamage);

            Owner.hand.DiscardCard(DeckID);
        };

        //Carta número 2.
        Action<Actor, Actor, CardData, int> Carta2 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //Reduce en 1 de daño el siguiente turno.
            //TODO: futuro esta función va a recibir un valor extra = cantidad de turnos.
            Owner.AddBuff(stats.GetBuff(BuffType.ArmourIncrease));
        };

        //Carta número 3.
        Action<Actor, Actor, CardData, int> Carta3 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //Restaura 2 de salud en el turno.
            Owner.AddBuff(stats.GetBuff(BuffType.Heal));
        };

        //Carta número 4.
        Action<Actor, Actor, CardData, int> Carta4 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            // Roba 1 carta.
            Owner.DrawCards(stats.extraCards);

            Owner.hand.DiscardCard(DeckID);

        };

        //Carta número 5.
        Action<Actor, Actor, CardData, int> Carta5 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //Carta Categoría Rara.
            //Añade un buffo de Daño +1;
            Owner.AddBuff(stats.GetBuff(BuffType.DamageIncrease));
        };

        //Carta número 6.
        Action<Actor, Actor, CardData, int> Carta6 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //Carta Combo
            List<Card> cantCards = ((Player)Owner).SearchCardType(stats);

            Target.GetDamage((stats.GetDebuff(DeBuffType.healthReduction).Ammount * cantCards.Count));

            foreach (var item in cantCards)
                Owner.hand.DiscardCard(item.DeckID);

            Target.GetDamage((stats.GetDebuff(DeBuffType.healthReduction).Ammount * 2) * cantCards.Count);
        };

        //Carta número 7.
        Action<Actor, Actor, CardData, int> Carta7 = (Actor Owner, Actor Target, CardData stats, int deckID) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //Carta Combo por cada carta
            

            Target.GetDamage((stats.GetDebuff(DeBuffType.healthReduction).Ammount * Owner.hand.hand.Count));
            Target.AddExtraTurn(stats.extraTurns);
            Owner.hand.DiscardCard(deckID);

        };


        //Carta número 8.
        Action<Actor, Actor, CardData, int> Carta8 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //Owner recibe 4 de daño.
            Owner.GetDamage(stats.GetDebuff(DeBuffType.healthReduction).Ammount);

            //Owner gana 1 turno.
            Owner.AddExtraTurn(1);

            Owner.hand.DiscardCard(DeckID);
        };

        //Carta número 9.
        Action<Actor, Actor, CardData, int> Carta9 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //+2 Vida, +1 carta
            Owner.AddBuff(stats.GetBuff(BuffType.Heal));
            Owner.DrawCards(1);

            Owner.hand.DiscardCard(DeckID);
        };

        //Carta número 13.
        Action<Actor, Actor, CardData, int> Carta13 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //Obtenemos toda la vida restante. Perdemos 2 turnos.
            Owner.AddBuffs(stats.GetAllBuffs());
            Target.AddExtraTurn(stats.extraTurns);

            Owner.hand.DiscardCard(DeckID);
        };

        //Carta número 15.
        Action<Actor, Actor, CardData, int> Carta15 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            Owner.ModifyEnergy(stats.Cost);

            //+2 Daño (Target), +2 Salud, +2 Reducción de Daño, +1 Carta, -3 Turnos
            Owner.AddBuffs(stats.GetAllBuffs());
            Owner.DrawCards(stats.extraCards);

            //Modificadores de daño.
            int realDamage = stats.GetDebuff(DeBuffType.healthReduction).Ammount + Owner.GetActiveBuffAmmount(BuffType.DamageIncrease);

            Target.GetDamage(realDamage);
            Target.AddExtraTurn(stats.extraTurns);

            Owner.hand.DiscardCard(DeckID);
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
        CardBehaviours.Add(7, Carta7);
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
    public static Action<Actor, Actor, CardData, int> GetCardBehaviour(int UniqueID)
    {
        return CardBehaviours.ContainsKey(UniqueID) ? CardBehaviours[UniqueID] : null;
    }
}
