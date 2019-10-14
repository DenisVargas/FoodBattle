using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core.Entities;
using TMPro;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public static CombatManager match;    // Singleton de la Clase.
    public Actor Current;                 // Referencia al Actor Activo en este momento.
    public Player player;                 // Referencia al jugador actual.
    public Enem Enemy;                    // Referencia al enemigo actual.

    public Queue<Actor> Turns = new Queue<Actor>();

    private void Awake()
    {
        if (match == null) match = this;
        else Destroy(this);

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

        Enemy = FindObjectOfType<Enem>();
        Enemy.OnEndTurn = EndCurrentTurn;
        Turns.Enqueue(Enemy);

        Current = GetNextActor();
        Current.StartTurn();
    }

    /// <summary>
    /// Cuando el jugador gana, pasa algo.
    /// </summary>
    public void PlayerWin()
    {

    }
    /// <summary>
    /// Cuando el jugador pierde, pasa algo.
    /// </summary>
    public void PlayerDefeat()
    {

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
    /// Añade inmediatamente Turnos al objetivo específicado.
    /// </summary>
    /// <param name="target"> El actor que va a recibir turnos extra. </param>
    /// <param name="Ammount"> La cantidad de turnos extras que va a recibir. </param>
    public void AddExtraTurns(Actor target, int Ammount)
    {
        for (int i = 0; i < Ammount; i++)
            Turns.Enqueue(target);
    }

    /// <summary>
    /// Al terminarse el turno se elige al siguiente Actor.
    /// </summary>
    /// <param name="source"></param>
    public void EndCurrentTurn(Actor source)
    {
        if (source.extraTurns > 0)
            source.extraTurns--;
        else
            Turns.Enqueue(source);

        Current = GetNextActor();
        Current.StartTurn();
    }

    void Update()
    {
        Current.UpdateTurn();
    }
}
