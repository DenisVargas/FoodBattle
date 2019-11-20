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
    public Action<Actor> RoundTurn = delegate { };

    public GameObject DeckSelectionPanel;

    Actor Current;                 // Referencia al Actor Activo en este momento.
    Player player;                 // Referencia al jugador actual.
    Enem Enemy;                    // Referencia al enemigo actual.

    public Animator HUDAnimations;        // Contiene las animaciones del Canvas.
    public ActionFeedbackHUD FeedbackHUD;

    private bool inGame = false;

    [Header("Rondas")]
    public bool[] _currentRound = new bool[2] { false, false };
    public int InitialEnergy = 3;
    public int EnergyIncreasePerRound = 1;

    public Queue<Actor> Turns = new Queue<Actor>();

    private void Awake()
    {
        if (match == null) match = this;
        else Destroy(this);

        //Suscribimos a eventos.
        FeedbackHUD.InformExecuteActions += CanExecuteAction;

        var test = Resources.FindObjectsOfTypeAll<Actor>();
        foreach (var item in test)
        {
            if (item.GetComponent<Player>())
                player = item.GetComponent<Player>();
            else
                Enemy = item.GetComponent<Enem>();
        }
    }

    public void SetDecksAndStartMatch(List<DeckData> aviableDecks, int playerDeckIndex)
    {
        var provitionalDeckList = new List<DeckData>(aviableDecks);

        player.deck.deckSelected = provitionalDeckList[playerDeckIndex];
        provitionalDeckList.RemoveAt(playerDeckIndex);
        //Enemy.deck.deckSelected = provitionalDeckList[0]; //Esto esta tirando errores x todos lados.
        StartGame();
    }

    public void StartGame()
    {
        SetTurnPlayer();
        SetTurnEnemy();
        Current = GetNextActor();
        Current.StartTurn(InitialEnergy);
        inGame = true;
    }

    public void SetTurnPlayer()
    {
        player.OnStartTurn += (Player) =>
        {
            SetNotificationState(false);
            ActualizeMatchRoundData(Player);
            HUDAnimations.SetTrigger("PlayerTurn");
        };
        player.OnEndTurn += EndCurrentTurn;
        player.OnActorDies += PlayerDefeat;
        player.deck.LoadAllCards();
        player.deck.ShuffleDeck();
        Turns.Enqueue(player);
    }
    public void SetTurnEnemy()
    {
        Enemy.OnStartTurn += (Enemy) =>
        {
            ActualizeMatchRoundData(Enemy);
            SetNotificationState(false);
            HUDAnimations.SetTrigger("EnemyTurn");
        };
        Enemy.OnEndTurn = EndCurrentTurn;
        Enemy.OnEnemyDie += PlayerWin;
        Turns.Enqueue(Enemy);

    }

    /// <summary>
    /// Retorna verdadero cuando ambos players tuvieron su turno.
    /// </summary>
    /// <returns></returns>
    public bool roundEnded()
    {
        return (_currentRound[0] && _currentRound[1]);
    }
    public void ActualizeMatchRoundData(Actor actor)
    {
        bool isPlayer = actor == player;
        if (isPlayer)
            _currentRound[0] = isPlayer;
        else
            _currentRound[1] = !isPlayer;
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
        StartCoroutine(DelayedLoadScene(3f, 3));
    }
    /// <summary>
    /// Cuando el jugador pierde, pasa algo.
    /// </summary>
    public void PlayerDefeat()
    {
        //Ahora mismo va a ser de golpe.
        StartCoroutine(DelayedLoadScene(4f, 4));
    }

    /// <summary>
    /// Devuelve el siguiente Actor al que le toca jugar.
    /// </summary>
    /// <returns></returns>
    public Actor GetNextActor()
    {
        return Turns.Dequeue();
    }
    ///// <summary>
    ///// Añade inmediatamente Turnos al objetivo específicado.
    ///// </summary>
    ///// <param name="target"> El actor que va a recibir turnos extra. </param>
    ///// <param name="Ammount"> La cantidad de turnos extras que va a recibir. </param>
    //public void AddExtraTurns(Actor target, int Ammount)
    //{
    //    for (int i = 0; i < Ammount; i++)
    //        Turns.Enqueue(target);
    //}

    /// <summary>
    /// Al terminarse el turno se elige al siguiente Actor.
    /// </summary>
    /// <param name="source"></param>
    public void EndCurrentTurn(Actor source)
    {
        //if (source.extraTurns > 0)
        //    source.extraTurns--;
        //else
        Turns.Enqueue(source);

        if (roundEnded())
        {
            _currentRound[0] = false;
            _currentRound[1] = false;
            InitialEnergy += EnergyIncreasePerRound;
        }

        Current = GetNextActor();
        Current.StartTurn(InitialEnergy);
        print(string.Format("Turno de {0}", Current.ActorName));
    }

    /// <summary>
    /// Informa al actor actual, si puede o no ejecutar Acciones.
    /// </summary>
    /// <param name="enable">¿Puede ejecutar acciones?</param>
    public void CanExecuteAction(bool enable)
    {
        //print(string.Format("Puede Ejecutar acciones {0} // CombatManager", enable));
        Current.CanExecuteActions = enable;
    }

    void Update()
    {
        if (inGame)
            Current.UpdateTurn();
    }

    IEnumerator DelayedLoadScene(float seconds, int scene)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(scene);
    }
}
