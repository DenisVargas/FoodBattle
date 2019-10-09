using System;
using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    //Eventos Comunes.
    public Action<Actor> OnStartTurn = delegate { };
    public Action OnUpdateTurn = delegate { };
    public Action<Actor> OnEndTurn = delegate { };

    public virtual void StartTurn() { }
    public virtual void UpdateTurn() { }
    public virtual void EndTurn() { }
}
