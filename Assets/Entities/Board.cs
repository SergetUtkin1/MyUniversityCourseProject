using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Board
{
    [SerializeField]
    public List<GameObject> cardPlaces;
    public List<Card> boadCards;
    //public List<GameObject> objectsOnBoard;

    public void AddCardToBoard(Card c)
    {
        boadCards.Add(c);
    }
}
