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
    public Image imageCard;
    private Animator anim;
    public bool isSelected;
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
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9) && hit.transform.GetComponent<Card>().canBeShowed)
        {
            cardSelected = hit.transform.GetComponent<Card>();
            isSelected = true;
            cardName.text = cardSelected.nameCard.text;
            description.text = cardSelected.description.text;
            cost.text = cardSelected.cost.text;
            damage.text = cardSelected.damage.text;
            imageCard.sprite = cardSelected.image.sprite;
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
