using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

public class Player : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    [SerializeField] private Button? CallButton;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    [SerializeField] private Button? FoldButton;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    [SerializeField] private Button? BetButton;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    [SerializeField] private InputField? BetInputField;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

    [SerializeField] private Text _betText;
    [SerializeField] private Text _balanceText;

    public float timeRemaining = 10;
    public float timeRemainingbefore = 10;
    public bool timerIsRunning = false;

    private int _balance = 1000;
    private int _betValue = 0;

    public PlayerBoardAtributes atributes;
    public Combination combination;

    [SerializeField] 
    public bool IsBot = true;

    public bool IsActive { get; set; } = true;

    public void NullifyPlayer()
    {
        _betValue = 0;
        IsActive = true;
        atributes.Hand = new List<Card>();
        atributes.Blind = null;
    }

    public int BetValue
    {
        get { return _betValue; }
        set
        {
            if (value < 0)
                throw new Exception("Value of bet cannot be negative");

            if (value > _balance)
                throw new Exception("Value of bet cannot be more than balance");

            Balance -= value;
            _betValue += value;
            _betText.text = $"Bet: {_betValue}";
        }
    }
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

    void Start()
    {
        if (!IsBot)
        {
            DisableButtons();
        }    
    }


    private void StopPlayerTimer()
        => timerIsRunning = false;

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                float seconds = Mathf.FloorToInt(timeRemaining % 60);
                if(seconds < timeRemainingbefore)
                {
                    Debug.Log(seconds);
                    timeRemainingbefore = seconds;
                }
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                StopPlayerTimer();
            }
        }
    }

    private void OnEnable()
    {
        if (!IsBot)
        {
            CallButton.onClick.AddListener(Call);
            FoldButton.onClick.AddListener(Fold);
            BetButton.onClick.AddListener(Bet);

            CallButton.onClick.AddListener(StopPlayerTimer);
            FoldButton.onClick.AddListener(StopPlayerTimer);
            BetButton.onClick.AddListener(StopPlayerTimer);

            gameManager.Bank.OnBettingStart += BetResponse;
        }
    }

    public void EnableButtons()
    {
        CallButton.interactable = true;
        FoldButton.interactable = true;
        BetButton.interactable = true;
        Debug.Log("Buttons are enabled");
    }

    public void DisableButtons()
    {
        CallButton.interactable = false;
        FoldButton.interactable = false;
        BetButton.interactable = false;
        Debug.Log("Buttons are disabled");
    }

    private void BetResponse()
    {
        EnableButtons();
        Debug.Log("Waiting for bet...");

        timeRemainingbefore = 10;
        timeRemaining = timeRemainingbefore;
        timerIsRunning = true;      
    }

    public void Bet()
    {
        if (int.TryParse(BetInputField.text, out var newBet))
        {
           Debug.Log($"Bet is {newBet}");
           BetValue = newBet;
        }
        else if(String.IsNullOrWhiteSpace(BetInputField.text))
        {
            newBet = 0;
            Debug.Log($"Bet is {newBet}");
            BetValue = newBet;
        }
        else
        {
            throw new Exception("Cannot convert text to int");
        }
        DisableButtons();
        gameManager.Bank.cts.Cancel();
    }

    public void Call()
    {
        Debug.Log("It's call");
        var newBet = gameManager.Bank.CurrentBet - BetValue;
        BetValue = newBet;
        DisableButtons();
        gameManager.Bank.cts.Cancel();
    }

    public void Fold()
    {
        Debug.Log("It's Fold");
        BetValue = 0;
        DisableButtons();
        gameManager.Bank.cts.Cancel();
    }
}
