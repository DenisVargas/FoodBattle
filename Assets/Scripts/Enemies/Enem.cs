using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.RandomSelections;

public class Enem : Actor
{
    public Action OnEnemyDie = delegate { };

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

    //feedback camara
    public cameraShaker shake;

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
        OnStartTurn(this);
        StartCoroutine(DelayedChoose(1.5f));
    }

    public void Decide()
    {
        // Calculamos el peso de cada acción. --> A futuro porque ahora solo tenemos 2 Acciones we
        float healImportance =  (1 - (Health / maxHealth)) * healWeight; // Cuanto menos vida tenga, mas alta es su importancia.

        //Ejecutamos la desición.
        List<float> posibilities = new List<float> { AttackWeight, healImportance };
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

        StartCoroutine(DelayedEndTurn(2f));
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
    }

    //====================================================== Actions =============================================================

    public void AttackToTarget()
    {
        //Activo la animación o lo que sea.
        print(string.Format("{0} Ejecutó la acción: {1}", ActorName, "Atacar"));
        target.GetDamage(Damage);

        cardAmmount--;
        HUD.cardsDisplay = cardAmmount;
    }

    public void SelfHeal(int ammount)
    {
        //Me curo.
        print(string.Format("{0} Ejecutó la acción: {1}", ActorName, "Curar"));
        Health += ammount;
        Health = Mathf.Clamp(Health, 0, maxHealth);
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
        StartCoroutine(shake.Shake(.30f, 0.9f));
        HUD.healthDisplay = Health;

        if (Health <= 0)
            OnEnemyDie();
    }

    IEnumerator DelayedChoose(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Decide();
    }

    IEnumerator DelayedEndTurn(float Seconds)
    {
        yield return new WaitForSeconds(Seconds);
        EndTurn();
    }
}
