using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public Card cardInside;

    public void AddCard(Card cartita)
    {
        if (cardInside != null)
        {
            cardInside.slotSelected = null;
            cardInside.transform.SetParent(cardInside.Owner.hand.transform);
            cardInside.Owner.hand.hand.Add(cardInside.DeckID, cardInside);
            cardInside.comingBack = true;
        }
        cardInside = cartita;
        cardInside.transform.Rotate(new Vector3(0, 0, 0));
        StartCoroutine(Aline());
    }

    IEnumerator Aline()
    {
        yield return new WaitForSeconds(2f);
        GetComponentInParent<SlotsGeneral>().DetectCombination();
        cardInside.Owner.hand.AlingCards();
    }
}
