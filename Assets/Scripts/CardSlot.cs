using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer)), RequireComponent(typeof(Collider))]
public class CardSlot : MonoBehaviour
{
    Renderer rend;
    Collider coll;
    [SerializeField] Card[] AssignedCards = new Card[3];
    [SerializeField] string SlotName = "Slot numero 1";

    public bool isLocked = false;

    // Start is called before the first frame update
    void Awake()
    {
        rend = GetComponent<Renderer>();
        coll = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void AssignCard(Card card)
    {
        Debug.LogWarning(string.Format("Carta {0} asignada al Slot {1}", card.Stats.CardName, SlotName));
        for (int i = 0; i < AssignedCards.Length; i++)
        {
            if (AssignedCards[i] == null)
            {
                AssignedCards[i] = card;
                break;
            }

            if (i == AssignedCards.Length - 1 && AssignedCards[i] != null)
                isLocked = true;
        }
    }

    public void MouseHoverStart()
    {
        print("Slot está siendo hovereado");
        //rend.material.color = Color.blue;
        rend.material.SetFloat("_ASEOutlineWidth", 0.12f); //Works
    }

    public void MouseHoverEnd()
    {
        print("Slot ya no esta siendo Hovereado");
        rend.material.SetFloat("_ASEOutlineWidth", 0f);
    }


    public void MouseSelect()
    {
        print("Slot Mouse Select");
    }

    public void MouseRelease()
    {
        print("Slot Mouse De-Select");
    }

}
