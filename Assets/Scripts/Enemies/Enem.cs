using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.RandomSelections;

public class Enem : Actor
{
    //Referencias.
    public EnemyHUD HUD;
    public Player target;

    //Estado y cosas
    public int cardAmmount = 20;

    //weas
    public int HealAmmount;

    //Pesos de cada acción.
    public float AttackWeight;
    public float healWeight;

    bool executedAction = false;
    bool canEndTurn = false;

    private void Awake()
    {
        target = FindObjectOfType<Player>();
        Health = maxHealth;

        HUD.SetRivalName(ActorName);
        HUD.healthDisplay = Health;
        HUD.cardsDisplay = cardAmmount;
    }

    public override void StartTurn()
    {
        //Decido que carajos hacer con mi vida.
        print("Empezó el Turno del enemigo");
        Decide();
        OnStartTurn(this);
    }

    public void Decide()
    {
        // Calculamos el peso de cada acción. --> A futuro porque ahora solo tenemos 2 Acciones we


        //Ejecutamos la desición.
        List<float> posibilities = new List<float> { AttackWeight, healWeight };
        int decition = RoulleteSelection.Roll(posibilities);

        switch (decition)
        {
            case 0:
                AttackToTarget();
                break;
            case 1:
                SelfHeal(HealAmmount);
                break;
            default:
                break;
        }
    }

    public override void UpdateTurn()
    {
        //print("UpdateEnemigo");
        base.UpdateTurn();
        OnUpdateTurn();

        //Terminamos el turno.
        if (canEndTurn)
            EndTurn();
    }

    public override void EndTurn()
    {
        base.EndTurn();
        OnEndTurn(this);
        canEndTurn = false;
        print("Turno del Player");
    }

    //====================================================== Actions =============================================================

    public void AttackToTarget()
    {
        //Activo la animación o lo que sea.
        print("Ataco");
        target.GetDamage(Damage);

        cardAmmount--;
        HUD.cardsDisplay = cardAmmount;
    }

    public void SelfHeal(int ammount)
    {
        //Me curo.
        print("Me curo");
        Health += ammount;
        HUD.healthDisplay = Health;

        cardAmmount--;
        HUD.cardsDisplay = cardAmmount;
    }

    public void AnimEvent_ActionEnded()
    {
        canEndTurn = true;
    }

    //============================================================================================================================

    public override void GetDamage(int damage)
    {
        // Aplico resistencias.
        // Hago el cálculo de daño recibido.

        Health -= damage;
        HUD.healthDisplay = Health;
    }
}
