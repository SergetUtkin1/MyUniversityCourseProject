using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerBoardAtributes
{
    [SerializeField] public GameObject TimerPosition;
    [SerializeField] public GameObject BlindPosition;
    [SerializeField] public GameObject[] HandPosition;
    public List<Card> _hand;
    //public List<GameObject> cardGameObjects;
    //public GameObject blindGameObject;


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
