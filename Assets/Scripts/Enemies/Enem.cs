using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enem : Actor
{
    //Referencias.
    public EnemyHUD HUD;

    //Estado y cosas

    //y mas weas.

    private void Awake()
    {
        Health = maxHealth;

        HUD.SetRivalName(ActorName);
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

    public override void GetDamage(int damage)
    {
        // Aplico resistencias.
        // Hago el cálculo de daño recibido
    }

    //Esto va a depender del diseño de los efectos de las cartas.
    //public void ApplyCardEffect()
    //{

    //}
}
