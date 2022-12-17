using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] CardNames cardConsts; 

    public static int GetIndexOfCard(string name)
    {
        for (int i = 0; i < CardNames.valuesOfCards.Length; i++)
        {
            if (name == CardNames.valuesOfCards[i])
            {
                return i;
            }
        }
        return -1;
    }

    public List<Card> GenerateNewDeck()
    {
        int cnt = 0;
        List<Card> new_deck = new List<Card>();
        foreach (var val in CardNames.valuesOfCards)
        {
            foreach (var s in CardNames.suits)
            {
                Figure figure = new Figure()
                {
                    Face = cardConsts.cardFaces[cnt],
                };
                Card card =Card.CreateCard(gameObject, val, s, figure);
                new_deck.Add(card);
                cnt++;
            }
        }

        return new_deck;
    }

    public void Shuffle<T>(IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
