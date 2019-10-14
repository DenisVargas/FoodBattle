using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enem : Actor
{
    //Referencias.
    public EnemyHUD HUD;

    //Estado y cosas
    public string RivalName = "Enemy";
    public int Health;
    public int MaxHealth;
    public float AttackDamage;
    //y mas weas.

    private void Awake()
    {
        HUD.SetRivalName(RivalName);
        HUD.healthDisplay = Health;
        HUD.cardsDisplay = 20f;
    }

    public void Decide()
    {
        //Aca ponemos un montón de ifs XD

        //Ejecutamos la desición.

        print("Turno del enemigo");

        //Terminamos el turno.
        EndTurn();
    }

    public override void StartTurn()
    {
        //Decido que carajos hacer con mi vida.
        Decide();
        OnStartTurn(this);
    }

    public override void UpdateTurn()
    {
        base.UpdateTurn();
        OnUpdateTurn();
    }

    public override void EndTurn()
    {
        base.EndTurn();
        OnEndTurn(this);
    }

    //Esto va a depender del diseño de los efectos de las cartas.
    //public void ApplyCardEffect()
    //{

    //}
}
