using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Bank
{
    public int Pot { get; set; } = 0;
    public int CurrentBet { get; set; } = 0;
    public int BigBlind { get; set; } = 40;
    public int SmallBlind { get { return GetSmallBlind(); } set { SmallBlind = value; } }

    public CancellationTokenSource cts { get; set; } = new CancellationTokenSource();

    public Action OnBettingStart;

    private int GetSmallBlind()
        => BigBlind / 2;

    public async Task RequestBet(Player[] players)
    {
        var ListOfPlayers = players.ToList().FindAll(p => p.IsActive == true);

        var respsonses = 0;
        var CBIsChanched = false;
        var additionalAmount = 0;
        while(respsonses < ListOfPlayers.Count + additionalAmount)
        {
            var index = respsonses % ListOfPlayers.Count;

            if (ListOfPlayers[index].IsBot)
            {
                var bet = AcceptBet(BetForBot(), ListOfPlayers[index]);
                if (bet > CurrentBet)
                {
                    CurrentBet = bet;
                    CBIsChanched = true;
                }
                else if (bet < CurrentBet)
                {
                    ListOfPlayers[index].IsActive = false;
                    ListOfPlayers.RemoveAt(index);
                }
            }
            else
            {
                var bet = await WaitingForBet(ListOfPlayers[index]);
                if (bet > CurrentBet)
                {
                    CurrentBet = bet;
                    CBIsChanched = true;
                }
                else if (bet < CurrentBet)
                {
                    ListOfPlayers[index].Fold();
                    ListOfPlayers[index].IsActive = false;
                    ListOfPlayers.RemoveAt(index);
                }
            }

            if(CBIsChanched)
            {
                if(index == 0)
                {
                    respsonses = 0;
                }
                else
                {
                    additionalAmount = index;
                }

                CBIsChanched = false;
            }

            respsonses++;
        }
    }

    private int BetForBot()
        => CurrentBet > 40 ? CurrentBet : 40;

    public int AcceptBet(int newbet, Player player)
    {
        Pot += newbet;
        player.BetValue = newbet;
        return newbet;
    }

    public async Task<int> WaitingForBet(Player player)
    {
        cts = new CancellationTokenSource();
        var currentPlayerBet = player.BetValue;
        OnBettingStart.Invoke();
        cts.CancelAfter(TimeSpan.FromSeconds(10));

        var flag = true;
        while(flag)
        {
            if(cts.Token.IsCancellationRequested)
            {
                flag = false;
            }
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        var bet = player.BetValue;
        Pot += bet - currentPlayerBet;

        return bet;
    }

    public void RecieveBankToWiners(List<Player> winners)
    {
        int prize = Pot / winners.Count;
        foreach (var player in winners)
        {
            player.Balance += prize;
        }
    }

    public void NullifyCurrentBank()
    {
        Pot = 0;
    }
}
