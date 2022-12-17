using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour, IComparable<Card>
{
    public string Value { get; set; }
    public string Suit { get; set; }
    public Figure CardFigure { get; set; }
    public int Rank { get { return Deck.GetIndexOfCard(this.Value); } }

    public static Card CreateCard(GameObject where, string value, string suit, Figure cardFigure)
    {
        Card card = where.AddComponent<Card>();
        card.Value = value;
        card.Suit = suit;
        card.CardFigure = cardFigure;
        return card;
    }

    public int CompareTo(Card obj)
    {
        return Rank - obj.Rank;
    }

    public static int operator -(Card card1, Card card2)
    {
        return card1.Rank - card2.Rank;
    }

    public static bool operator >(Card card1, Card card2)
    {
        return card1.Rank > card2.Rank;
    }

    public static bool operator <(Card card1, Card card2)
    {
        return card1.Rank < card2.Rank;
    }

    public static bool operator ==(Card card1, Card card2)
    {
        return card1.Rank == card2.Rank;
    }

    public static bool operator !=(Card card1, Card card2)
    {
        return card1.Rank != card2.Rank;
    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
