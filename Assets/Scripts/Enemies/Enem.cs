using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.RandomSelections;

public class Enem : Actor
{
    public Action OnEnemyDie = delegate { };

    //Referencias.
    [Header("Referencias")]
    public EnemyHUD HUD;
    public Player target;
    public GameObject GetHitPrefab;
    public GameObject GetHitParticleParent;

    //Estado y cosas
    [Header("Estado y Cosas")]
    public int cardAmmount = 20;

    //weas
    [Header("Weas")]
    public int HealAmmount;

    //animacion de cartas
    [Header("Animacion de Cartas")]
    public Transform ShowCard;
    public bool isShowing;
    public List<Card> cardsEnemy = new List<Card>();
    public GameObject deckEnemy;
    public Card cardSelected;

    //Pesos de cada acción.
    [Header("Pesos de cada accion")]
    public float AttackWeight;
    public float healWeight;

    //feedback camara
    [Header("Feedback Camara")]
    public cameraShaker shake;
    public AudioSource au;
    public AudioClip dmg;
    public AudioClip hlth;


    bool _canExecuteActionM = false;
    bool executedAction = false;
    bool canEndTurn = false;

    public override bool CanExecuteActions
    {
        get => _canExecuteActionM;
        set
        {
            _canExecuteActionM = value;
            canEndTurn = _canExecuteActionM;
        }
    }


    private void Awake()
    {
        au = GetComponent<AudioSource>();
        target = FindObjectOfType<Player>();
        for (int i = 0; i < deckEnemy.transform.childCount; i++)
            cardsEnemy.Add(deckEnemy.transform.GetChild(i).GetComponent<Card>());
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
                heal(HealAmmount);
                break;
            default:
                break;
        }

        StartCoroutine(DelayedEndTurn(2f));
    }

    public override void UpdateTurn()
    {
        //print("UpdateEnemigo");
        OnUpdateTurn();


        if (isShowing)
        {
            if (Vector3.Distance(cardSelected.transform.position, ShowCard.transform.position)>= 1f)
            {
                cardSelected.transform.position = Vector3.Lerp(cardSelected.transform.position, ShowCard.transform.position, Time.deltaTime);
                cardSelected.anim.Play("EnemyAttack");
            }
            else
            {
                cardSelected.transform.parent = null;
                isShowing = false;
                cardSelected = null;
            }
        }
        //Terminamos el turno.
        //if (canEndTurn)
        //    EndTurn();
    }

    public override void EndTurn()
    {
        base.EndTurn();
        OnEndTurn(this);

        CanExecuteActions = false;
        canEndTurn = false;
    }

    //====================================================== Actions =============================================================

    public void AttackToTarget()
    {
        //Activo la animación o lo que sea.
        print(string.Format("{0} Ejecutó la acción: {1}", ActorName, "Atacar"));
        target.GetDamage(Damage);

        cardSelected = cardsEnemy[UnityEngine.Random.Range(0, cardsEnemy.Count)];
        isShowing = true;

        cardAmmount--;
        HUD.cardsDisplay = cardAmmount;
    }

    //============================================================================================================================

    public override void GetDamage(int damage)
    {
        // Aplico resistencias.
        // Hago el cálculo de daño recibido.
        Health -= damage;
        StartCoroutine(shake.Shake(.30f, 0.9f));
        au.clip = dmg;
        au.Play();
        var particle = Instantiate(GetHitPrefab, GetHitParticleParent.transform, false);
        Destroy(particle, 3f);
        HUD.healthDisplay = Health;

        CombatManager.match.FeedbackHUD.SetDamage("Daño Infligido:", damage);
        CombatManager.match.HUDAnimations.SetTrigger("PlayerMakesDamage");
        if (Health <= 0)
            OnEnemyDie();
    }

    public override void heal(int Ammount)
    {
        //Me curo.
        print(string.Format("{0} Ejecutó la acción: {1}", ActorName, "Curar"));
        Health += Ammount;
        Health = Mathf.Clamp(Health, 0, maxHealth);
        HUD.healthDisplay = Health;
        au.clip = hlth;
        au.Play();

        cardSelected = cardsEnemy[UnityEngine.Random.Range(0, cardsEnemy.Count)];
        isShowing = true;

        CombatManager.match.FeedbackHUD.SetHeal("Recuperación:", Ammount);
        CombatManager.match.HUDAnimations.SetTrigger("EnemyGetsHealed");

        cardAmmount--;
        HUD.cardsDisplay = cardAmmount;
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
