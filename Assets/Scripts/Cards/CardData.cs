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

    public Buff GetBuff(BuffType buffType)
    {
        return Buffs.Where(b => b.BuffType == buffType).FirstOrDefault();
    }
    public DeBuff GetDebuff(DeBuffType deBuffType)
    {
        return Debuffs.Where(b => b.DebuffType == deBuffType).FirstOrDefault();
    }
    public List<Buff> GetAllBuffs()
    {
        return Buffs;
    }
    public List<DeBuff> GetAllDebuffs()
    {
        return Debuffs;
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
    public int Ammount;
    public EffectDurationType durationType;
    public int Duration;
    public EffectUseType useType;
    public int Uses;
}

public enum DeBuffType
{
    healthReduction,
    ArmourReduction,
    DamageReduction,
    nullify
}

[Serializable]
public struct DeBuff
{
    public EffectTarget effectTarget;
    public DeBuffType DebuffType;
    public int Ammount;
    public EffectDurationType durationType;
    public int Duration;
    public EffectUseType useType;
    public int Uses;
}
