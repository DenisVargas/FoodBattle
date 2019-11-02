using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IA.RandomSelections;
using Random = UnityEngine.Random;

/*
     Este script se va a encargar de manejar la Deck y de cierta manera la mano del jugador.
     Solo va contar las cartas que tenemos dentro del mazo y de repartir la mano.
     Tambíen, si una carta es utilizada, lo pondra en el "cementerio" y en algún punto
     devolverá las cartas al mazo principal.

    /// Ejemplo de formato: (Assets/Resources/Text/textFile01.txt) --> Resources.Load<TextAsset>("Text/textFile01");
*/

[Serializable]
public struct CardTypes
{
    //Path, FileName, Cantidad.

    /// <summary>
    /// La ruta en donde guardamos el scriptable object.
    /// </summary>
    [Tooltip("El Unique ID de la carta incluída en este mazo.")]
    public int IncludedCardID;
    ///// <summary>
    ///// El nombre del archivo.
    ///// Opcional.
    ///// </summary>
    //[Tooltip("El nombre del archivo")]
    //public string Nombre;
    /// <summary>
    /// Índica la cantidad de dicha carta que va a haber en el mazo.
    /// </summary>
    [Tooltip("Cantidad de cartas de dicho tipo que va a haber en el mazo")]
    public int AmmountInDeck;
}

[Serializable]
public class Deck : MonoBehaviour
{
    [HideInInspector] public int RemaingCardsAmmount;
    [HideInInspector] public int UsedCardAmmount;

    [Header("Seteos Importantes")]
    [HideInInspector] public Player Owner;
    public GameObject CardPrefab;
    public Transform CardParent;

    [Header("Cartas Incluídas en el Mazo")]
    /// <summary>
    /// Lista de Datos relacionados a las cartas existentes.
    /// </summary>
    public List<CardTypes> Included = new List<CardTypes>();
    /// <summary>
    /// Datos cargados de las cartas concretas, ordenadas por su [UniqueID]
    /// </summary>
    public Dictionary<int, Card> DeckCardReferencies = new Dictionary<int, Card>();

    [Header("Totales")]
    [HideInInspector] public int TotalCards;                             // Se autorrellena. Cantidad de cartas actual en el mazo. Se reduce al sacar nuevas cartas.
    [HideInInspector] public int CardTypesAviable;                       // Se autorrellena. Cantidad de tipos de cartas. En nuestro caso cada Carta es un Tipo de carta único.
    public Queue<int> DeckCards = new Queue<int>();    // Se autorrellena. Cola de cartas en el Mazo, utiliza el ID de cada carta.
    public Stack<int> UsedCards = new Stack<int>();    // Se autorrellena. Pila de cartas Utilizadas, también utiliza el ID, y va aumentando a medida que activamos cartas.

    public void LoadAllCards()
    {
        CardTypesAviable = Included.Count;

        //Por cada objeto incluido en Included.
        //Calculamos cuantas cartas totales hay dentro del deck.
        int addedID = 0;
        foreach (var includedItem in Included)
        {
            //Cargamos todos los Scriptable Objects usando el path y el nombre dentro de [Included];
            CardData data = CardDatabase.GetCardData(includedItem.IncludedCardID);

            for (int i = 0; i < includedItem.AmmountInDeck; i++)
            {
                // Creamos una carta por cada uno y le atacheamos su data.
                Card realCard = Instantiate(CardPrefab, CardParent.position, Quaternion.identity, CardParent).GetComponent<Card>();
                realCard.Stats = data;
                realCard.LoadCardDisplayInfo();
                realCard.CanBeActivated = (incomingCost) =>  Owner.RemainingActions - incomingCost >= 0;

                addedID++;
                realCard.DeckID = addedID;

                // Suscribirse al evento incluido en cada carta. Esto permite registrar las cartas que se van activando.
                realCard.OnUseCard += CardUsed;

                //Le atacheamos el efecto.
                realCard.CardEffect = CardDatabase.GetCardBehaviour(data.ID);

                //Lo añadimos al diccionario
                DeckCardReferencies.Add(realCard.DeckID, realCard); // Lista de cartas reales que fueron generadas e instanciadas.
                DeckCards.Enqueue(realCard.DeckID);
            }
        }

        TotalCards = RemaingCardsAmmount;
    }

    /// <summary>
    /// Baraja y mezcla el Deck;
    /// </summary>
    public void ShuffleDeck()
    {
        //Armo una lista con todas las cartas de la queue.
        List<int> CardRep = new List<int>();
        int elements = DeckCards.Count;
        for (int i = 0; i < elements; i++)
            CardRep.Add(DeckCards.Dequeue());

        for (int i = 0; i < CardRep.Count; i++)
        {
            //Random Index entre el rango de cartas que queda.
            int randomIndex = Random.Range(0, CardRep.Count);
            int elementB = CardRep[randomIndex];

            //Swapeo los elementos.
            CardRep[randomIndex] = CardRep[i];
            CardRep[i] = elementB;
        }

        //Reseteo deckCards. ReColoco cada carta.
        DeckCards = new Queue<int>();
        foreach (var item in CardRep)
            DeckCards.Enqueue(item);
    }
    /// <summary>
    /// Retorna una cantidad de cartas, el contenido de cada carta es aleatoria.
    /// </summary>
    /// <param name="ammount">Cantidad de cartas a devolver.</param>
    /// <returns></returns>
    public List<Card> DrawCards(int ammount = 5)
    {
        //Retorno una lista aleatoria de [Ammount] cantidad de cartas.
        List<Card> drawedCards = new List<Card>();
        for (int i = 0; i < ammount; i++)
        {
            int drawedCardID = DeckCards.Dequeue();
            RemaingCardsAmmount--;
            drawedCards.Add(DeckCardReferencies[drawedCardID]);
        }

        return drawedCards;
    }
    /// <summary>
    /// Registra el uso de una carta.
    /// </summary>
    /// <param name="UniqueID"> Identificador único de la carta.</param>
    public void CardUsed(int UniqueID)
    {
        // Por si necesitamos llevar un registro de que cartas fueron utilizadas.
        UsedCards.Push(UniqueID);
        UsedCardAmmount++;
    }
    /// <summary>
    /// Retorna las cartas utilizadas al deck principal.
    /// </summary>
    public void ReturnUsedCardsToDeck()
    {
        //Llamo la animación que muestra cuando las cartas vuelven al Deck
        while (UsedCards.Count > 0)
            DeckCards.Enqueue(UsedCards.Pop());
    }
}
