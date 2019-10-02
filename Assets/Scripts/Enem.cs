using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enem : Actor
{
    public WorldSpaceHealthBar healthBar;

    //Estado y cosas
    public int Health;
    public int MaxHealth;
    public float AttackDamage;
    //y mas weas.

    public void Decide()
    {
        //Aca ponemos un montón de ifs XD

        //Ejecutamos la desición.

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
}
