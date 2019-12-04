using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.RandomSelections;

/*
 * NOTAS: 
 * Por ahora el enemigo solo toma desiciones limitadas, no usa el deck propio.
 */

public class Enem : Actor
{
    public Action OnEnemyDie = delegate { };

    //Referencias.
    [Header("Referencias")]
    public EnemyHUD HUD;
    public Player target;
    public GameObject GetHitPrefab;
    public GameObject GetHitParticleParent;
    public GameObject TurnEnem;

    //weas
    [Header("Weas")]
    public int HealAmmount;

    //animacion de cartas
    [Header("Animacion de Cartas")]
    public Transform ShowCard;
    public Transform fromDeckToHand;
    public int cardStateShow;
    public List<Card> cardsEnemy = new List<Card>();
    public Transform handEnemy;
   // public Transform discardDeck;
    public Card cardSelected;
    public List<Card> inDeck = new List<Card>();
    public bool toHand = false;

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


    public override void Awake()
    {
        base.Awake();
        TurnEnem.SetActive(!enabled);
        au = GetComponent<AudioSource>();
        target = FindObjectOfType<Player>();
        for (int i = 0; i < handEnemy.transform.childCount; i++)
            cardsEnemy.Add(handEnemy.transform.GetChild(i).GetComponent<Card>());
        cardStateShow = 0;
        Health = maxHealth;

        HUD.SetRivalName(ActorName);
        HUD.healthDisplay = Health;
        HUD.EnergyDisplay = Energy;
        hand.AlingCards();
        //deck.LoadAllCards(); todavia no se usa pero hay que ponerlo en algun momento, si lo pongo tira un error por algo de la seleccion del deck
    }

    public override void StartTurn(int turnEnergy)
    {
        //Decido que carajos hacer con mi vida.
        OnStartTurn(this);
        TurnEnem.SetActive(enabled);
        Energy = turnEnergy;
        //StartCoroutine(DelayedChoose(1.5f));
        cardStateShow = 3;
    }

