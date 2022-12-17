using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Bank
{
    public int Pot { get; set; } = 0;
    public int CurrentBetValue { get; set; } = 0; //суммарно
    public int BigBlind { get; set; } = 40;
    public int SmallBlind { get { return GetSmallBlind(); } set { SmallBlind = value; } }
    private int _respsonses;
    private int _additionalAmount;

    public CancellationTokenSource BetCancellationTokenSource { get; set; } = new CancellationTokenSource();

    public Action OnBettingStart;
    public Action OnBettingFinished;

    private int GetSmallBlind()
        => BigBlind / 2;

    public void AcceptBlinds(Player[] players)
    {
        foreach (var player in players)
        {
            if(player.atributes.Blind != null)
                switch (player.atributes.Blind.Name)
                {
                    case "Big Blind":
                        player.BetValue = BigBlind;
                        Pot += BigBlind;
                        CurrentBetValue = BigBlind;
                        break;

                    case "Small Blind":
                        player.BetValue = SmallBlind;
                        Pot += SmallBlind;
                        break;

                    default:
                        break;
                }
        }
        _respsonses = 2;
        _additionalAmount = 1;
    }

    public async Task RequestBet(Player[] players)
    {
        var ListOfPlayers = players.ToList();
        
        var CBIsChanched = false;

        while(_respsonses < ListOfPlayers.Count + _additionalAmount)
        {
            var index = _respsonses % ListOfPlayers.Count;

            if(ListOfPlayers[index].IsActive)
            {
                var bet = await WaitingForBet(ListOfPlayers[index]);
                if (bet > CurrentBetValue)
                {
                    CurrentBetValue = bet;
                    CBIsChanched = true;
                }
                else if (bet < CurrentBetValue)
                {
                    ListOfPlayers[index].IsActive = false;
                    //ListOfPlayers.RemoveAt(index);
                }
            }

            if(CBIsChanched)
            {
                if(index == 0)
                {
                    _respsonses = 0;
                }
                else
                {
                    _additionalAmount = index;
                }

                CBIsChanched = false;
            }

            _respsonses++;
        }
        _respsonses = 0;
        _additionalAmount = 0;

        OnBettingFinished.Invoke();
    }

    private int BetForBot(Player bot)
        => CurrentBetValue - bot.BetValue;

    public async Task<int> WaitingForBet(Player player)
    {
        var timeCts = new CancellationTokenSource();
        player.StartPlayerTimer();

        if (player.IsBot)
        {
            timeCts.CancelAfter(TimeSpan.FromSeconds(3));

            var flag = true;
            while (flag)
            {
                if (timeCts.Token.IsCancellationRequested)
                {
                    flag = false;
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            var newbet = BetForBot(player);
            Pot += newbet;
            player.BetValue = newbet;
        }
        else
        {
            BetCancellationTokenSource = new CancellationTokenSource();
            var currentPlayerBet = player.BetValue;
            OnBettingStart.Invoke();
            timeCts.CancelAfter(TimeSpan.FromSeconds(5));

            var flag = true;
            while(flag)
            {
                if(BetCancellationTokenSource.Token.IsCancellationRequested)
                {
                    flag = false;
                }

                if(timeCts.Token.IsCancellationRequested)
                {
                    player.Fold();
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            Pot += player.BetValue - currentPlayerBet;
        }

        player.StopPlayerTimer();

        return player.BetValue;
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
        CurrentBetValue = 0;
    }
}
