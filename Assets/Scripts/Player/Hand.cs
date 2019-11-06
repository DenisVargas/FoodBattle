using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Es la "Mano del jugador"
public class Hand : MonoBehaviour
{
    public List<Transform> cards = new List<Transform>();
    public Dictionary<int, Card> hand = new Dictionary<int, Card>();

    public Transform node1;
    public Transform node2;
    private Vector3 startPost;
    public int maxCardsInHand;

    public void HandControl(bool activate)
    {
        cards.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                cards.Add(transform.GetChild(i));
            }
        }
        foreach (var item in cards)
        {
            item.GetComponent<Card>().isInteractuable = activate;
        }
    }

    public void GetDrawedCards(Deck deck, int Ammount)
    {
        if (hand.Count < 4)
        {
            foreach (var item in deck.DrawCards(Ammount))
            {
                item.transform.SetParent(transform);
                item.inHand = true;
                hand.Add(item.DeckID, item);
            }
            AlingCards();
        }
    }

    public void DiscardCard(int idCard)
    {
        foreach (var item in hand)
        {
            if (item.Key == idCard)
            {
                var carta = item.Value;
                carta.comingBack = false;
                carta.stopAll = true;
                carta.transform.SetParent(carta.discardPosition);
                hand.Remove(idCard);
                break;
            }
        }
    }

    public void AlingCards()
    {
        cards.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                cards.Add(transform.GetChild(i));
            }
        }
        foreach (var item in cards)
        {
            item.transform.position = Vector3.zero;
        }
        var leftPoint = node1.position;
        var rightPoint = node2.position;

        var delta = (leftPoint - rightPoint).magnitude;
        var howMany = cards.Count;
        var howManyGapsBetweenCards = howMany - 1;
        var theHighestIndex = howMany;
        var gapFromOneItemToTheNextOne = delta / howManyGapsBetweenCards;
        for (int i = 0; i < theHighestIndex; i++)
        {
            cards[i].transform.position = leftPoint;
            cards[i].transform.position += new Vector3((-i * gapFromOneItemToTheNextOne), 0, 0);
            cards[i].GetComponent<Card>().starPos = cards[i].transform.position;
            cards[i].GetComponent<Card>().inHand = true;
        }
    }
}