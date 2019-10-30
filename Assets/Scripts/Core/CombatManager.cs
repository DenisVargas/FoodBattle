using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Entities;
using TMPro;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public static CombatManager match;    // Singleton de la Clase.

    public Actor Current;                 // Referencia al Actor Activo en este momento.
    public Player player;                 // Referencia al jugador actual.
    public Enem Enemy;                    // Referencia al enemigo actual.

    public Animator HUDAnimations;        // Contiene las animaciones del Canvas.
    public ActionFeedbackHUD FeedbackHUD;

    public Queue<Actor> Turns = new Queue<Actor>();

    private void Awake()
    {
        if (match == null) match = this;
        else Destroy(this);

        //Suscribimos a eventos.
        FeedbackHUD.InformExecuteActions += CanExecuteAction;

        //Player
        player = FindObjectOfType<Player>();
        player.OnStartTurn += () => 
        {
            SetNotificationState(false);
            HUDAnimations.SetTrigger("PlayerTurn");
        };
        player.OnEndTurn += EndCurrentTurn;
        player.OnActorDies += PlayerDefeat;
        Turns.Enqueue(player);

        //Enemigo.
        Enemy = FindObjectOfType<Enem>();
        Enemy.OnStartTurn += () =>
        {
            SetNotificationState(false);
            HUDAnimations.SetTrigger("EnemyTurn");
        };
        Enemy.OnEndTurn = EndCurrentTurn;
        Enemy.OnEnemyDie += PlayerWin;
        Turns.Enqueue(Enemy);

        //Fundamental que esto se setee.
        CardBehaviour.InitCardBehaviourDictionary();
        player.deck.LoadAllCards();

        Current = GetNextActor();
        Current.StartTurn();
    }

    /// <summary>
    /// Le "Avisa" al actor correspondiente que no puede realizar acciones hasta que el estado cambie a activo.
    /// </summary>
    /// <param name="active">Estado actual de la notificación.</param>
    public void SetNotificationState(bool active)
    {
        Current.CanExecuteActions = active;
    }

    /// <summary>
    /// Cuando el jugador gana, pasa algo.
    /// </summary>
    public void PlayerWin()
    {
        //Ahora mismo va a ser de golpe.
        StartCoroutine(DelayedLoadScene(3f, 1));
    }
    /// <summary>
    /// Cuando el jugador pierde, pasa algo.
    /// </summary>
    public void PlayerDefeat()
    {
        //Ahora mismo va a ser de golpe.
        StartCoroutine(DelayedLoadScene(4f, 2));
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
        print(string.Format("Turno de {0}", Current.ActorName));
    }

    /// <summary>
    /// Informa al actor actual, si puede o no ejecutar Acciones.
    /// </summary>
    /// <param name="enable">¿Puede ejecutar acciones?</param>
    public void CanExecuteAction(bool enable)
    {
        print(string.Format("Puede Ejecutar acciones {0} // CombatManager", enable));
        Current.CanExecuteActions = enable;
    }

    void Update()
    {
        Current.UpdateTurn();
    }

    IEnumerator DelayedLoadScene(float seconds, int scene)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(scene);
    }
}
