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

                case "Flush":
                    winners = FindWinnersFLush(players);
                    break;

                case "Two Pair":
                    winners = FindWinnersTwoPair(players);
                    break;

                default:
                    winners = FindWinnersByHighCard(players);
                    break;
            }

            if (winners.Count > 1)
            {
                winners = FindWinnersByKicker(winners, boardCards);
            }

            return winners;
        }
        else
        {
            return players;
        }    
    }

    public static List<Player> FindWinnersByKicker(List<Player> players, List<Card> boardCards)
    {
        if (players.First().combination.cards.Count != 5)
        {
            var winners = new List<Player>() { players.First() };
            var kickers = Utils.ConCatBoardHand(boardCards, players.First())
            .FindAll(x => !players.First().combination.cards
            .Any(c => c == x)).OrderBy(x => x.Rank).TakeLast(5 - players.First().combination.cards.Count).ToList();
            for (int i = 1; i < players.Count; i++)
            {
                var kickersChallenger = Utils.ConCatBoardHand(boardCards, players[i])
                    .FindAll(x => !players[i].combination.cards
                    .Any(c => c == x)).OrderBy(x => x.Rank).TakeLast(5 - players[i].combination.cards.Count).ToList();

                var cnt = 0;
                for (int j = kickers.Count - 1; j >= 0; j--)
                {
                    if (kickers[j] == kickersChallenger[j])
                    {
                        cnt++;
                    }
                    else if(kickers[j] < kickersChallenger[j])
                    {
                        winners.Clear();
                        winners.Add(players[i]);
                        kickers = kickersChallenger;
                        break;
                    }
                }

                if(cnt == kickers.Count)
                {
                    winners.Add(players[i]);
                }
            }
            return winners;
        }
        else
        {
            return players;
        }
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

    public static List<Player> FindWinnersTwoPair(List<Player> players)
    {
        var highCard = FindHighCard(players.First().combination.cards);
        var winners = new List<Player>() { players.First() };
        for (int i = 1; i < players.Count; i++)
        {
            var hcChallenger = FindHighCard(players[i].combination.cards);
            if (hcChallenger.First() == highCard.First())
            {
                hcChallenger = FindPair(players[i].combination.cards.FindAll(x => !hcChallenger.Contains(x)));
                highCard = FindPair(winners.First().combination.cards.FindAll(x => !highCard.Contains(x)));
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

    public static List<Player> FindWinnersFLush(List<Player> players)
    {
        var highFlush = players.First().combination.cards
            .OrderByDescending(x => x.Rank).Take(5).ToList();
        var winners = new List<Player>() { players.First() };
        for (int i = 0; i < players.Count; i++)
        {
            var highFlushChallenger = players[i].combination.cards
            .OrderByDescending(x => x.Rank).Take(5).ToList();

            var flag = true;
            for (int j = 0; j < 5; j++)
            {
                if (highFlush[j] < highFlushChallenger[j])
                {
                    highFlush = highFlushChallenger;
                    winners.Clear();
                    winners.Add(players[i]);
                    flag = false;
                    break;
                }
                else if (highFlush[j] > highFlushChallenger[j])
                {
                    flag = false;
                    break;
                }
            }

            if(flag)
            {
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
        cards = cards.OrderByDescending(x => x.Rank).ToList();
        combinations.AddRange(cards.FindAll(c => cards.Count(x => x == c) == 2));

        return combinations.Take(2).ToList();
    }

    public static List<Card> FindTwoPair(List<Card> cards)
    {
        var combinations = new List<Card>();
        var cardsCopy = new List<Card>(cards);
        cardsCopy.OrderByDescending(x => x.Value);
        for (int i = 0; i < 2; i++)
        {
            var pair = FindPair(cardsCopy);
            combinations.AddRange(pair);
            cardsCopy.RemoveAll(x => pair.Contains(x));
        }

        return combinations.Count >= 4 ? combinations.Take(4).ToList() : new List<Card>();
    }

    public static List<Card> FindThreeOfKind(List<Card> cards)
    {
        var combinations = new List<Card>();
        cards = cards.OrderByDescending(x => x.Rank).ToList();

        combinations.AddRange(cards.FindAll(c => cards.Count(x => x == c) == 3));

        return combinations.Take(3).ToList();
    }

    public static List<Card> FindStraight(List<Card> cards)
    {
        var combinations = new List<Card>();
        combinations.Add(cards[0]);
        cards = cards.OrderBy(x => x.Rank).ToList();
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
        cards = cards.OrderBy(x => x.Rank).ToList();
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
        cardCopy.OrderByDescending(x => x.Rank);
        var threeCards = FindThreeOfKind(cardCopy);
        combinations.AddRange(threeCards);
        cardCopy.RemoveAll(x => threeCards.Contains(x));
        combinations.AddRange(FindPair(cardCopy));

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
