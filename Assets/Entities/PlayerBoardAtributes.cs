using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerBoardAtributes
{
    [SerializeField] public GameObject BlindPosition;
    [SerializeField] public GameObject[] HandPosition;
    public List<Card> _hand;
    public List<GameObject> handObject;


    public Blind Blind { get; set; } 
    public List<Card> Hand {
        get
        {
            return _hand;
        }
        set 
        {
            _hand = value;
        }
    }

    public void AddCardToHand(Card card)
    => _hand.Add(card);

}