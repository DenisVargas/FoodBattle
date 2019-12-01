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
    public Action<Actor, Actor, CardData, int> CardEffect = delegate { };
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
    public bool isInteractuable;
    public bool comingBack = false;
    public bool inHand = false;
    public bool canBeShowed;
    public bool targetHitted = false;
    public bool toSlot = false;
    public LayerMask posLayer;

    public Vector3 starPos;
    private Vector3 mOffset;

    [Header("Shaders")]
    public Renderer mats;
    public float shaderLerp;
    public bool shaderStart = false;
    public GameObject canvas;
    public GameObject[] objetos;

    public AudioSource ni;
    public AudioClip clickCard;
    public AudioClip noEnergy;

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
        //isInteractuable = false;
        //starPos = transform.position;
        comingBack = true;
        back = true;
        shaderLerp = 100;
    }

    public void LoadCardDisplayInfo()
    {
        this.name = Stats.CardName;
        nameCard.text = Stats.CardName;
        description.text = Stats.description;
        cost.text = Stats.Cost.ToString();
        damage.text = Stats.GetDebuff(DeBuffType.healthReduction).Ammount.ToString();
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
                
                if (comingBack)
                    anim.SetBool("Flip", false);

                if (stopAll)
                {
                    var dist = Vector3.Distance(transform.position, discardPosition.position);
                    if (dist >= 0)
                        transform.position = Vector3.Lerp(transform.position, discardPosition.position, Time.deltaTime * 3f);

                    else
                        inHand = false;
                }
                if (comingBack)
                {
                    var dist = Vector3.Distance(transform.position, starPos);
                    if (dist >= 0)
                        transform.position = Vector3.Lerp(transform.position, starPos, Time.deltaTime * 6f);
                    else
                        comingBack = false;
                }
                if (shaderStart)
                {
                    shaderLerp -= 2f;
                    ShadersOP(shaderLerp);
                    if (shaderLerp <= 0)
                    {
                        comingBack = false;
                        stopAll = true;
                        transform.SetParent(discardPosition.transform);
                        Owner.hand.AlingCards();
                        shaderStart = false;
                    }
                }
            }
        }
    }

    public void ShadersOP(float sec)
    {
        foreach (var item in mats.materials)
        {
            item.SetFloat("_Progress", sec / 100);
        }
    }

    public void GoToSlot()
    {

    }

    public void ActivateCard()
    {
        if (CanBeActivated(Stats.Cost))
        {
            CardEffect(Owner, Rival, Stats, DeckID);
            //Acá va todos los efectos.
            //shaderStart = true;
            OnUseCard(Stats.ID);
        }
        else
        {
            touchScreen = false;
            back = false;
            comingBack = true;
            CombatManager.match.HUDAnimations.SetTrigger("PlayerNoENergy");
            ni.clip = noEnergy;
            ni.Play();
        }

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
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, posLayer))
                    {
                        //Esto es un juego de booleans >:D
                        if (hit.collider.gameObject.layer == 10)
                        {
                            toSlot = false;
                            targetHitted = true;
                            back = !targetHitted;
                            touchScreen = targetHitted;
                        }
                        else if (hit.collider.gameObject.layer == 11)
                        {
                            toSlot = true;
                            Debug.Log("asdsadas");
                        }
                        else
                        {
                            targetHitted = false;
                            back = !targetHitted;
                            touchScreen = targetHitted;
                            toSlot = false;
                        }
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
                    if (back)
                        comingBack = true;
                    else if (touchScreen && toSlot)
                        GoToSlot();
                    else if (touchScreen && !toSlot)
                        ActivateCard();

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