    public void Decide()
    {
        if (hand.transform.childCount == 0)
            StartCoroutine(DelayedEndTurn(1.5f));
        else
        {
            // Calculamos el peso de cada acción. --> A futuro porque ahora solo tenemos 2 Acciones we
            float healImportance = (1 - (Health / maxHealth)) * healWeight; // Cuanto menos vida tenga, mas alta es su importancia.

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
    }

    public override void UpdateTurn()
    {
        //print("UpdateEnemigo");
        OnUpdateTurn();
        //del deck a la mano
        if (toHand)
        {
            if (inDeck.Count > 0)
            {
                if (Vector3.Distance(inDeck[0].transform.position, handEnemy.transform.position) >= 1f)
                    inDeck[0].transform.position = Vector3.Lerp(inDeck[0].transform.position, handEnemy.transform.position, Time.deltaTime);
                else
                {
                    cardsEnemy.Add(inDeck[0]);
                    inDeck[0].transform.SetParent(handEnemy.transform);
                    inDeck[0].shaderStart = false;
                    inDeck[0].canvas.SetActive(true);
                    inDeck[0].anim.enabled = true;
                    foreach (var i in inDeck[0].objetos)
                    {
                        i.SetActive(true);
                    }
                    inDeck[0].transform.rotation = Quaternion.Euler(handEnemy.transform.rotation.x, handEnemy.transform.rotation.y, handEnemy.transform.rotation.z);
                    inDeck[0].Stats = null;
                    inDeck.Remove(inDeck[0]);
                }
            }
            else
            {
                hand.AlingCards();
                if (hand.transform.childCount > 0)
                {
                    if (Energy > 0)
                        StartCoroutine(DelayedChoose(1f));
                }
                else
                    StartCoroutine(DelayedEndTurn(1.5f));
                toHand = false;
            }
        }
        //de la mano a la mesa
        if (cardStateShow == 1)
        {
            if (Vector3.Distance(cardSelected.transform.position, ShowCard.transform.position) >= 1f)
                cardSelected.transform.position = Vector3.Lerp(cardSelected.transform.position, ShowCard.transform.position, Time.deltaTime);
            else
            {
                cardSelected.canBeShowed = true;
                StartCoroutine(WaitCard(2f));
            }
        }
        //de la mesa al descarte
        else if (cardStateShow == 2)
        {
            cardSelected.canBeShowed = false;
            cardSelected.shaderStart = true;
            cardSelected.canvas.SetActive(false);
            foreach (var i in cardSelected.objetos)
            {
                i.SetActive(false);
            }
            if (cardSelected.shaderLerp <= 0)
            {
                cardStateShow = 0;
                if (Energy > 0)
                    StartCoroutine(DelayedChoose(1f));
                else
                    StartCoroutine(DelayedEndTurn(1.5f));
                cardSelected.transform.SetParent(fromDeckToHand.transform);
                cardSelected.transform.position = fromDeckToHand.transform.position;
                //cardSelected.transform.rotation = Quaternion.Euler(new Vector3(-45, 0, 0));
                cardSelected = null;
            }

        }
        //del deck a la mano
        else if (cardStateShow == 3)
        {
            if (fromDeckToHand.transform.childCount == 0)
            {
                cardStateShow = 0;
                StartCoroutine(DelayedChoose(1.5f));
            }
            else
            {
                for (int i = 0; i < fromDeckToHand.childCount; i++)
                {
                    inDeck.Add(fromDeckToHand.GetChild(i).GetComponent<Card>());
                    inDeck[i].shaderLerp = 100;
                    inDeck[i].ShadersOP(100);
                    inDeck[i].anim.SetTrigger("BackToIdle");
                    inDeck[i].anim.enabled = false;
                    inDeck[i].transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));

                }
                cardStateShow = 0;
                toHand = true;
            }
        }
    }

    public override void EndTurn()
    {
        OnEndTurn(this);

        Energy = 0;
        TurnEnem.SetActive(!enabled);
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

        var randomNumber = UnityEngine.Random.Range(0, cardsEnemy.Count);
        cardSelected = cardsEnemy[randomNumber];
        cardSelected.Stats = CardDatabase.GetCardData(1);
        cardSelected.discardPosition = fromDeckToHand;
        if (Energy >= cardSelected.Stats.Cost)
        {
            cardSelected.anim.SetTrigger("EnemyAttack");
            cardsEnemy.Remove(cardSelected);
            print(string.Format("{0} Ejecutó la acción: {1}", ActorName, "Atacar"));
            cardStateShow = 1;
            cardSelected.LoadCardDisplayInfo();
            target.GetDamage(cardSelected.Stats.GetDebuff(DeBuffType.healthReduction).Ammount);

            Energy -= cardSelected.Stats.Cost;
            HUD.EnergyDisplay = Energy;
        }
        else
            StartCoroutine(DelayedEndTurn(2f));

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

    protected override void DestroyArmour()
    {
        print("Cierto que el enemigo no tiene esta lógica xD");
    }

    protected override void heal(int Ammount)
    {
        cardSelected = cardsEnemy[UnityEngine.Random.Range(0, cardsEnemy.Count)];
        cardSelected.Stats = (CardData)Resources.Load("Cards/003");
        cardSelected.LoadCardDisplayInfo();
        cardSelected.discardPosition = fromDeckToHand;
        if (Energy >= cardSelected.Stats.Cost)
        {
            cardSelected.anim.SetTrigger("EnemyAttack");
            cardsEnemy.Remove(cardSelected);
            cardStateShow = 1;
            print(string.Format("{0} Ejecutó la acción: {1}", ActorName, "Curar"));
            //Me curo.
            int HealAmmount = cardSelected.Stats.GetBuff(BuffType.Heal).Ammount;
            Health += HealAmmount;
            Health = Mathf.Clamp(Health, 0, maxHealth);
            HUD.healthDisplay = Health;
            au.clip = hlth;
            au.Play();

            CombatManager.match.FeedbackHUD.SetHeal("Recuperación:", HealAmmount);
            CombatManager.match.HUDAnimations.SetTrigger("EnemyGetsHealed");

            Energy -= cardSelected.Stats.Cost;
            HUD.EnergyDisplay = Energy;
        }
        else
            StartCoroutine(DelayedEndTurn(2f));
    }

    IEnumerator WaitCard(float seconds)
    {
        cardStateShow = 0;
        yield return new WaitForSeconds(seconds);
        cardStateShow = 2;
    }

    IEnumerator WaitCardToDeck(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        cardSelected.anim.SetTrigger("BackToIdle");
        cardSelected.transform.position = fromDeckToHand.transform.position;
        cardSelected.transform.rotation = Quaternion.Euler(new Vector3(-45, 180, 0));
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
