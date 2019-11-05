﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] TMP_Text _playerName = null;
    [SerializeField] TMP_Text _playerLife     = null;
    [SerializeField] TMP_Text _remaingActions = null;
    [SerializeField] TMP_Text _usedCards      = null;
    [SerializeField] TMP_Text _remainingCards = null;
    [SerializeField] TMP_Text _buffDamage = null;
    [SerializeField] TMP_Text _buffArmor = null;

    public GameObject EndTurnButton;


    public float PlayerLife
    {
        set { _playerLife.text = value.ToString(); }
    }
    public int RemainingActions
    {
        set { _remaingActions.text = value.ToString(); }
    }
    public int UsedCards
    {
        set { _usedCards.text = value.ToString(); }
    }
    public int RemainingCards
    {
        set { _remainingCards.text = value.ToString(); }
    }

    public int SetBuffDamage
    {
        set { _buffDamage.text = value.ToString(); }
    }

    public int SetBuffArmor
    {
        set { _buffArmor.text = value.ToString(); }
    }

    public void SetPlayerName(string playerName)
    {
        _playerName.text = playerName;
    }
    /// <summary>
    /// Activa o desactiva el boton de termino de turno en el Canvas.
    /// </summary>
    /// <param name="Enable"></param>
    public void ShowEndTurnButton(bool Enable)
    {
        //Esto podríamos animarlo.
        EndTurnButton.SetActive(Enable);
        //Alternativa
        //EndTurnButton.interactable = Enable;
    }
}
