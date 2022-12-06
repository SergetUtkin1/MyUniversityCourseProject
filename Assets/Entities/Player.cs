using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class Player : MonoBehaviour
{
    private int _balance;

    [SerializeField] 
    public bool IsBot = true;
    public bool IsActive { get; set; }
    public PlayerBoardAtributes atributes;
    public Combination combination;

    public int Balance
    {
        get { return _balance; }
        set 
        {
            if(value < 0)
            {
                throw new Exception("Value of balance cannot be negative");
            }
            _balance = value; 
        }
    }

    public void Fold()
    {

    }

    public void Bet(int betValue)
    {

    }
}
