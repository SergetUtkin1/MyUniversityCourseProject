using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank
{
    public int Pot { get; set; }
    public int BigBlind { get; set; }
    public int SmallBlind { get; set; }

    public void RequestBet(Player player)
    {
        if(player.IsBot)
        {
            AcceptBet(40, player);
        }
        else
        {
            WaitingForBet(player);
        }
    }

    public void AcceptBet(int bet, Player player)
    {
        Pot += bet;
        player.Bet = bet;
    }

    public void WaitingForBet(Player player)
    {

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
