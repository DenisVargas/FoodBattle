using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotsGeneral : MonoBehaviour
{
    public List<Slot> slots = new List<Slot>();
    public GameObject cardPref;
    public GameObject effect;
    public bool startEffect = false;
    public Vector3 startPos;
    public AudioSource asd;
    public AudioClip fusion;

    void Awake()
    {
        asd = GetComponent<AudioSource>();
        startPos = effect.transform.position;
        var slotsInChild = GetComponentsInChildren<Slot>();
        foreach (var item in slotsInChild)
            slots.Add(item);

    }
    private void Update()
    {
        if (startEffect)
        {
            if (Vector3.Distance(effect.transform.position, new Vector3(effect.transform.position.x, -7f, effect.transform.position.z)) >= 0.01f)
            {
                effect.transform.position = Vector3.Lerp(effect.transform.position, new Vector3(effect.transform.position.x, -7f, effect.transform.position.z),Time.deltaTime);
            }
        }
        else
        {
            if (Vector3.Distance(effect.transform.position, startPos) >= 0.01f)
                effect.transform.position = Vector3.Lerp(effect.transform.position, startPos, Time.deltaTime * 1.5f);
        }
    }

    public void DetectCombination()
    {
        if (slots[0].cardInside != null && slots[1].cardInside != null)
        {
            if (slots[0].cardInside.Stats.canFusion && slots[1].cardInside.Stats.canFusion)
            {
                var carta1 = slots[0].cardInside;
                var carta2 = slots[1].cardInside;
                if (carta1.Stats.IDFusion == carta2.Stats.ID && carta2.Stats.IDFusion == carta1.Stats.ID)
                {
                    Card fusioned = Instantiate(cardPref, slots[2].transform.position, Quaternion.Euler(new Vector3(-45, 0, 0))).GetComponent<Card>();
                    fusioned.Stats = CardDatabase.GetCardData(carta1.Stats.IDCardFusioned);
                    fusioned.CardEffect = CardDatabase.GetCardBehaviour(fusioned.Stats.ID);
                    fusioned.Owner = FindObjectOfType<Player>();
                    fusioned.Rival = FindObjectOfType<Enem>();
                    fusioned.DeckID = carta1.Stats.IDCardFusioned;
                    fusioned.LoadCardDisplayInfo();
                    StartCoroutine(SendCard(fusioned));
                    asd.clip = fusion;
                    asd.Play();
                }
            }
        }
    }

    IEnumerator SendCard(Card toSend)
    {
        startEffect = true;
        yield return new WaitForSeconds(2f);
        toSend.CardEffect(toSend.Owner, toSend.Rival, toSend.Stats, toSend.DeckID);
        toSend.shaderStart = true;
        toSend.canvas.SetActive(false);
        startEffect = false;
        foreach (var i in toSend.objetos)
            i.SetActive(false);
        Destroy(toSend.gameObject, 2f);
        slots[0].cardInside.Owner.hand.hand.Add(slots[0].cardInside.DeckID, slots[0].cardInside);
        slots[1].cardInside.Owner.hand.hand.Add(slots[1].cardInside.DeckID, slots[1].cardInside);

        slots[0].cardInside.Owner.hand.DiscardCard(slots[0].cardInside.DeckID);
        slots[1].cardInside.Owner.hand.DiscardCard(slots[1].cardInside.DeckID);
        slots[0].cardInside = null;
        slots[1].cardInside = null;
    }
}
