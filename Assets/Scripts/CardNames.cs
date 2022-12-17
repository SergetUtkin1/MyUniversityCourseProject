using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardNames : MonoBehaviour
{
    [SerializeField] 
    public Sprite[] cardFaces;
    [SerializeField] 
    public Sprite cardBack;
    public static readonly string[] valuesOfCards = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
    public static readonly string[] suits = { "clubs", "diamonds", "hearts", "spades" };
}