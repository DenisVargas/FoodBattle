using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/*
   Este script se va encargar de todo lo que LA CARTA debe hacer.
   Eso incluye, su activación.
   El player ni el deck van a tener conocimiento de algun método interno.
   Va a tener un Evento que se va a disparar cuando la carta es consumida.
   El deck va a estar suscrito a dicho evento, de esta forma se entera que la carta fue útilizada.
*/

public class Card : MonoBehaviour
{
    #region Eventos
    public event Action<int> OnUseCard = delegate { };
    public Action<Actor, Actor, CardData> CardEffect = delegate { };
    public Func<int, bool> CanBeActivated;
    //public event Action OnCardIsSeleced = delegate { }; 
    #endregion

    [Header("Data Fundamental")]
    public CardData Stats;
    public Actor Owner;
    public Actor Rival;
    public int DeckID;

    [Header("Posicionamiento de la carta")]
    public bool back;
    public bool touchScreen = false;
    public bool stopAll = false;
    public bool isInteractuable = false;
    public bool comingBack = false;
    public bool inHand = false;
    public bool canBeShowed;

    public Vector3 starPos;
    private Vector3 mOffset;

    public AudioSource ni;
    public AudioClip clickCard;

    //public GameObject attack;
    private float mZCoord;

    [Header("HUD")]
    public TextMeshProUGUI nameCard;
    public TextMeshProUGUI description;
    public TextMeshProUGUI cost;
    public TextMeshProUGUI damage;
    public Image image;

    public Transform discardPosition;

    public Animator anim;
    Rigidbody rb;
    BoxCollider col;
    private void Awake()
    {
        ni = GetComponent<AudioSource>();
        discardPosition = GameObject.Find("DeckDiscard").GetComponent<Transform>();
        Owner = GetComponentInParent<Actor>();
        Rival = FindObjectOfType<Enem>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
        inHand = false;
        //starPos = transform.position;
        comingBack = true;
        back = true;
    }

    public void LoadCardDisplayInfo()
    {
        this.name = Stats.CardName;
        nameCard.text = Stats.CardName;
        description.text = Stats.description;
        cost.text = Stats.Cost.ToString();
        damage.text = Stats.GetDebuffAmmount(DeBuffType.healthReduction).ToString();
        image.sprite = Stats.image;
    }

    private void Update()
    {
        if (Owner.ActorName == "Gordon Ramsay")
        {
            canBeShowed = true;
            if (inHand)
            {
                if (!back)
                    anim.SetBool(stopAll ? "ToTable" : "Flip", true);
                else if (comingBack)
                    anim.SetBool("Flip", false);

                if (stopAll)
                {
                    var dist = Vector3.Distance(transform.position, discardPosition.position);
                    if (dist >= 0)
                        transform.position = Vector3.Lerp(transform.position, discardPosition.position, Time.deltaTime * 3f);
                }
                if (comingBack)
                {
                    var dist = Vector3.Distance(transform.position, starPos);
                    if (dist >= 0)
                        transform.position = Vector3.Lerp(transform.position, starPos, Time.deltaTime * 6f);
                    else
                        comingBack = false;
                }
            }
        }
        
    }


    public void ActivateCard()
    {
        //Acá va todos los efectos.
        CardEffect(Owner, Rival, Stats);
        //Debug.Log("ataque");
        transform.SetParent(discardPosition);
        OnUseCard(Stats.ID);
    }

    public void OnMouseDown()
    {
        if (Owner.ActorName == "Gordon Ramsay")
        {
            if (isInteractuable)
            {
                if (!stopAll)
                {
                    starPos = transform.position;
                    comingBack = false;
                    touchScreen = false;
                    back = true;
                    mZCoord = Camera.main.WorldToScreenPoint(transform.position).z;
                    mOffset = transform.position - GetMouseAsWorldPoint();
                    ni.clip = clickCard;
                    ni.Play();
                }
            }
        }
    }
    public void OnMouseDrag()
    {
        if (Owner.ActorName == "Gordon Ramsay")
        {
            if (isInteractuable)
            {
                if (!stopAll)
                {
                    transform.position = GetMouseAsWorldPoint() + mOffset;

                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        //Esto es un juego de booleans >:D
                        bool targetHitted = hit.collider.gameObject.layer == 10;
                        back = !targetHitted;
                        touchScreen = targetHitted;
                    }
                }
            }
        }
    }
    private void OnMouseUp()
    {
        if (Owner.ActorName == "Gordon Ramsay")
        {
            if (isInteractuable)
            {
                if (!stopAll)
                {
                    if (back || !CanBeActivated(Stats.Cost))
                        comingBack = true;
                    else if (touchScreen && CanBeActivated(Stats.Cost))
                    {
                        stopAll = true;
                        ActivateCard();
                    }
                }
            }
        }
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}