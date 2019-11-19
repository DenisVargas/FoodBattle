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

    public Vector3 FollowOffset = Vector3.zero;
    public Transform rotator;
    public Vector3 OnSelectRotation;

    #region Debugg

    [SerializeField] Renderer rend;
    [SerializeField] float speed;

    Collider cardCollider;
    Vector3 normalRotation;
    Vector3 targetlocation;

    #endregion

    [Header("Data Fundamental")]
    public CardData Stats;
    public Actor Owner;
    public Actor Rival;
    public int DeckID;

    [Header("Posicionamiento de la carta")]
    [HideInInspector] public bool Drag;
    public bool isInteractuable;
    public bool canBeShowed;

    //public bool back;
    //public bool touchScreen = false;
    //public bool stopAll = false;
    //public bool comingBack = false;
    //public bool inHand = false;

    public Vector3 starPos;
    private Vector3 mOffset;

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
        cardCollider = GetComponent<Collider>();
        normalRotation = rotator.eulerAngles;
        print(string.Format("La rotación normal es {0} ", normalRotation));

        ni = GetComponent<AudioSource>();
        discardPosition = GameObject.Find("DeckDiscard").GetComponent<Transform>();
        Owner = GetComponentInParent<Actor>();
        Rival = FindObjectOfType<Enem>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
        //inHand = false;
        //isInteractuable = false;
        //starPos = transform.position;
        //comingBack = true;
        //back = true;
    }

    /// <summary>
    /// Carga y muestra la información de las cartas en el HUD.
    /// </summary>
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
        if (!Drag && transform.position != targetlocation)
        {
            transform.position += (targetlocation - transform.position).normalized * speed * Time.deltaTime;
        }
    }


    public void ActivateCard()
    {
        if (CanBeActivated(Stats.Cost))
        {
            //Acá va todos los efectos.
            CardEffect(Owner, Rival, Stats, DeckID);
            OnUseCard(Stats.ID);
        }
        else
        {
            CombatManager.match.HUDAnimations.SetTrigger("PlayerNoENergy");
            ni.clip = noEnergy;
            ni.Play();
        }

    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public void SetTargetLocation(Vector3 targetlocation)
    {
        this.targetlocation = targetlocation;
    }

    public void FollowTarget(Vector3 targetPosition)
    {
        rend.material.color = Color.blue;
        transform.position = targetPosition + FollowOffset;
    }

    public void MouseHoverStart()
    {
        print(string.Format("EL mouse está sobre la carta {0}", Stats.CardName));
        rend.material.color = Color.red;
    }

    public void MouseHoverEnd()
    {
        rend.material.color = Color.white;
    }

    public void MouseSelect()
    {
        Drag = true;
        cardCollider.enabled = false;
        rend.material.color = Color.yellow;
        rotator.rotation = Quaternion.Euler(OnSelectRotation);
    }

    public void MouseRelease()
    {
        Drag = false;
        cardCollider.enabled = true;
        rotator.rotation = Quaternion.Euler(normalRotation);
    }
}