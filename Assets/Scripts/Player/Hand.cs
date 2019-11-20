using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Es la "Mano del jugador"
public class Hand : MonoBehaviour
{
    public Action OnStartDraw = delegate { };
    public Action OnEndDraw = delegate { };

    public Dictionary<int, Card> hand;                   // Contiene las cartas ordenadas por su "UniqueID".
    public List<Transform> ObjectsToAllign;              // Se autorrellena al ejecutar DrawCards. Esta es la lista de cartas que ya está en la mano, si se agrega una carta nueva, se desplazan a un costado.
    public List<Transform> ToDrawCards;                  // Representación del deck real (Componente Transform de cada carta en la mano).
    List<Vector3> _finalPositions;                       // Posiciones finales calculadas para cada de los elementos dentro de "ToDrawCards"


    [Header("Límite de Cartas")]
    [Tooltip("Límite de cartas en la mano.")]
    public int maxCardsInHand;

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

    [Header("Parámetros de Animación")]
    public AnimationCurve LerpCurve;
    public float AnimationDuration = 1f;
    public float FrameRate = 25f;

    int AmmountOfElements = 0;                                 // Lo usamos para saber si la cantidad de cartas actual es par o impar.
    bool drawingCard = false;

    //================================================ DEBUG GIZMOS =====================================================================
#if UNITY_EDITOR

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

#endif
    //================================================= UNITY EVENTS ====================================================================

    private void Awake()
    {
        //Inicializaciones:
        hand = new Dictionary<int, Card>();
        ObjectsToAllign = new List<Transform>();
        ToDrawCards = new List<Transform>();
        _finalPositions = new List<Vector3>();

        center = transform.position;
    }

    //================================================== MEMBER FUNCS ===================================================================

    /// <summary>
    /// Activa o desactiva la interactividad.
    /// </summary>
    /// <param name="activate">Activar o desactivar?</param>
    public void HandControl(bool activate)
    {
        foreach (var item in hand)
            item.Value.isInteractuable = activate;
    }
    /// <summary>
    /// Permite obtener cartas desde el deck seleccionado.
    /// </summary>
    /// <param name="deck">Mazo del cual va a extraer cartas.</param>
    /// <param name="Ammount">Cantidad de cartas a extraer.</param>
    public void GetDrawedCards(Deck deck, int Ammount)
    {
        HandControl(false);

        //Independientemente de si estamos en el límite o no, debería sacar una carta extra.
        foreach (var item in deck.DrawCards(Ammount))
        {
            item.transform.SetParent(transform);
            hand.Add(item.DeckID, item);
            ToDrawCards.Add(item.transform);
        }

        StartCoroutine(DrawCards(deck));
    }
    /// <summary>
    /// Permite descartar una carta de la mano.
    /// </summary>
    /// <param name="idCard"></param>
    public void DiscardCardFromHand(int idCard)
    {
        if (hand.ContainsKey(idCard))
        {
            var carta = hand[idCard];
            //carta.comingBack = false;
            //carta.stopAll = true;
            carta.transform.SetParent(carta.discardPosition);
            hand.Remove(idCard);
        }
    }


    /// <summary>
    /// Recalcula las posiciones de cada elemento.
    /// </summary>
    private void CalculateHandFinalPositions()
    {
        _finalPositions.Clear();        

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
        _finalPositions.Add(firstElementPos);

        //Posicionamos todos los demás elementos.
        for (int i = 1; i < AmmountOfElements; i++)
        {
            Vector3 newElementPosition;
            if (InvertXPositioning)
                newElementPosition = firstElementPos - new Vector3(padding * i, 0, 0);
            else
                newElementPosition = firstElementPos + new Vector3(padding * i, 0, 0);
            _finalPositions.Add(newElementPosition);
        }
    }
    /// <summary>
    /// Posiciona Inmediatamente a las cartas en la posición final.
    /// </summary>
    public void PlaceElementsAtFinalPosition()
    {
        //Por ahora.
        for (int i = 0; i < ObjectsToAllign.Count; i++)
        {
            var currentElement = ObjectsToAllign[i];
            currentElement.transform.position = _finalPositions[i];
        }
    }

    //================================================== CORRUTINES =====================================================================

    IEnumerator DrawCards(Deck deck)
    {
        drawingCard = true;
        OnStartDraw();

        ToDrawCards[0].SetParent(transform);
        AmmountOfElements++;
        ObjectsToAllign.Insert(0, ToDrawCards[0]);
        deck.ExtractCard(); //Le aviso al deck que extraje una carta.
        CalculateHandFinalPositions();

        int elementsToLerp = ToDrawCards.Count;
        var currentCard = ToDrawCards[0];

        float currentLerpTime = 0;
        float AnimationFrameRate = 1 / FrameRate;

        float LerpRate = 0;
        print("Tiempo de la animación es:" + LerpRate);

        Vector3[] originalPositions = new Vector3[ObjectsToAllign.Count];
        for (int i = 0; i < ObjectsToAllign.Count; i++)
            originalPositions[i] = ObjectsToAllign[i].position;

        while (elementsToLerp > 0)
        {
            currentLerpTime += AnimationFrameRate;
            LerpRate = currentLerpTime / AnimationDuration;

            for (int i = 0; i < ObjectsToAllign.Count; i++)
            {
                var element = ObjectsToAllign[i];
                Vector3 finalPos = _finalPositions[i];

                if (element.transform.position != _finalPositions[i])
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
                    ObjectsToAllign.Insert(0, ToDrawCards[0]);
                    deck.ExtractCard(); //Le aviso al deck que extraje una carta.
                    CalculateHandFinalPositions();

                    originalPositions = new Vector3[ObjectsToAllign.Count];
                    for (int i = 0; i < ObjectsToAllign.Count; i++)
                        originalPositions[i] = ObjectsToAllign[i].position;
                }
            }
            yield return new WaitForSeconds(AnimationFrameRate);
        }

        drawingCard = false;
        OnEndDraw();
        HandControl(true);
    }
}