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
        "Flush Royal",//
        "Flush Straight",
        "Four Of Kind", // h
        "Full House",// h of 3 /q
        "Flush",//last/q
        "Straight",// last/q
        "Three Of Kind", // h/q
        "Two Pair",// h/q
        "Pair",// h/q
        "HighCard" 
    };

    public static List<Player> FindWinners(List<Player> players, List<Card> boardCards)
    {
        if(players.Count > 1)
        {
            var winners = new List<Player>();
            switch (players.First().combination.Name)
            {
                case "Full House":
                    winners = FindWinnersFullHouse(players);
                    break;

                case "Pair":
                    winners = FindWinnersPair(players);
                    break;

                default:
                    winners = FindWinnersByHighCard(players);

                    if (winners.Count > 1)
                    {
                        winners = FindWinnersByKicker(players, boardCards);
                    }
                    break;
            }

            return winners;
        }
        else
        {
            return players;
        }    
    }

    // TO UTILS
    public static List<Card> ConCatBoardHand(List<Card> boardCards, Player player)
        => (new List<Card>(boardCards).Concat(player.atributes.Hand)).ToList();

    public static List<Player> FindWinnersByKicker(List<Player> players, List<Card> boardCards)
    {
        var highCard = FindHighCard(ConCatBoardHand(boardCards, players.First())
            .FindAll(x => !players.First().combination.cards
            .Any(c => c==x)));
        var winners = new List<Player>() { players.First() };
        for (int i = 1; i < players.Count; i++)
        {
            var hcChallenger = FindHighCard(ConCatBoardHand(boardCards, players.First())
                .FindAll(x => !players[i].combination.cards
                .Any(c => c == x)));
            if (hcChallenger.First() == highCard.First())
            {
                winners.Add(players[i]);
            }
            else if (hcChallenger.First() > highCard.First())
            {
                highCard = hcChallenger;
                winners.Clear();
                winners.Add(players[i]);
            }
        }
        return winners;
    }

    public static List<Player> FindWinnersByHighCard(List<Player> players)
    {
        var highCard = FindHighCard(players.First().combination.cards);
        var winners = new List<Player>() { players.First() };
        for (int i = 1; i < players.Count; i++)
        {
            var hcChallenger = FindHighCard(players[i].combination.cards);
            if (hcChallenger.First() == highCard.First())
            {
                winners.Add(players[i]);
            }
            else if (hcChallenger.First() > highCard.First())
            {
                highCard = hcChallenger;
                winners.Clear();
                winners.Add(players[i]);
            }
        }
        return winners;
    }

    public static List<Player> FindWinnersFullHouse(List<Player> players)
    {
        var highCard = FindHighCard(FindThreeOfKind(players.First().combination.cards));
        var winners = new List<Player>() { players.First() };
        for (int i = 1; i < players.Count; i++)
        {
            var hcChallenger = FindHighCard(FindThreeOfKind(players[i].combination.cards));
            if (hcChallenger.First() == highCard.First())
            {
                hcChallenger = FindPair(players[i].combination.cards);
                highCard = FindPair(winners.First().combination.cards);
                if (highCard.First()  == hcChallenger.First())
                {
                    winners.Add(players[i]);
                    highCard = FindHighCard(FindThreeOfKind(winners.First().combination.cards));
                }
                else if(hcChallenger.First() > highCard.First())
                {
                    highCard = FindHighCard(FindThreeOfKind(players[i].combination.cards));
                    winners.Clear();
                    winners.Add(players[i]);
                }
            }
            else if (hcChallenger.First() > highCard.First())
            {
                highCard = hcChallenger;
                winners.Clear();
                winners.Add(players[i]);
            }
        }
        return winners;
    }

    public static List<Player> FindWinnersPair(List<Player> players)
    {
        var highCard = FindHighCard(FindPair(players.First().combination.cards));
        var winners = new List<Player>() { players.First() };
        for (int i = 1; i < players.Count; i++)
        {
            var hcChallenger = FindHighCard(FindPair(players[i].combination.cards));
            if (hcChallenger.First() == highCard.First())
            {
                hcChallenger = FindPair(players[i].combination.cards.FindAll(x => highCard.Contains(x)));
                highCard = FindPair(winners.First().combination.cards.FindAll(x => hcChallenger.Contains(x)));
                if (highCard.First() == hcChallenger.First())
                {
                    winners.Add(players[i]);
                    highCard = FindHighCard(FindPair(winners.First().combination.cards));
                }
                else if (hcChallenger.First() > highCard.First())
                {
                    highCard = FindHighCard(FindPair(players[i].combination.cards));
                    winners.Clear();
                    winners.Add(players[i]);
                }
            }
            else if (hcChallenger.First() > highCard.First())
            {
                highCard = hcChallenger;
                winners.Clear();
                winners.Add(players[i]);
            }
        }
        return winners;
    }


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
