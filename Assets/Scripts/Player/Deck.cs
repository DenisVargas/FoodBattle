using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.RandomSelections;

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
    [Tooltip("La ruta en donde esta guardado el correspondiente Scriptable Object que guarda las stats")]
    public string CompletePath;
    ///// <summary>
    ///// El nombre del archivo.
    ///// Opcional.
    ///// </summary>
    //[Tooltip("El nombre del archivo")]
    //public string Nombre;
    /// <summary>
    /// Índica la cantidad de dicha carta que va a haber en el mazo.
    /// </summary>
    [Tooltip("Cantidad de cartas que va a haber en el mazo")]
    public int AmmountInDeck;
}

[Serializable]
public class Deck : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public event Action<int> OnCardUsed = delegate { };

    [HideInInspector] public int RemaingCardsAmmount;
    [HideInInspector] public int UsedCardAmmount;

    [Header("Seteos Importantes")]
    public GameObject CardPrefab;
    public Transform CardParent;

    [Header("Cartas Incluídas en el Mazo")]
    /// <summary>
    /// Lista de Datos relacionados a las cartas existentes.
    /// </summary>
    public List<CardTypes> Included = new List<CardTypes>();
    /// <summary>
    /// Datos cargados de las cartas, ordenadas por su [UniqueID].
    /// </summary>
    public Dictionary<int, Card> AviableCards = new Dictionary<int, Card>();

    [Header("Totales")]
    public int TotalCards;                             // Se autorrellena. Cantidad de cartas actual en el mazo. Se reduce al sacar nuevas cartas.
    public int CardTypesAviable;                       // Se autorrellena. Cantidad de tipos de cartas. En nuestro caso cada Carta es un Tipo de carta único.
    public Queue<int> DeckCards = new Queue<int>();    // Se autorrellena. Cola de cartas en el Mazo, utiliza el ID de cada carta.
    public Stack<int> UsedCards = new Stack<int>();    // Se autorrellena. Pila de cartas Utilizadas, también utiliza el ID, y va aumentando a medida que activamos cartas.

    public void LoadAllCards()
    {
        CardTypesAviable = Included.Count;

        //List<Tuple<int a, int b> donde a = tipo de carta. b = Cantidad para añadir. 
        //Nombre: [ToAddList]
        List<Tuple<int, int>> ToAddList = new List<Tuple<int, int>>();

        //Por cada objeto incluido en Included.
        //Calculamos cuantas cartas totales hay dentro del deck.
        foreach (var includedItem in Included)
        {
            //Cargamos todos los Scriptable Objects usando el path y el nombre dentro de [Included];
            CardData data = Resources.Load<CardData>(includedItem.CompletePath);

            // Creamos una carta por cada uno y le atacheamos su data.
            Card realCard = Instantiate(CardPrefab, transform.position, Quaternion.identity, CardParent).GetComponent<Card>();
            realCard.Stats = data;
            realCard.LoadCardDisplayInfo();

            // Suscribirse al evento incluido en cada carta.
            // Esto permite registrar las cartas que se van activando.
            realCard.OnUseCard += CardUsed;

            //Le atacheamos el efecto.
            realCard.CardEffect = CardBehaviour.GetCardBehaviour(data.ID);

            // Creo una tupla donde añado { a = cantidad de cartas, b = ID del tipo de carta }
            Tuple<int, int> AmmountAndType = Tuple.Create(includedItem.AmmountInDeck, data.ID);
            RemaingCardsAmmount += includedItem.AmmountInDeck;

            // Añado la tupla a [ToAddList]
            ToAddList.Add(AmmountAndType);

            //Guardamos los datos dentro de [AviableCards]
            AviableCards.Add(data.ID, realCard);
        }

        //A medida que añado de forma aleatoría una carta a la queue, voy reduciendo la cantidad que falta.

        //Mientras [ToAddList].count sea mayor a 0
        while (ToAddList.Count > 0)
        {
            //Para referencia:
            //Tuple<int a, int b> donde:
            //       a = Cantidad para añadir.
            //       b = tipo de carta (UniqueID).

            //Creo un numero aleatório que este dentro del rango (1 - [ToAddList.count])
            int randomIndex = UnityEngine.Random.Range(0, ToAddList.Count);
            Tuple<int, int> selected = ToAddList[randomIndex];

            //Reduzco en 1 la cantidad de cartas que falta añadir de ese tipo.
            Tuple<int, int> reduxedSelected = Tuple.Create(selected.Item1 - 1, selected.Item2);

            //Si la cantidad de cartas de un tipo particular se vuelve 0 lo sacamos del contenedor ([ToAddList]).
            if (reduxedSelected.Item1 == 0)
                ToAddList.RemoveAt(randomIndex);
            //Sino... Remplazo la tupla en dicho indice por la versión reducida.
            else
                ToAddList[randomIndex] = reduxedSelected;
            //Nota: esto es porque las Tuplas son inmutables.

            //Añado una carta del tipo que existe dentro del Index resultante 
            DeckCards.Enqueue(selected.Item2);
        }

        TotalCards = RemaingCardsAmmount;
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
            drawedCards.Add(AviableCards[drawedCardID]);
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

        OnCardUsed(AviableCards[UniqueID].Stats.cost);
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
