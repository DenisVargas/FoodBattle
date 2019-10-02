using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.RandomSelections;

[Serializable]
public struct CardTypes
{
    //Path, Nombre, Cantidad.

    /// <summary>
    /// La ruta en donde guardamos el scriptable object. (Ignorar si usamos Resources.Load)
    /// </summary>
    String Path;
    /// <summary>
    /// El nombre del archivo.
    /// </summary>
    String Nombre;
    /// <summary>
    /// Índica la cantidad de dicha carta que va a haber en el mazo.
    /// </summary>
    int AmmountInDeck;
}

[Serializable]
public class Deck : MonoBehaviour
{
    public int RemainngCardsAmmount;
    public int UsedCardAmmount;

    /// <summary>
    /// Lista de Datos relacionados a las cartas existentes.
    /// </summary>
    public List<CardTypes> Included = new List<CardTypes>();
    /// <summary>
    /// Datos cargados de las cartas, ordenadas por su [UniqueID].
    /// </summary>
    public Dictionary<int, Card> AviableCards = new Dictionary<int, Card>();

    public int TotalCards;
    public int CardTypesAviable;
    public Queue<Card> DeckCards = new Queue<Card>();
    public Stack<Card> UsedCards = new Stack<Card>();

    public void LoadAllCards()
    {
        CardTypesAviable = Included.Count;

        //Cargamos todos los Scriptable Objects usando el path y el nombre dentro de [Included] y creamos una carta por cada uno.
        //Añadimos un ID único para cada instancia de la carta dentro de [AviableCards]
        //Guardamos los datos dentro de [AviableCards]

        //List<Tuple<int a, int b> donde a = tipo de carta. b = Cantidad para añadir. 
        //Nombre: [ToAddList]

		//Calculamos cuantas cartas totales hay dentro del deck.
			// Por cada item en Included
            // Creo una tupla donde añado { a = cantidad de cartas, b = ID del tipo de carta }
            // Añado la tupla a [ToAddList]

        //A medida que añado de forma aleatoría una carta a la queue, voy reduciendo la cantidad que falta.

		//Mientras [ToAddList].count sea mayor a 0
			//Creo un numero aleatório que este dentro del rango (1 - [ToAddList.count])
            //Reduzco en 1 la cantidad de cartas que falta añadir de ese tipo.
            //Si la cantidad de cartas de un tipo particular se vuelve 0, lo sacamos del contenedor ([ToAddList]).
            //Añado una carta del tipo que existe dentro del Index resultante 
            //--> DeckCards.Enqueue(AviableCards[ToAddList[Index].Item2])
    }

    /// <summary>
    /// Retorna una cantidad de cartas, el contenido de cada carta es aleatoria.
    /// </summary>
    /// <param name="ammount">Cantidad de cartas a devolver.</param>
    /// <returns></returns>
    public List<Card> DrawCards(int ammount = 5)
    {
        //Retorno una lista aleatoria de [Ammount] cantidad de cartas.
        return new List<Card>();
    }

    public void UseCard(int id)
    {
        Card toActivate = AviableCards[id];
        toActivate.OnTable();

        //Añado la carta a la pila de cartas usadas.
        UsedCards.Push(toActivate);
    }

    /// <summary>
    /// Retorna las cartas utilizadas al deck principal.
    /// </summary>
    public void ReturnUsedCardsToDeck()
    {
        //Llamo la animación que muestra cuando las cartas vuelven al Deck
        while (UsedCards.Count > 0)
        {
            DeckCards.Enqueue(UsedCards.Pop());
        }
    }
}
