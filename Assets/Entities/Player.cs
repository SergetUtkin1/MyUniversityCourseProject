using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

public class Player : MonoBehaviour
{
    [SerializeField] private Button FoldButton;
    [SerializeField] private Button CallButton;
    [SerializeField] private Button BetButton;
    [SerializeField] private InputField BetInputField;
    [SerializeField] private Text _betText;
    [SerializeField] private Text _balanceText;

    private int _balance = 1000;
    private int _bet = 0;

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
            _balanceText.text = $"Balance: {value}";
            _balance = value; 
        }
    }

    public int Bet
    {
        get { return _bet; }
        set
        {
            if (value < 0)
                throw new Exception("Value of bet cannot be negative");

            if(value < _balance)
                throw new Exception("Value of bet cannot be less than balance");

            _balance -= value;
            _betText.text = $"Bet: {value}";
            _bet = value;
        }
    }

    public void Fold()
    {

    }
}
