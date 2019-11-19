using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Es la "Mano del jugador"
public class Hand : MonoBehaviour
{
    public Action OnStartDraw = delegate { };
    public Action OnEndDraw = delegate { };

    public List<Transform> cards = new List<Transform>();
    public Dictionary<int, Card> hand = new Dictionary<int, Card>();
    public List<GameObject> ToDrawCards;                  // Representación del deck real.


    [Header("Límite de Cartas")]
    [Tooltip("Límite de cartas en la mano.")]
    public int maxCardsInHand;
    int AmmountOfElements;                                 // Lo usamos para saber si la cantidad de cartas actual es par o impar.

    [Header("Posicionamiento de las cartas")]
    /// <summary>
    /// Permite que los objetos se añadan desde un extremo o otro de la mano actual.
    /// </summary>
    public bool InvertXPositioning = false;
    [Space]
    public Vector3 center;
    public Vector3 objectOffset;
    public float DistanceLimit;
    public float maxPadding;

    List<Vector3> positions;

    [Header("Parámetros de Animación")]
    public AnimationCurve LerpCurve;
    public float AnimationDuration = 1f;
    public float FrameRate = 25f;
    bool drawingCard = false;


    //public Transform node1;
    //public Transform node2;
    //private Vector3 startPost;

    public void HandControl(bool activate)
    {
        foreach (var item in hand)
            item.Value.isInteractuable = activate;
    }

    public void GetDrawedCards(Deck deck, int Ammount)
    {
        if (hand.Count < maxCardsInHand)
        {
            foreach (var item in deck.DrawCards(Ammount))
            {
                item.transform.SetParent(transform);
                //item.inHand = true;
                hand.Add(item.DeckID, item);
            }
            StartCoroutine(DrawCards());
        }
    }

    public void DiscardCard(int idCard)
    {
        foreach (var item in hand)
        {
            if (item.Key == idCard)
            {
                var carta = item.Value;
                //carta.comingBack = false;
                //carta.stopAll = true;
                carta.transform.SetParent(carta.discardPosition);
                hand.Remove(idCard);
                break;
            }
        }
    }

    //public void AlingCards()
    //{
    //    cards.Clear();
    //    for (int i = 0; i < transform.childCount; i++)
    //    {
    //        if (transform.GetChild(i).gameObject.activeSelf)
    //        {
    //            cards.Add(transform.GetChild(i));
    //        }
    //    }
    //    foreach (var item in cards)
    //    {
    //        item.transform.position = Vector3.zero;
    //    }
    //    var leftPoint = node1.position;
    //    var rightPoint = node2.position;

    //    var delta = (leftPoint - rightPoint).magnitude;
    //    var howMany = cards.Count;
    //    var howManyGapsBetweenCards = howMany - 1;
    //    var theHighestIndex = howMany;
    //    var gapFromOneItemToTheNextOne = delta / howManyGapsBetweenCards;
    //    for (int i = 0; i < theHighestIndex; i++)
    //    {
    //        cards[i].transform.position = leftPoint;
    //        cards[i].transform.position += new Vector3((-i * gapFromOneItemToTheNextOne), 0, 0);
    //        cards[i].GetComponent<Card>().starPos = cards[i].transform.position;
    //        cards[i].GetComponent<Card>().inHand = true;
    //    }
    //}

    // Start is called before the first frame update
    void Start()
    {
        AmmountOfElements = cards.Count;
        //print(string.Format("Hay {0} elementos.", AmmountOfElements));

        CalculateHandFinalPositions();
        PlaceElements();
    }

