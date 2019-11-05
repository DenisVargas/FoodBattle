using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ActionFeedbackHUD : MonoBehaviour
{
    public event Action<bool> InformExecuteActions = delegate { };

    public TMP_Text Titulo_Daño;
    public TMP_Text Cantidad_Daño;
    public TMP_Text Titulo_Energia;
    public TMP_Text Cantidad_Energia;
    public TMP_Text Titulo_Recuperacion;
    public TMP_Text Cantidad_Recuperacion;
    public TMP_Text Titulo_Turnos;
    public TMP_Text Cantidad_Turnos;
    public TMP_Text Titulo_BuffAtaque;
    public TMP_Text Buff_Daño;
    public TMP_Text Titulo_BuffArmor;
    public TMP_Text Buff_Armor;

    string[] originalValues;
    private void Awake()
    {
        originalValues = new string[12]
        {
            Titulo_Daño.text,
            Cantidad_Daño.text,
            Titulo_Energia.text,
            Cantidad_Energia.text,
            Titulo_Recuperacion.text,
            Cantidad_Recuperacion.text,
            Titulo_Turnos.text,
            Cantidad_Turnos.text,
            Titulo_BuffAtaque.text,
            Buff_Daño.text,
            Titulo_BuffArmor.text,
            Buff_Armor.text
        };
    }

    public void SetDamage(string Title, int Ammount)
    {
        Titulo_Daño.text = Title;
        Cantidad_Daño.text = Ammount.ToString();
    }
    public void SetEnergy(string Title, int Ammount)
    {
        Titulo_Energia.text = Title;
        Cantidad_Energia.text = Ammount.ToString();
    }
    public void SetHeal(string Title, int Ammount)
    {
        Titulo_Recuperacion.text = Title;
        Cantidad_Recuperacion.text = Ammount.ToString();
    }
    public void SetTurns(string Title, int Ammount)
    {
        Titulo_Turnos.text = Title;
        Cantidad_Turnos.text = Ammount.ToString();
    }
    public void SetBuffAttack(string Title, int Ammount)
    {
        Titulo_BuffAtaque.text = Title;
        Buff_Daño.text = Ammount.ToString();
    }
    public void SetBuffArmor(string Title, int Ammount)
    {
        Titulo_BuffArmor.text = Title;
        Buff_Armor.text = Ammount.ToString();
    }

    public void ResetDisplay()
    {
        Titulo_Daño.text = originalValues[0];
        Cantidad_Daño.text = originalValues[1];

        Titulo_Energia.text = originalValues[2];
        Cantidad_Energia.text = originalValues[3];

        Titulo_Recuperacion.text = originalValues[4];
        Cantidad_Recuperacion.text = originalValues[5];

        Titulo_Turnos.text = originalValues[6];
        Cantidad_Turnos.text = originalValues[7];
    }

    public void AnimEvent_StartActionFeedback()
    {
        //No Puedo seguir con el loop del juego.
        //print(string.Format("No Puede Ejecutar acciones\nAnimEvent_StartActionFeedback() desde ActionFeedbackHud --> CombatManager"));
        InformExecuteActions(false);
    }

    public void AnimEvent_EndActionFeedback()
    {
        //Puedo seguir con el loop del juego.
        //print(string.Format("Puede Ejecutar acciones\nAnimEvent_EndActionFeedback desde ActionFeedbackHud --> CombatManager"));
        InformExecuteActions(true);
    }
}
