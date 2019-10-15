using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="Nueva Carta", menuName ="Crear Carta")]
public class CardData : ScriptableObject
{
    public string nameCard;
    public int ID;
    public int cost;
    public int damage;

    public BuffType BuffType;
    public int healAmmount;
    public Sprite image;
    [TextArea]
    public string description;
}
