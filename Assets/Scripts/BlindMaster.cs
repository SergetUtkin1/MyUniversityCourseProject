using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindMaster : MonoBehaviour
{
    [SerializeField] 
    private Sprite[] BlindFaces;
    private readonly string[] BlindNames = {
        
            "Big Blind",
            "Small Blind",
            "Dealer"
    };


    public Blind BigBlind { get; set; }
    public Blind SmallBlind { get; set; }
    public Blind DealerBlind { get; set; }

    public void GenerateBlinds()
    {
        for (int i = 0; i < BlindNames.Length; i++)
        {
            var figure = new Figure()
            {
                Face = BlindFaces[i],
            };
            var blind = new Blind(BlindNames[i], figure);

            switch (i)
            {
                case 0:
                    BigBlind = blind;
                    break;
                case 1:
                    SmallBlind = blind;
                    break;
                case 2:
                    DealerBlind = blind;
                    break;
                default:
                    break;
            }
        }
    }
}