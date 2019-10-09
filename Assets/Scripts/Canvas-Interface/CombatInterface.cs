using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatInterface : MonoBehaviour
{
    //[SerializeField] TMP_Text _playerLife     = null;
    public CanvasHealthBarr healthBarr;
    [SerializeField] TMP_Text _remaingActions = null;
    [SerializeField] TMP_Text _usedCards      = null;
    [SerializeField] TMP_Text _remainingCards = null;

    [SerializeField] Button EndTurnButton     = null;

    public float PlayerLife
    {
        set
        {
            //_playerLife.text = "Lifes: " + value; }
            healthBarr.UpdateDisplay(value);
        }
    }
    public string RemainingActions
    {
        set { _remaingActions.text = value; }
    }
    public string UsedCards
    {
        set { _usedCards.text = value; }
    }
    public string RemainingCards
    {
        set { _remainingCards.text = value; }
    }

    /// <summary>
    /// Activa o desactiva el boton de termino de turno en el Canvas.
    /// </summary>
    /// <param name="Enable"></param>
    public void ShowEndTurnButton(bool Enable)
    {
        EndTurnButton.gameObject.SetActive(Enable);
        //Alternativa
        //EndTurnButton.interactable = Enable;
    }
}
