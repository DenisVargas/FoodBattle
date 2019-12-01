using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public Card cardsInside;

    public void AddCard(Card cartita)
    {
        cartita.back = true;
        cartita.transform.SetParent(cartita.Owner.hand.transform);
        cardsInside = cartita;
    }
}
