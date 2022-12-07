using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject CardPrefab;
    [SerializeField] Deck deck;
    [SerializeField] private Player[] _players;
    [SerializeField] private Board board;
    private List<Card> cards;
    private Combination BestCombinationAtGame; 
    
    void Start()
    {
        cards = deck.GenerateNewDeck();
        deck.Shuffle(cards);
        PlayPreFlop();
    }

    public void ShowCards(Player player)
    {
        for (int i = 0; i < 2; i++)
        {
            player.atributes.handObject[i].GetComponent<SpriteRenderer>().sprite = player.atributes._hand[i].CardFigure.Face;
        }
    }

    public void HideCards(Player player)
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject newCard = Instantiate(CardPrefab, player.atributes.HandPosition[i].transform.position, player.atributes.HandPosition[i].transform.rotation);
            player.atributes.handObject.Add(newCard);

            player.atributes.handObject[i].GetComponent<SpriteRenderer>().sprite = deck.GetComponent<CardNames>().cardBack;
        }
    }

    public void PlayPreFlop()
    {
        for (int i = 0; i < 5; i++)
        {
            board.AddCardToBoard(cards[0]);
            GameObject newCard = Instantiate(CardPrefab, board.cardPlaces[i].transform.position, board.cardPlaces[i].transform.rotation);
            board.objectsOnBoard.Add(newCard);

            newCard.GetComponent<SpriteRenderer>().sprite = deck.GetComponent<CardNames>().cardBack;
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
    }

    public void PlayFlop()
    {
        for (int i = 0; i < 3; i++)
        {
            board.objectsOnBoard[i].GetComponent<SpriteRenderer>().sprite = board.boadCards[i].CardFigure.Face;
        }
    }

    public void PlayTurn()
    {
        board.objectsOnBoard[3].GetComponent<SpriteRenderer>().sprite = board.boadCards[3].CardFigure.Face;

    }

    public void PlayRiver()
    {
        board.objectsOnBoard[4].GetComponent<SpriteRenderer>().sprite = board.boadCards[4].CardFigure.Face;
    }

    public void ShowDown()
    {
        BestCombinationAtGame = CombinationMaster.FindBestCombination((new List<Card>(_players.First().atributes.Hand)).Concat(board.boadCards).ToList());
        foreach (var player in _players)
        {
            ShowCards(player);
            player.combination = CombinationMaster.FindBestCombination((new List<Card>(player.atributes.Hand)).Concat(board.boadCards).ToList());
            if (player.combination.Rank < BestCombinationAtGame.Rank)
                BestCombinationAtGame = player.combination;


            Debug.Log($"player with: {player.combination.Name + player.combination.Rank}");
        }
        
    }

    public List<Player> DefineWinners()
    {
        var winners = CombinationMaster.FindWinners(_players.ToList().FindAll(p => p.combination.Rank == BestCombinationAtGame.Rank), board.boadCards);
        
        foreach(var player in winners)
        {
            for (int i = 0; i < 2; i++)
            {
                player.atributes.handObject[i].GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.5f);
            }
        }
        return winners;
    }

    public void RestartGame()
        => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
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
            DefineWinners();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            DefineWinners();
        }
    }
}