    /// <summary>
    /// Recalcula las posiciones de cada elemento.
    /// </summary>
    private void CalculateHandFinalPositions()
    {
        positions.Clear();
        center = transform.position;

        //Calcular el padding --> la distancia entre cada elemento.
        float padding = (DistanceLimit * 2) / AmmountOfElements;
        //Si el padding es mayor al maxPadding, entonces usamos el valor máximo en vez del resultado.
        if (padding > maxPadding) padding = maxPadding;

        //Calculamos la posición mínima del primer elemento.
        float EvenAmmountOfElements = AmmountOfElements / 2;
        float minXPos = 0;
        if (AmmountOfElements % 2 == 0)
            minXPos -= ((padding * EvenAmmountOfElements) - (0.5f * padding));
        else
            minXPos -= padding * EvenAmmountOfElements;


        //Esto controla la dirección en el que los elementos se desplazan.
        if (InvertXPositioning)
            minXPos = center.x + minXPos;
        else
            minXPos = center.x - minXPos;

        //Posicionamos el primer elemento.
        Vector3 firstElementPos = center + new Vector3(-minXPos + objectOffset.x, objectOffset.y, objectOffset.z);
        positions.Add(firstElementPos);

        //Posicionamos todos los demás elementos.
        for (int i = 1; i < AmmountOfElements; i++)
        {
            Vector3 newElementPosition;
            if (InvertXPositioning)
                newElementPosition = firstElementPos - new Vector3(padding * i, 0, 0);
            else
                newElementPosition = firstElementPos + new Vector3(padding * i, 0, 0);
            positions.Add(newElementPosition);
        }
    }

    //Esto hace que cada carta se posicione en donde debe ir.
    public void PlaceElements()
    {
        //Por ahora.
        for (int i = 0; i < cards.Count; i++)
        {
            var currentElement = cards[i];
            currentElement.transform.position = positions[i];
        }
    }

    private void OnDrawGizmos()
    {
        //Centro.
        Gizmos.color = Color.red;
        Vector3 center = transform.position;
        Gizmos.DrawSphere(center, 0.5f);

        Gizmos.DrawSphere(center + new Vector3(DistanceLimit, 0), 0.5f);
        Gizmos.DrawSphere(center - new Vector3(DistanceLimit, 0), 0.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(center, center + new Vector3(maxPadding, 0, 0));
    }

    IEnumerator DrawCards()
    {
        drawingCard = true;
        OnStartDraw();

        ToDrawCards[0].transform.SetParent(this.transform);
        AmmountOfElements++;
        cards.Insert(0, ToDrawCards[0].transform);
        CalculateHandFinalPositions();

        int elementsToLerp = ToDrawCards.Count;
        var currentCard = ToDrawCards[0];

        float currentLerpTime = 0;
        float AnimationFrameRate = 1 / FrameRate;

        float LerpRate = 0;
        print("Tiempo de la animación es:" + LerpRate);

        Vector3[] originalPositions = new Vector3[cards.Count];
        for (int i = 0; i < cards.Count; i++)
            originalPositions[i] = cards[i].transform.position;

        while (elementsToLerp > 0)
        {
            currentLerpTime += AnimationFrameRate;
            LerpRate = currentLerpTime / AnimationDuration;

            for (int i = 0; i < cards.Count; i++)
            {
                var element = cards[i];
                Vector3 finalPos = positions[i];

                if (element.transform.position != positions[i])
                {
                    //Hago el lerp de las posiciones.
                    Vector3 lerpedPosition = Vector3.Lerp(originalPositions[i], finalPos, LerpCurve.Evaluate(LerpRate));
                    element.transform.position = lerpedPosition;
                }
            }

            if (currentLerpTime >= AnimationDuration) //Alternativa posA = posB
            {
                print("Termine las animaciones.");
                //Al final del la animación sacamos el elemento.
                ToDrawCards.RemoveAt(0);
                elementsToLerp--;
                currentLerpTime = 0;

                if (elementsToLerp > 0)
                {
                    print("Hay mas cartas");
                    AmmountOfElements++;
                    currentCard = ToDrawCards[0];
                    ToDrawCards[0].transform.SetParent(transform);
                    cards.Insert(0, ToDrawCards[0].transform);
                    CalculateHandFinalPositions();

                    originalPositions = new Vector3[cards.Count];
                    for (int i = 0; i < cards.Count; i++)
                        originalPositions[i] = cards[i].transform.position;
                }
            }
            yield return new WaitForSeconds(AnimationFrameRate);
        }

        drawingCard = false;
        OnEndDraw();
    }
}