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
    //public event Action OnCardIsSeleced = delegate { }; 
    #endregion

    [Header("Data Fundamental")]
    public CardData Stats;
    public Actor Owner;
    public Actor Rival;

    [Header("Posicionamiento de la carta")]
    public bool lookCard = false;
    public bool back;
    public bool touchScreen = false;
    public bool stopAll = false;
    public bool comingBack = false;

    public Vector3 starPos;
    private Vector3 mOffset;

    //public GameObject attack;
    private float mZCoord;

    [Header("HUD")]
    public TextMeshProUGUI nameCard;
    public TextMeshProUGUI description;
    public TextMeshProUGUI cost;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI life;
    public Image image;

    public Transform lookPosition;
    public Transform tablePosition;
    public Transform targetPosition;

    Animator anim;
    Rigidbody rb;
    BoxCollider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();

        //attack.SetActive(false);
        starPos = transform.position;
        back = true;
    }
    public void LoadCardDisplayInfo()
    {
        nameCard.text = Stats.nameCard;
        description.text = Stats.description;
        cost.text = Stats.cost.ToString();
        damage.text = Stats.damage.ToString();
        life.text = Stats.healAmmount.ToString();
        image.sprite = Stats.image;
    }

    private void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.layer == 9 && lookCard)
                {
                    lookCard = !lookCard;
                    stopAll = true;
                }
                else if (hit.collider.gameObject.layer == 9 && !lookCard)
                {
                    lookCard = !lookCard;
                    stopAll = false;
                    comingBack = true;
                }
            }
        }*/
        /*else if (Input.GetMouseButtonUp(0))
            lookCard = false;*/

        if (lookCard)
        {
            transform.position = Vector3.Lerp(transform.position, lookPosition.position, Time.deltaTime * 5);
        }

        if (!back && !stopAll)
        {
            anim.SetBool("Flip", true);
        }
        else if(!back && stopAll)
        {
            anim.SetBool("ToTable", true);
        }
        else if (back)
        {
            anim.SetBool("Flip", false);
        }

        if (stopAll && !lookCard)
        {
            var dist = Vector3.Distance(transform.position, tablePosition.position);
            if (dist >= 0)
                transform.position = Vector3.Lerp(transform.position, targetPosition.position, Time.deltaTime * 3f);
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

    public void ActivateCard()
    {
        Debug.Log("ataque");
        OnUseCard(Stats.ID);
        CardEffect(Owner, Rival, Stats);
        //Acá va todos los efectos.
    }

    public void OnMouseDown()
    {
        if (!stopAll)
        {
            starPos = transform.position;
            comingBack = false;
            touchScreen = false;
            back = true;
            mZCoord = Camera.main.WorldToScreenPoint(transform.position).z;
            mOffset = transform.position - GetMouseAsWorldPoint();
        }
    }
    public void OnMouseDrag()
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

                //if (hit.collider.gameObject.layer == 10)
                //{
                //    back = false;
                //    if (!touchScreen) touchScreen = true;
                //}
                //else
                //{
                //    back = true;
                //    touchScreen = false;
                //}
            }
        }
    }
    private void OnMouseUp()
    {
        if (!stopAll)
        {
            if (back)
                comingBack = true;
            else if (touchScreen)
            {
                stopAll = true;
                GetChildTable();
                ActivateCard();

                //StartCoroutine(WipAttack());
            }
        }
    }

    public void GetChildTable()
    {
        List<Transform> childs = new List<Transform>();
        for (int i = 0; i < tablePosition.childCount; i++)
        {
            childs.Add(tablePosition.GetChild(i));
        }
        foreach (var item in childs)
        {
            if (!item.GetComponent<PositionTable>().inUse)
            {
                targetPosition = item.transform;
                item.GetComponent<PositionTable>().inUse = true;
                item.GetComponent<PositionTable>().cardInUse = this;
                break;
            }
        }
    }
    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    //IEnumerator WipAttack()
    //{
    //    yield return new WaitForSeconds(1);
    //    attack.SetActive(true);
    //    yield return new WaitForSeconds(2);
    //    attack.SetActive(false);
    //}
}