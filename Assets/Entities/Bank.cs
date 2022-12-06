using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour
{
    public int Sum { get; set; }
    public void AcceptBet(int bet)
    {
        Sum += bet;
    }

    public void RecieveBankToWiners(List<Player> winners)
    {
        int prize = Sum / winners.Count;
        foreach (var player in winners)
        {
            player.Balance += prize;
        }
    }

    public void NullifyCurrentBank()
    {
        Sum = 0;
    }
}
