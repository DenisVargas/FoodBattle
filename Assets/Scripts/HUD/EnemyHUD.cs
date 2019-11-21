using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyHUD : MonoBehaviour
{
    [SerializeField] TMP_Text _name = null;
    [SerializeField] TMP_Text _health = null;
    [SerializeField] TMP_Text _energy = null;

    public GameObject EnenmyHudObject;

    private void Awake()
    {
        EnenmyHudObject.SetActive(false);
    }

    /// <summary>
    /// Setea el nombre del rival en el HUD.
    /// </summary>
    /// <param name="name">Nombre del rival.</param>
    public void SetRivalName(string name)
    {
        _name.text = name;
    }
    /// <summary>
    /// Actualiza en el HUD la cantidad de vida del rival.
    /// </summary>
    public float healthDisplay
    {
        set { _health.text = value.ToString(); }
    }
    /// <summary>
    /// Actualiza en el HUD la cantidad de cartas del rival.
    /// </summary>
    public float EnergyDisplay
    {
        set { _energy.text = value.ToString(); }
    }

}
