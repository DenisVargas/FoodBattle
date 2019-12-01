﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotsGeneral : MonoBehaviour
{
    public List<Slot> slots = new List<Slot>();
    public GameObject cardPref;

    void Awake()
    {
        var slotsInChild = GetComponentsInChildren<Slot>();
        foreach (var item in slotsInChild)
            slots.Add(item);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            DetectCombination();
        }
    }

    public void DetectCombination()
    {
        if (slots[0].cardsInside.Stats.canFusion && slots[1].cardsInside.Stats.canFusion)
        {
            var carta1 = slots[0].cardsInside;
            var carta2 = slots[1].cardsInside;
            if (carta1.Stats.IDFusion == carta2.Stats.ID && carta2.Stats.IDFusion == carta1.Stats.ID)
            {
                Card fusioned = Instantiate(cardPref, slots[2].transform.position, Quaternion.Euler(new Vector3(-90, 0, 0))).GetComponent<Card>();
                fusioned.Stats = CardDatabase.GetCardData(carta1.Stats.IDCardFusioned);
                fusioned.CardEffect = CardDatabase.GetCardBehaviour(fusioned.Stats.ID);
                fusioned.Owner = FindObjectOfType<Player>();
                fusioned.Rival = FindObjectOfType<Enem>();
                fusioned.DeckID = carta1.Stats.IDCardFusioned;
                fusioned.LoadCardDisplayInfo();
                StartCoroutine(SendCard(fusioned));
            }
        }
    }

    IEnumerator SendCard(Card toSend)
    {
        yield return new WaitForSeconds(2f);
        toSend.CardEffect(toSend.Owner, toSend.Rival, toSend.Stats, toSend.DeckID);
    }
}
