using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="Nueva Carta", menuName ="Crear Carta")]
public class CreateCard : ScriptableObject
{
    public int damage;
    public int life;
    public int cost;
    public Sprite image;
    public string nameCard;
    public string description;
}
