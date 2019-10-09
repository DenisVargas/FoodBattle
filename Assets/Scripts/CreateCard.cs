using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="Nueva Carta", menuName ="Crear Carta")]
public class CreateCard : ScriptableObject
{
    public string nameCard;
    public string description;
    public int cost;
    public int damage;
    public int life;
    public Sprite image;
}
