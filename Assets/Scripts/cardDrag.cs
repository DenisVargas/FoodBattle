using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Usar raycast para obtener el punto en el espacio en el que el mouse se esta posando.
//Si golpea a una carta, este activa su HoverFunction.
//Si cliqueamos mientras la carta esta en hover.
//  La carta empieza a seguir la posición del mouse, y ya no es collisionable.

public class cardDrag : MonoBehaviour
{
    public GameObject debugPositionObject;
    public LayerMask LM_GameElements;
    public bool Dragging;

    #region Context

    Vector3 mousePositionInWorld = Vector3.zero;
    Card currentCard = null;
    CardSlot currentSlot = null; 

    #endregion

    //Selection.
    Card selectedCard = null;
    CardSlot selectedSlot = null;
    Vector3 FieldPosition = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        //Obtengo el contexto actual del mouse.
        PullContext();

        //Selection.
        if (Input.GetMouseButtonDown(0))
            Selection();
        if (Input.GetMouseButtonUp(0))
            Deselection();

        if (Dragging)
            selectedCard.FollowTarget(mousePositionInWorld);
    }

    /// <summary>
    /// Realiza un raycast múltiple detectando todos los elementos de juego bajo el mouse.
    /// </summary>
    private void PullContext()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, float.MaxValue, LM_GameElements);

        print(string.Format("Se encontraron {0} objetos", hits.Length));

        #region Posición del Mouse en el Tablero
        RaycastHit hit = hits.Where(h => h.transform.tag == "GameField")
                        .FirstOrDefault();

        debugPositionObject.transform.position = hit.point; // Objeto en el campo.
        mousePositionInWorld = hit.point; // Posición del Mouse 
        #endregion

        #region Detección de Cartas
        Card findedCard = hits.Where(h => h.transform.GetComponent<Card>() != null)
                          .Select(h => h.transform.GetComponent<Card>())
                          .FirstOrDefault();

        if (currentCard != null)
            currentCard.MouseHoverEnd();
        if (findedCard != null)
            findedCard.MouseHoverStart();

        currentCard = findedCard;
        #endregion

        #region Slots Cartas
        CardSlot findedS = hits.Where(h => h.transform.GetComponent<CardSlot>())
                               .Select(h => h.transform.GetComponent<CardSlot>())
                               .DefaultIfEmpty(null)
                               .First();

        if (currentSlot != null)
            currentSlot.MouseHoverEnd();

        if (findedS != null)
            findedS.MouseHoverStart();

        currentSlot = findedS;
        #endregion
    }

    public void Selection()
    {
        print("Selecciono la wea");
        if (currentCard != null)
        {
            Dragging = true;
            selectedCard = currentCard;
            selectedCard.MouseSelect();
            selectedCard.FollowTarget(mousePositionInWorld);
        }
    }

    private void Deselection()
    {
        print("Deselecciono la wea");

        if (currentSlot != null && Dragging && !currentSlot.isLocked)
        {
            currentSlot.AssignCard(selectedCard);
        }

        if (selectedCard != null)
        {
            Dragging = false;
            selectedCard.MouseRelease();
            selectedCard = null;
        }
    }

    public void ActivateCard()
    {

    }
}
