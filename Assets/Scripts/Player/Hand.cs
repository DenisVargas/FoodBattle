using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Es la "Mano del jugador"
public class Hand : MonoBehaviour
{
    public Actor Owner;

    public Dictionary<int, Card> handCards;                   // Contiene las cartas ordenadas por su "UniqueID".
    public List<Transform> ToDrawCards;                  // Representación del deck real (Componente Transform de cada carta en la mano).
    List<Vector3> _finalPositions;                       // Posiciones finales calculadas para cada de los elementos dentro de "ToDrawCards"
    public List<Transform> ObjectsToAllign;              // Se autorrellena al ejecutar DrawCards. Esta es la lista de cartas que ya está en la mano, si se agrega una carta nueva, se desplazan a un costado.
    List<Vector3> _currentPositions;


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

    //========================= Animación ===============================
    bool _drawingCards = false;
    int elementsToLerp = 0;
    float currentLerpTime = 0;
    float ScaledFrameRate = 0;
    float LerpRate = 0;

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
        handCards = new Dictionary<int, Card>();
        ObjectsToAllign = new List<Transform>();
        ToDrawCards = new List<Transform>();
        _finalPositions = new List<Vector3>();
        _currentPositions = new List<Vector3>();

        ScaledFrameRate = 1 / FrameRate;
        center = transform.position;
    }

    //================================================== MEMBER FUNCS ===================================================================

    /// <summary>
    /// Activa o desactiva la interactividad.
    /// </summary>
    /// <param name="activate">Activar o desactivar?</param>
    public void HandControl(bool activate)
    {
        if (handCards == null) return;

        foreach (var item in handCards)
            item.Value.isInteractuable = activate;
    }
    /// <summary>
    /// Permite obtener cartas desde el deck seleccionado.
    /// </summary>
    /// <param name="deck">Mazo del cual va a extraer cartas.</param>
    /// <param name="Ammount">Cantidad de cartas a extraer.</param>
    public void GetDrawedCards(Deck deck, int Ammount)
    {
        //Independientemente de si estamos en el límite o no, debería sacar una carta extra.
        foreach (var item in deck.DrawCards(Ammount))
        {
            item.transform.SetParent(transform);
            handCards.Add(item.DeckID, item);
            ToDrawCards.Add(item.transform);
        }

        HandControl(false);

        Debug.LogWarning(string.Format("================ Inicio la animación para {0} ================", Owner.name));

        _drawingCards = true;
        Owner.OnStartDraw();
        SetNewLerpElement();
        StartCoroutine(DrawCards(deck));
    }
    private void SetNewLerpElement()
    {
        Transform card = ToDrawCards[0]; //Siempre tomo el primer elemento de ToDrawCards.
        card.SetParent(transform);
        //ToDrawCards.RemoveAt(0); // Remuevo el elemento cada vez que la animación concluye.
        Owner.OnExtractCard(); //Le aviso al deck que extraje una carta.

        ObjectsToAllign.Insert(0, card);
        elementsToLerp = ObjectsToAllign.Count;

        GetOriginalPositions();
        CalculateHandFinalPositions();
    }

    /// <summary>
    /// Permite descartar una carta de la mano.
    /// </summary>
    /// <param name="idCard"></param>
    public void DiscardCardFromHand(int idCard)
    {
        if (handCards.ContainsKey(idCard))
        {
            var carta = handCards[idCard];
            //carta.comingBack = false;
            //carta.stopAll = true;
            carta.transform.SetParent(carta.discardPosition);
            handCards.Remove(idCard);
        }
    }
    /// <summary>
    /// Recalcula las posiciones de cada elemento.
    /// </summary>
    private void CalculateHandFinalPositions()
    {
        _finalPositions.Clear(); //Posiciones originales calculadas anteriormente.
        float AmmountOfElements = ObjectsToAllign.Count; //Elementos existentes en la mano + el nuevo.

        float padding = (DistanceLimit * 2) / AmmountOfElements; // Distancia entre cada elemento.

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
    public void GetOriginalPositions()
    {
        _currentPositions = ObjectsToAllign
                            .Select(ta => ta.position)
                            .ToList();
    }

    //================================================== CORRUTINES =====================================================================

    IEnumerator DrawCards(Deck deck)
    {
        while (ToDrawCards.Count > 0)
        {
            currentLerpTime += ScaledFrameRate;              // Aumenta el tiempo de la animación.
            LerpRate = currentLerpTime / AnimationDuration;     // Calcula el porcentaje de completado de la animación.

            for (int i = 0; i < ObjectsToAllign.Count; i++)
            {
                var element = ObjectsToAllign[i];
                Vector3 finalPos = _finalPositions[i];

                if (element.transform.position != _finalPositions[i])
                {
                    try
                    {
                        //Hago el lerp de las posiciones.
                        Vector3 lerpedPosition = Vector3.Lerp(_currentPositions[i], finalPos, LerpCurve.Evaluate(LerpRate));
                        element.transform.position = lerpedPosition;
                        print("Hasta acá todo OKiDOkis");
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Debug.LogWarning("El owner es:" + Owner.ActorName);
                        Debug.LogError(string.Format("Elementos a lerpear {0}, Objetos a alinear {1}, cantidad de posiciones finales {2}, cantidad de posiciones originales {3}", elementsToLerp, ObjectsToAllign.Count, _finalPositions.Count, _currentPositions.Count));
                    }
                }
            }

            if (currentLerpTime >= AnimationDuration) //Alternativa posA = posB
            {
                currentLerpTime = 0;
                ToDrawCards.RemoveAt(0); //Al final del la animación sacamos el elemento.
                elementsToLerp--;

                print(string.Format("A Cartas a obtener {0}, elementsToLerp value is {1}", ToDrawCards.Count, elementsToLerp));

                if (ToDrawCards.Count > 0)
                    SetNewLerpElement();
            }
            yield return new WaitForSeconds(ScaledFrameRate);
        }

        print("Termine las animaciones.");
        _drawingCards = false;
        HandControl(true);
        Owner.OnEndDraw();
    }
}