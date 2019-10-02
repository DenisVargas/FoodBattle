using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlineCard : MonoBehaviour
{

    public List<Transform> cards = new List<Transform>();    
    public float distance = 2;

    private void Awake()
    {
        AlingCards();
    }

    public void AlingCards()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            cards.Add(transform.GetChild(i));
        }
        foreach (var item in cards)
        {
            item.transform.localPosition = Vector3.zero;
        }
        int pivot_id = cards.Count / 2;
        Transform pivot = cards[pivot_id];

        Vector3 offset = pivot.transform.position - transform.transform.position;
        offset = offset.normalized * distance;

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.position = pivot.transform.position + offset * (i - pivot_id);
        }
        cards.Clear();

    }
}
