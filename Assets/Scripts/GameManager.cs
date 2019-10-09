using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core.Entities;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static Actor Current;
    public static Player player;
    public static Actor Enemy;

    public Queue<Actor> Turns = new Queue<Actor>();

    private void Awake()
    {
        //Player
        player = FindObjectOfType<Player>();
        player.OnEndTurn += EndCurrentTurn;
        Turns.Enqueue(player);

        //Enemies = FindObjectsOfType<Actor>()
        //          .Where(x => x.GetComponent<Enem>() != null)
        //          .ToArray();

        //foreach (var Enemy in Enemies)
        //{
        //    Turns.Enqueue(Enemy);
        //    Enemy.OnEndTurn += EndCurrentTurn;
        //}

        Enemy = FindObjectOfType<Actor>();
        Enemy.OnEndTurn = EndCurrentTurn;
        Turns.Enqueue(Enemy);

        Current = GetNextActor();
        Current.StartTurn();
    }

    /// <summary>
    /// Devuelve el siguiente Actor al que le toca jugar.
    /// </summary>
    /// <returns></returns>
    public Actor GetNextActor()
    {
        return Turns.Dequeue();
    }

    /// <summary>
    /// Al terminarse el turno se elige al siguiente Actor.
    /// </summary>
    /// <param name="source"></param>
    public void EndCurrentTurn(Actor source)
    {
        Turns.Enqueue(source);
        Current = GetNextActor();
        Current.StartTurn();
    }

    void Update()
    {
        Current.UpdateTurn();
    }
}
