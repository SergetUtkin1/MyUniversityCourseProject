using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    //[SerializeField] private GameObject CardPrefab;
    //[SerializeField] private GameObject BlindPrefab;
    [SerializeField] private Deck deck;
    [SerializeField] private BlindMaster blindMaster;
    //private Bank _bank;
    [SerializeField] private Player[] _players;
    [SerializeField] private Board board;

    private List<Card> cards;
    private Combination Nuts;

    [SerializeField] private Text _PotText;

    public bool IsBettingFinished { get; set; } = false;
    public Bank Bank { get; private set; } = new Bank();

    void Start()
    {
        StartCoroutine(PlayGameSequence());
    }

    void SetBlinds()
    {
        blindMaster.GenerateBlinds();
        _players.First().atributes.Blind = blindMaster.SmallBlind;
        _players[1].atributes.Blind = blindMaster.BigBlind;
        _players.Last().atributes.Blind = blindMaster.DealerBlind;
    }

    public void ShowCards(Player player)
    {
        //ICardShowable => ShowCards
        for (int i = 0; i < 2; i++)
        {
            player.atributes.HandPosition[i].GetComponent<SpriteRenderer>().sprite = player.atributes._hand[i].CardFigure.Face;
            player.atributes.HandPosition[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }

    public void ShowBlinds()
    {
        foreach (var player in _players)
        {
            player.atributes.BlindPosition.GetComponent<SpriteRenderer>().sprite = null;
            if (player.atributes.Blind != null)
            {
                player.atributes.BlindPosition.GetComponent<SpriteRenderer>().sprite = player.atributes.Blind.BlindFigure.Face;
            }
        }
    }

    public void HideCards(Player player)
    {
        for (int i = 0; i < 2; i++)
        {
            player.atributes.HandPosition[i].GetComponent<SpriteRenderer>().sprite = deck.GetComponent<CardNames>().cardBack;
            player.atributes.HandPosition[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }


    public void PlayPreFlop()
    {
        SetBlinds();
        ShowBlinds();

        Bank.AcceptBlinds(_players);

        for (int i = 0; i < 5; i++)
        {
            board.AddCardToBoard(cards[0]);

            board.cardPlaces[i].GetComponent<SpriteRenderer>().sprite = deck.GetComponent<CardNames>().cardBack;
            cards.RemoveAt(0);
        }

        foreach (var player in _players)
        {
            player.atributes.AddCardToHand(cards[0]);
            player.atributes.AddCardToHand(cards[1]);
            cards.RemoveRange(0, 2);
            HideCards(player);
            if (!player.IsBot)
            ShowCards(player);  
        }
        _ = Bank.RequestBet(_players);
    }

    public void PlayFlop()
    {
        for (int i = 0; i < 3; i++)
        {
            board.cardPlaces[i].GetComponent<SpriteRenderer>().sprite = board.boadCards[i].CardFigure.Face;
        }

        _ = Bank.RequestBet(_players);
    }

    public void PlayTurn()
    {
        board.cardPlaces[3].GetComponent<SpriteRenderer>().sprite = board.boadCards[3].CardFigure.Face;
        _ = Bank.RequestBet(_players);
    }

    public void PlayRiver()
    {
        board.cardPlaces[4].GetComponent<SpriteRenderer>().sprite = board.boadCards[4].CardFigure.Face;
        _ = Bank.RequestBet(_players);
    }

    public void ShowDown()
    {
        Nuts = CombinationMaster.FindBestCombination((new List<Card>(_players.First().atributes.Hand)).Concat(board.boadCards).ToList());
        foreach (var player in _players)
        {
            if(player.IsActive)
            {
                ShowCards(player);
                player.combination = CombinationMaster.FindBestCombination((new List<Card>(player.atributes.Hand)).Concat(board.boadCards).ToList());
                if (player.combination.Rank < Nuts.Rank)
                    Nuts = player.combination;


                Debug.Log($"player with: {player.combination.Name + player.combination.Rank}");
            }
        }
        
    }

    public List<Player> DefineWinners()
    {
        var winners = CombinationMaster.FindWinners(_players.ToList().FindAll(p => p.combination.Rank == Nuts.Rank && p.IsActive), board.boadCards);
        
        foreach(var player in winners)
        {
            for (int i = 0; i < 2; i++)
            {
                player.atributes.HandPosition[i].GetComponent<SpriteRenderer>().color = new Color(1, 0.8f, 0.05f, 1);
            }
        }
        return winners;
    }

    public void MovePlayers()
    {
        var temp = _players[0];
        for (int i = 0; i < _players.Length; i++)
        {
            if(i != _players.Length - 1)
            {
                _players[i] = _players[ i + 1];
            }
            else
            {
                _players[i] = temp;
            }
            Debug.Log(_players[i].IsBot);
        }
    }

    public void ContinueGame()
    {
        foreach (var player in _players)
        {
            if(player.Balance != 0)
            player.NullifyPlayer();
        }
        board.boadCards.Clear();
        Bank.NullifyCurrentBank();
        MovePlayers();
        cards = deck.GenerateNewDeck();
        deck.Shuffle(cards);
        PlayPreFlop();
    }

    public void RestartGame()
        => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    void OnEnable()
    {
        Bank.OnBettingFinished += BettingEndLisentner;
    }

    void OnDisable()
    {
        Bank.OnBettingFinished -= BettingEndLisentner;
    }

    private void BettingEndLisentner()
    {
        IsBettingFinished = true;
    }

    void Update()
    {
        _PotText.text = $"Pot: {Bank.Pot}";

        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayFlop();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayTurn();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayRiver();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ShowDown();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Bank.RecieveBankToWiners(DefineWinners());
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            RestartGame();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ContinueGame();
        }
    }

    IEnumerator PlayGameSequence()
    {
        ContinueGame();
        while(!IsBettingFinished)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        IsBettingFinished = false;

        PlayFlop();
        while (!IsBettingFinished)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        IsBettingFinished = false;

        PlayTurn();
        while (!IsBettingFinished)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        IsBettingFinished = false;

        PlayRiver();
        while (!IsBettingFinished)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        IsBettingFinished = false;

        ShowDown();
        yield return new WaitForSeconds(1);
        DefineWinners();

    }
}