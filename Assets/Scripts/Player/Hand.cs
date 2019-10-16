using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Es la "Mano del jugador"
public class Hand : MonoBehaviour
{
    
    public List<Transform> cards = new List<Transform>();
    public Transform node1;
    public Transform node2;
    private Vector3 startPost;
    //public cameraShaker shake;              <- Llamo al script cameraShaker
    void Awake()
    {
        transform.Rotate(new Vector3(-45, 0, 0));
        AlingCards();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AlingCards();
        }
        /*if (Input.GetKeyDown(KeyCode.U)){
        StartCoroutine(shake.Shake(.15f, .4f));          <---- "supuestamente tiembla la camara pero no pude con el error que me pausa pero deberia funcionar
        }*/
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
            item.transform.localPosition = Vector3.zero;
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
            cards[i].transform.position += new Vector3((-i * gapFromOneItemToTheNextOne), 0,0);
        }

    }
}
