using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]


public class Card
{
    public string cardDescription;
    public string cardName;
    public int cost;
    public int id;

    public Sprite sprite;

    public Card()
    {

    }

    public Card(int Id, string CardName, int Cost, string CardDescription, Sprite _Sprite)
    {
        id = Id;
        cardName = CardName;
        cost = Cost;
        cardDescription = CardDescription;
        sprite = _Sprite;
    }
}
