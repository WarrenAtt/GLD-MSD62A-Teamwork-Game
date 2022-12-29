using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="GameItem",menuName ="Game Items/Create Game item")]
public class ItemScriptableObject : ScriptableObject
{
    public string title;
    public Sprite icon;
    public int increaseValue;
    public int cost;
    public Type type; //stores either health or mana (not both at the same time)


    public enum Type
    {
        Health,
        Shield,
        Speed
    }
}
