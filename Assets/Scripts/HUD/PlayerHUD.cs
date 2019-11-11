using System.Collections;
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

    [SerializeField] Image _buffDamageSlot = null;
    [SerializeField] TMP_Text _buffDamage = null;
    [SerializeField] Image _buffArmourSlot = null;
    [SerializeField] TMP_Text _buffArmor = null;

    public GameObject EndTurnButton;

    private void Awake()
    {
        SetBuffDisplay(BuffType.ArmourIncrease, false, 0.1f);
        SetBuffDisplay(BuffType.DamageIncrease, false, 0.1f);
    }

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

    public void SetBuffDisplay(BuffType type, bool On, float FadeTime = 1f)
    {
        float alphaValue = On ? 1f : 0;

        switch (type)
        {
            case BuffType.ArmourIncrease:
                _buffArmourSlot.CrossFadeAlpha(alphaValue, FadeTime, false);//Icono.
                _buffArmor.CrossFadeAlpha(alphaValue, FadeTime, false);   //Texto.
                break;
            case BuffType.DamageIncrease:
                _buffDamageSlot.CrossFadeAlpha(alphaValue, FadeTime, false);//Icono.
                _buffDamage.CrossFadeAlpha(alphaValue, FadeTime, false);    //Texto.
                break;
            case BuffType.CardCostDecrease:
                //Falta implementar.
                break;
            case BuffType.NullyfyCardCost:
                //Falta implementar.
                break;
            case BuffType.Invulnerability:
                //Falta implementar.
                break;
            default:
                break;
        }
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
