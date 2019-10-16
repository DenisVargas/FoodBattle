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
    public Image imageCard;
    private Animator anim;
    public bool isSelected;

    private void Start()
    {
        anim = GetComponent<Animator>();
        isSelected = false;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9))
        {
            cardSelected = hit.transform.GetComponent<Card>();
            isSelected = true;
            cardName.text = cardSelected.nameCard.text;
            description.text = cardSelected.description.text;
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
