using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindMaster : MonoBehaviour
{
    [SerializeField]
    private static readonly Sprite[] BlindFaces;
    private readonly string[] BlindNames = {
        "Dealer",
        "Small Blind",
        "Big Blind"
    };

    public List<Blind> GenerateBlinds()
    {
        var blinds = new List<Blind>();

        for (int i = 0; i < BlindNames.Length; i++)
        {
            var figure = new Figure()
            {
                Face = BlindFaces[i],
            };
            var blind = Blind.CreateBlind(gameObject, BlindNames[i], figure);
        }

        return blinds;
    }
}