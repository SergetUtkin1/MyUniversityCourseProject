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
        var ListOfPlayers = players.ToList();

        var respsonses = 0;
        var CBIsChanched = false;
        var additionalAmount = 0;
        while(respsonses < ListOfPlayers.Count(p => p.IsActive == true) + additionalAmount)
        {
            var index = respsonses % ListOfPlayers.Count(p => p.IsActive == true);

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
                }
            }

            if(CBIsChanched)
            {
                additionalAmount = index;
                CBIsChanched = false;
            }

            respsonses++;
        }
    }

    private int BetForBot()
        => CurrentBet < 40 ? 40 : CurrentBet;

    public int AcceptBet(int bet, Player player)
    {
        Pot += bet;
        player.BetValue = bet;
        return bet;
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
