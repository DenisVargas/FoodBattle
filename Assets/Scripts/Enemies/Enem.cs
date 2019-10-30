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
    public Transform discardDeck;
    public int cardStateShow;
    public List<Card> cardsEnemy = new List<Card>();
    public Transform handEnemy;
    public Transform enemyDeck;
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
    public AudioClip turn;


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
        for (int i = 0; i < handEnemy.transform.childCount; i++)
            cardsEnemy.Add(handEnemy.transform.GetChild(i).GetComponent<Card>());
        cardStateShow = 0;
        Health = maxHealth;

        HUD.SetRivalName(ActorName);
        HUD.healthDisplay = Health;
        HUD.cardsDisplay = cardAmmount;
        hand.AlingCards();
        deck.LoadAllCards();
    }

    public override void StartTurn()
    {
        //Decido que carajos hacer con mi vida.
        OnStartTurn();
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
    }

    public override void UpdateTurn()
    {
        //print("UpdateEnemigo");
        OnUpdateTurn();


        if (cardStateShow == 1)
        {
            if (Vector3.Distance(cardSelected.transform.position, ShowCard.transform.position)>= 1f)
            {
                cardSelected.transform.position = Vector3.Lerp(cardSelected.transform.position, ShowCard.transform.position, Time.deltaTime);
                cardSelected.anim.Play("EnemyAttack");
            }
            else
            {
                cardSelected.canBeShowed = true;
                StartCoroutine(WaitCard(3f));
            }
        }
        else if (cardStateShow == 2)
        {
            if (Vector3.Distance(cardSelected.transform.position, discardDeck.transform.position) >= 1f)
                cardSelected.transform.position = Vector3.Lerp(cardSelected.transform.position, discardDeck.transform.position, Time.deltaTime * 5);
            else
                StartCoroutine(WaitCardToDeck(2f));
        }
        else if (cardStateShow == 3)
        {
            if (Vector3.Distance(cardSelected.transform.position, handEnemy.transform.position) >= 1f)
                cardSelected.transform.position = Vector3.Lerp(cardSelected.transform.position, handEnemy.transform.position, Time.deltaTime * 5);
            else
            {
                cardSelected.canBeShowed = false;
                cardSelected.Stats = null;
                hand.AlingCards();
                cardSelected = null;
                cardStateShow = 0;
                StartCoroutine(DelayedEndTurn(2f));
            }
        }
        //Terminamos el turno.
        //if (canEndTurn)
        //    EndTurn();
    }

    public override void EndTurn()
    {
        OnEndTurn(this);

        //Feedback
        au.clip = turn;
        au.Play();

        CanExecuteActions = false;
        canEndTurn = false;
    }

    //====================================================== Actions =============================================================

    public void AttackToTarget()
    {
        //Activo la animación o lo que sea.
        print(string.Format("{0} Ejecutó la acción: {1}", ActorName, "Atacar"));

        var randomNumber = UnityEngine.Random.Range(0, cardsEnemy.Count);
        cardSelected = cardsEnemy[randomNumber];
        cardsEnemy.RemoveAt(randomNumber);
        cardSelected.Stats = (CardData)Resources.Load("Cards/001");
        cardStateShow = 1;
        cardSelected.LoadCardDisplayInfo();
        target.GetDamage(cardSelected.Stats.damage);

        cardAmmount--;
        HUD.cardsDisplay = cardAmmount;
    }

    //============================================================================================================================

    public override void GetDamage(int damage)
    {
        // Aplico resistencias.
        // Hago el cálculo de daño recibido.
        Health -= damage;

        //Feedback
        HUD.healthDisplay = Health;
        StartCoroutine(shake.Shake(.30f, 0.9f));
        var particle = Instantiate(GetHitPrefab, GetHitParticleParent.transform, false);
        Destroy(particle, 3f);
        CombatManager.match.FeedbackHUD.SetDamage("Daño Infligido:", damage);
        CombatManager.match.HUDAnimations.SetTrigger("PlayerMakesDamage");
        au.clip = dmg;
        au.Play();

        //Condición de victoria para el Player.
        if (Health <= 0)
        {
            //Digo que no puedo terminar mi turno.
            OnEnemyDie();
        }
    }

    public override void heal(int Ammount)
    {
        cardSelected = cardsEnemy[UnityEngine.Random.Range(0, cardsEnemy.Count)];
        cardStateShow = 1;
        cardSelected.Stats = (CardData)Resources.Load("Cards/003");
        cardSelected.LoadCardDisplayInfo();

        //Me curo.
        print(string.Format("{0} Ejecutó la acción: {1}", ActorName, "Curar"));
        Health += cardSelected.Stats.buffAmmount;
        Health = Mathf.Clamp(Health, 0, maxHealth);
        HUD.healthDisplay = Health;
        au.clip = hlth;
        au.Play();


        CombatManager.match.FeedbackHUD.SetHeal("Recuperación:", cardSelected.Stats.buffAmmount);
        CombatManager.match.HUDAnimations.SetTrigger("EnemyGetsHealed");

        cardAmmount--;
        HUD.cardsDisplay = cardAmmount;
    }

    IEnumerator WaitCard(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        cardStateShow = 2;
    }

    IEnumerator WaitCardToDeck(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        cardSelected.anim.SetTrigger("BackToIdle");
        cardSelected.transform.position = enemyDeck.transform.position;
        cardSelected.transform.rotation = Quaternion.Euler(new Vector3(-45, 180, 0));
        cardStateShow = 3;
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
