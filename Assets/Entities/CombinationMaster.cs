using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombinationMaster
{
    private static readonly string[] JrSraightValues = { 
        "A", 
        "2", 
        "3", 
        "4", 
        "5" 
    };
    private static readonly string[] RoyalValues = { 
        "10", 
        "J", 
        "Q", 
        "K",
        "A"
    };
    private static readonly string[] Names = { 
        "Flush Royal",
        "Flush Straight",
        "Four Of Kind", 
        "Full House",
        "Flush",
        "Straight",
        "Three Of Kind",
        "Two Pair",
        "Pair",
        "HighCard" 
    };

    public static Combination FindBestCombination(List<Card> cards)
    {
        var res = new Combination();
        var Methods = new List<Func<List<Card>, List<Card>>>()
        {
            FindFlushRoyal,
            FindFlushStraight,
            FindFourOfKind,
            FindFullHouse,
            FindFlush,
            FindStraight,
            FindThreeOfKind,
            FindTwoPair,
            FindPair,
            FindHighCard,
        };

        for (int i = 0; i < Methods.Count; i++)
        {
            var cardsOfCombintations = Methods[i].Invoke(cards);

            if (cardsOfCombintations.Count != 0)
            {
                res.cards = cardsOfCombintations;
                res.Name = Names[i];
                res.Rank = i;
                break;
            }
        }

        return res;
    }

    public static List<Card> FindHighCard(List<Card> cards)
    {
        Card CurHigh = cards[0];
        foreach (var card in cards)
        {
            if (CurHigh < card)
                CurHigh = card;
        }
        return new List<Card>() { CurHigh };
    }

    public static List<Card> FindPair(List<Card> cards)
    {
        var combinations = new List<Card>();
        cards.OrderBy(x => x.Value);
        combinations.AddRange(cards.FindAll(c => cards.Count(x => x == c) == 2));

        return combinations;
    }

    public static List<Card> FindTwoPair(List<Card> cards)
    {
        var combinations = new List<Card>();
        var cardsCopy = new List<Card>(cards);
        cardsCopy.OrderBy(x => x.Value);
        for (int i = 0; i < 2; i++)
        {
            var pair = FindPair(cardsCopy);
            combinations.AddRange(pair);
            cardsCopy.RemoveAll(x => pair.Contains(x));
        }

        return combinations.Count == 4 ? combinations : new List<Card>();
    }

    public static List<Card> FindThreeOfKind(List<Card> cards)
    {
        var combinations = new List<Card>();
        cards.OrderBy(x => x.Value);

        combinations.AddRange(cards.FindAll(c => cards.Count(x => x == c) == 3));

        return combinations;
    }

    public static List<Card> FindStraight(List<Card> cards)
    {
        var combinations = new List<Card>();
        combinations.Add(cards[0]);
        cards.OrderBy(x => x.Value);
        for (int i = 1; i < cards.Count; i++)
        {
            switch (cards[i - 1] - cards[i])
            {
                case 0:
                    break;
                case -1:
                    combinations.Add(cards[i]);
                    break;

                default:
                    if (combinations.Count >= 5)
                    {
                        return combinations;
                    }
                    combinations.Clear();
                    combinations.Add(cards[i]);
                    break;
            }
        }

        if (combinations.Count < 5)
        {
            combinations.Clear();
            for (int i = 0; i < 5; i++)
            {
                if (cards.Any(x => x.Value == JrSraightValues[i]))
                {
                    combinations.Add(cards.Find(x => x.Value == JrSraightValues[i]));
                }
            }
        }

        return combinations.Count < 5 ? new List<Card>() : combinations;
    }

    public static List<Card> FindFlush(List<Card> cards)
    {
        var combinations = new List<Card>();
        cards.OrderBy(x => x.Value);
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            combinations = cards.FindAll(c => c.Suit == cards[i].Suit);
            if (combinations.Count >= 5)
            {
                break;
            }
            else
            {
                combinations.Clear();
            }
        }

        return combinations;
    }

    public static List<Card> FindFullHouse(List<Card> cards)
    {
        var combinations = new List<Card>();
        var cardCopy = new List<Card>(cards);
        cardCopy.OrderBy(x => x.Value);
        var threeCards = FindThreeOfKind(cardCopy);
        combinations.AddRange(threeCards);
        cardCopy.RemoveAll(x => threeCards.Contains(x));
        combinations.AddRange(cardCopy.FindAll(c => cardCopy.Count(x => x == c) >= 2));

        return combinations.Count != 5 ? new List<Card>() : combinations;
    }

    public static List<Card> FindFourOfKind(List<Card> cards)
    {
        var combinations = new List<Card>();

        combinations.AddRange(cards.FindAll(c => cards.Count(x => x == c) == 4));

        return combinations;
    }

    public static List<Card> FindFlushStraight(List<Card> cards)
    {
        var combination = new List<Card>();
        var FlushCards = FindFlush(cards);

        if (FlushCards.Count > 0)
        {
            combination = FindStraight(FlushCards);
        }
        else
        {
            combination.Clear();
        }
        return combination;
    }

    public static List<Card> FindFlushRoyal(List<Card> cards)
    {
        var combinations = new List<Card>();
        var copyCards = FindFlush(cards);
        combinations.AddRange(copyCards.FindAll(x => RoyalValues.Contains(x.Value)));
        return combinations.Count == 5 ? combinations : new List<Card>();
    }

}
