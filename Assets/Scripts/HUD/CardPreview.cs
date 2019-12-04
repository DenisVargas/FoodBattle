using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPreview : MonoBehaviour
{
    private Card cardSelected;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI description;
    public TextMeshProUGUI cost;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI fusionName;
    public Image imageCard;
    private Animator anim;
    public GameObject isFusionable;
    public bool isSelected;
    public LayerMask detect;
    //public AudioSource s;
    //public AudioClip coll;      quise poner algun sonido cuando el mouse apoya a la carta 
    //                                  y que sepa que aparece una carta al lado de la pantalla

    private void Start()
    {
        anim = GetComponent<Animator>();
        isSelected = false;
        //s = GetComponent<AudioSource>();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, detect))
        {
            if (hit.transform.GetComponentInChildren<Card>())
                cardSelected = hit.transform.GetComponentInChildren<Card>();
            else if (hit.transform.GetComponent<Card>())
                cardSelected = hit.transform.GetComponent<Card>();

            if (cardSelected != null && cardSelected.canBeShowed)
            {
                isSelected = true;
                cardName.text = cardSelected.nameCard.text;
                description.text = cardSelected.description.text;
                cost.text = cardSelected.cost.text;
                damage.text = cardSelected.damage.text;
                imageCard.sprite = cardSelected.image.sprite;
                if (cardSelected.Stats.canFusion)
                {
                    isFusionable.SetActive(true);
                    var cardFusioned = CardDatabase.GetCardData(cardSelected.Stats.IDFusion);
                    fusionName.text = cardFusioned.CardName;
                }
                else
                    isFusionable.SetActive(false);
            }
        }
        else
        {
            cardSelected = null;
            isSelected = false;
        }
        if (cardSelected != null)
            anim.SetBool("Selected", isSelected);
        else
            anim.SetBool("Selected", isSelected);
    }
}
