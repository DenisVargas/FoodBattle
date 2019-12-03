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
    public static Dictionary<int, CardData> CardDatas;
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

    public static void LoadAllCardDatas()
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
            //Si tengo no tengo un buff de reducción de costo continuo o permanente:
            int cost = -stats.Cost;
            if (Owner.HasCurrentlyZeroCost()) cost = 0;
            Owner.ModifyEnergy(cost);

            //Modificadores de daño.
            int realDamage = stats.GetDebuff(DeBuffType.healthReduction).Ammount + Owner.GetActiveBuffAmmount(BuffType.DamageIncrease);

            //Inflige 2 puntos de daño al oponente.
            Target.GetDamage(realDamage);

            Owner.hand.DiscardCard(DeckID);
         //   Owner.hand.AlingCards();

        };

        //Carta número 2.
        Action<Actor, Actor, CardData, int> Carta2 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            //Si tengo no tengo un buff de reducción de costo continuo o permanente:
            int cost = -stats.Cost;
            if (Owner.HasCurrentlyZeroCost()) cost = 0;
            Owner.ModifyEnergy(cost);

            //Reduce en 1 de daño el siguiente turno.
            //TODO: futuro esta función va a recibir un valor extra = cantidad de turnos.
            Owner.AddBuff(stats.GetBuff(BuffType.ArmourIncrease));
            CombatManager.match.FeedbackHUD.SetBuffArmor("Resistencia: ", Owner.GetActiveBuffAmmount(BuffType.ArmourIncrease));
            CombatManager.match.HUDAnimations.SetTrigger("PlayerGetShield");
            Owner.hand.DiscardCard(DeckID);
        };

        //Carta número 3.
        Action<Actor, Actor, CardData, int> Carta3 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            //Si tengo no tengo un buff de reducción de costo continuo o permanente:
            int cost = -stats.Cost;
            if (Owner.HasCurrentlyZeroCost()) cost = 0;
            Owner.ModifyEnergy(cost);

            //Restaura 2 de salud en el turno.
            Owner.AddBuff(stats.GetBuff(BuffType.Heal));

            Owner.hand.DiscardCard(DeckID);
        };

        //Carta número 4.
        Action<Actor, Actor, CardData, int> Carta4 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            //Si tengo no tengo un buff de reducción de costo continuo o permanente:
            int cost = -stats.Cost;
            if (Owner.HasCurrentlyZeroCost()) cost = 0;
            Owner.ModifyEnergy(cost);

            // Roba 1 carta.
            List<Card> cantCards = ((Player)Owner).SearchCardType(stats);
            foreach (var item in cantCards)
                Owner.hand.DiscardCard(item.DeckID);

            Owner.DrawCards(cantCards.Count);
        };

        //Carta número 5.
        Action<Actor, Actor, CardData, int> Carta5 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            //Si tengo no tengo un buff de reducción de costo continuo o permanente:
            int cost = -stats.Cost;
            if (Owner.HasCurrentlyZeroCost()) cost = 0;
            Owner.ModifyEnergy(cost);

            //Carta Categoría Rara.
            //Añade un buffo de Daño +1;
            Owner.AddBuff(stats.GetBuff(BuffType.DamageIncrease));

            Owner.hand.DiscardCard(DeckID);
        };

        //Carta número 6.
        Action<Actor, Actor, CardData, int> Carta6 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            //Si tengo no tengo un buff de reducción de costo continuo o permanente:
            int cost = -stats.Cost;
            if (Owner.HasCurrentlyZeroCost()) cost = 0;
            Owner.ModifyEnergy(cost);

            //Carta Combo
            List<Card> cantCards = ((Player)Owner).SearchCardType(stats);

            int realDamage = (stats.GetDebuff(DeBuffType.healthReduction).Ammount * cantCards.Count) + Owner.GetActiveBuffAmmount(BuffType.DamageIncrease);
            Target.GetDamage(realDamage);

            foreach (var item in cantCards)
                Owner.hand.DiscardCard(item.DeckID);
        };

        //Carta número 7.
        Action<Actor, Actor, CardData, int> Carta7 = (Actor Owner, Actor Target, CardData stats, int deckID) =>
        {
            //El player consume Energía.
            //Si tengo no tengo un buff de reducción de costo continuo o permanente:
            int cost = -stats.Cost;
            if (Owner.HasCurrentlyZeroCost()) cost = 0;
            Owner.ModifyEnergy(cost);

            //Carta Combo por cada carta
            List<Card> cantCards = ((Player)Owner).SearchCardType(stats);

            int realDamage = (stats.GetDebuff(DeBuffType.healthReduction).Ammount * Owner.hand.hand.Count) + Owner.GetActiveBuffAmmount(BuffType.DamageIncrease);
            Target.GetDamage(realDamage);

            Owner.hand.DiscardCard(deckID);
            if (cantCards.Count >= 1)
                Owner.DrawCards(1);
           // Owner.hand.AlingCards();

        };


        //Carta número 8.
        Action<Actor, Actor, CardData, int> Carta8 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //Si tengo no tengo un buff de reducción de costo continuo o permanente:
            int cost = -stats.Cost;
            if (Owner.HasCurrentlyZeroCost()) cost = 0;
            Owner.ModifyEnergy(cost);

            //Owner recibe 1 de daño.
            Owner.GetDamage(stats.GetDebuff(DeBuffType.healthReduction).Ammount);

            Owner.AddBuff(stats.GetBuff(BuffType.DamageIncrease));


            Owner.hand.DiscardCard(DeckID);
            Owner.DrawCards(stats.extraCards);
            //Owner.hand.AlingCards();

        };

        //Carta número 9.
        Action<Actor, Actor, CardData, int> Carta9 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //Si tengo no tengo un buff de reducción de costo continuo o permanente:
            int cost = -stats.Cost;
            if (Owner.HasCurrentlyZeroCost()) cost = 0;
            Owner.ModifyEnergy(cost);

            //+2 Vida, +1 carta
            Owner.AddBuff(stats.GetBuff(BuffType.Heal));

            Owner.hand.DiscardCard(DeckID);
            Owner.DrawCards(1);
        };

        //Carta número 10
        Action<Actor, Actor, CardData, int> Carta10 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //Si tengo no tengo un buff de reducción de costo continuo o permanente:
            int cost = -stats.Cost;
            if (Owner.HasCurrentlyZeroCost()) cost = 0;
            Owner.ModifyEnergy(cost);

            int realDamage = stats.GetDebuff(DeBuffType.healthReduction).Ammount + Owner.GetActiveBuffAmmount(BuffType.DamageIncrease);
            Target.GetDamage(realDamage);

            if (Owner.hand.IsFull())
                Target.AddDebuff(stats.GetDebuff(DeBuffType.ArmourDestruction));

            Owner.hand.DiscardCard(DeckID);
        };

        //Carta número 11
        Action<Actor, Actor, CardData, int> Carta11 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //Inflije 7 de daño, Otorga 2 punto de energía por cada "Teniente Bacon"(001) o "Coronel Pochoclon"(007) que tengas en la mano.
            int realDamage = stats.GetDebuff(DeBuffType.healthReduction).Ammount + Owner.GetActiveBuffAmmount(BuffType.DamageIncrease);
            Target.GetDamage(realDamage);

            int Energy = stats.Cost;

            foreach (var item in Owner.hand.hand)
            {
                Card Current = item.Value;
                if (Current.Stats.ID == 1 || Current.Stats.ID == 7)
                    Energy += 2;
            }

            //Si tengo no tengo un buff de reducción de costo continuo o permanente:
            int cost = -stats.Cost;
            if (Owner.HasCurrentlyZeroCost()) cost = 0;
            if (Energy > 0) cost = Energy; //Si los puntos de energía están a favor, entonces los sumamos.

            Owner.ModifyEnergy(cost);
            Owner.hand.DiscardCard(DeckID);
        };

        //Carta número 12.
        Action<Actor, Actor, CardData, int> Carta12 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //Otorga 1 punto de armadura por cada 4 puntos de vida que tenga el dueño. Si la vida es mayor al 50% Inflige 1 puntos de daño por cada 4 puntos de vida.

            int Points = (Owner.Health / 4);
            Debug.Log("Active Esta mierda");

            Owner.AddBuff(new Buff() { BuffType = BuffType.ArmourIncrease, Ammount = Points, durationType = EffectDurationType.Inmediate });

            float percentage = Owner.Health / Owner.maxHealth;
            Debug.Log("percentage" + percentage);

            if (percentage > 0.49f)
            {
                Target.GetDamage(Points);
            }

            //Si tengo no tengo un buff de reducción de costo continuo o permanente:
            int cost = -stats.Cost;
            if (Owner.HasCurrentlyZeroCost()) cost = 0;
            Owner.ModifyEnergy(cost);

            Owner.hand.DiscardCard(DeckID);
        };

        //Carta número 13.
        Action<Actor, Actor, CardData, int> Carta13 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            //Si tengo no tengo un buff de reducción de costo continuo o permanente:
            int cost = -stats.Cost;
            if (Owner.HasCurrentlyZeroCost()) cost = 0;
            Owner.ModifyEnergy(cost);

            //Obtenemos toda la vida restante. Perdemos 2 turnos.
            Owner.AddBuff(stats.GetBuff(BuffType.FullHealthRestore));
            Owner.AddDebuff(stats.GetDebuff(DeBuffType.DamageReduction));

            Owner.hand.DiscardCard(DeckID);
        };

        //Carta número 14.
        Action<Actor, Actor, CardData, int> Carta14 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //Obtienes 4 de armadura  (SI "BURRITO DEFENSOR (ID 002)  ESTA EN MANO LAS CARTAS EN MANO SE REDUCEN -1 DE COSTO)

            //Si tengo no tengo un buff de reducción de costo continuo o permanente:
            int cost = -stats.Cost;
            if (Owner.HasCurrentlyZeroCost()) cost = 0;
            Owner.ModifyEnergy(cost);

            Owner.AddBuff(stats.GetBuff(BuffType.ArmourIncrease));

            foreach (var item in Owner.hand.hand)
            {
                if (item.Value.Stats.ID == 2)
                {
                    Owner.AddBuff(stats.GetBuff(BuffType.CardCostDecrease));
                    break;
                }
            }
            Owner.hand.DiscardCard(DeckID);
        };

        //Carta número 15.
        Action<Actor, Actor, CardData, int> Carta15 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            //El player consume Energía.
            //Si tengo no tengo un buff de reducción de costo continuo o permanente:
            int cost = -stats.Cost;
            if (Owner.HasCurrentlyZeroCost()) cost = 0;
            Owner.ModifyEnergy(cost);

            //+2 Daño (Target), +2 Salud, +2 Reducción de Daño, +1 Carta, -3 Turnos
            Owner.AddBuffs(stats.GetAllBuffs());

            //Modificadores de daño.
            int realDamage = stats.GetDebuff(DeBuffType.healthReduction).Ammount + Owner.GetActiveBuffAmmount(BuffType.DamageIncrease);

            Target.GetDamage(realDamage);

            Owner.hand.DiscardCard(DeckID);
            Owner.DrawCards(stats.extraCards);
        };

        /*
         * Notas:
         * Las cartas de combo no tienen costo porque se originan del uso de los slots...
        */

        Action<Actor, Actor, CardData, int> Carta16 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            // Vuelve al jugador invulnerable.
            Owner.AddBuff(stats.GetBuff(BuffType.Invulnerability));
        };

        Action<Actor, Actor, CardData, int> Carta18 = (Actor Owner, Actor Target, CardData stats, int DeckID) =>
        {
            // Hace que el costo sea 0.
            Owner.AddBuff(stats.GetBuff(BuffType.NullyfyCardCost));
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
        CardBehaviours.Add(10, Carta10);
        CardBehaviours.Add(11, Carta11);
        CardBehaviours.Add(12, Carta12);
        CardBehaviours.Add(13, Carta13);
        CardBehaviours.Add(14, Carta14);
        CardBehaviours.Add(15, Carta15);
        CardBehaviours.Add(16, Carta16);
        CardBehaviours.Add(18, Carta18);
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
