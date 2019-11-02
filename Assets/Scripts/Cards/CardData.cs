using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="Nueva Carta", menuName ="Crear Carta")]
public class CardData : ScriptableObject
{
    public string CardName;
    public int ID;
    public Sprite image;
    public int Cost;

    [Header("GameFlow")]
    public int extraTurns;
    public int extraCards;

    [Header("Buffs & Effects")]
    public List<Buff> Buffs;
    public List<DeBuff> Debuffs;
    [TextArea]
    public string description;

    public int GetBuffAmmount(BuffType buffType)
    {
        var buff = Buffs.Where( b => b.BuffType == buffType).FirstOrDefault();
        return buff.BuffAmmount;
    }
    public int GetDebuffAmmount(DeBuffType deBuffType)
    {
        var debuff = Debuffs.Where(db => db.DebuffType == deBuffType).FirstOrDefault();
        return debuff.DebuffAmmount;
    }
}
public enum EffectTarget
{
    owner,
    target
}
public enum EffectDurationType
{
    Inmediate,
    Permanent,
    Limited
}
public enum EffectUseType
{
    Infinit,
    Limited
}
public enum BuffType
{
    none,
    Heal,
    FullHealthRestore,
    ArmourIncrease,
    DamageIncrease,
    CardCostDecrease,
    NullyfyCardCost,
    Invulnerability
}

[Serializable]
public struct Buff
{
    public EffectTarget effectTarget;
    public BuffType BuffType;
    public int BuffAmmount;
    public EffectDurationType durationType;
    public int Duration;
    public EffectUseType useType;
    public int Uses;
}

public enum DeBuffType
{
    healthReduction,
    ArmourReduction,
    nullify
}

[Serializable]
public struct DeBuff
{
    public EffectTarget effectTarget;
    public DeBuffType DebuffType;
    public int DebuffAmmount;
    public EffectDurationType durationType;
    public int Duration;
    public EffectUseType useType;
    public int Uses;
}
