using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blind : MonoBehaviour
{
    public string Name { get; set; }
    public Player Owner { get; set; }
    public Figure BlindFigure { get; set; }

    public static Blind CreateBlind(GameObject where, string name, Figure blindFigure)
    {
        Blind blind = where.AddComponent<Blind>();
        blind.Name = name;
        blind.BlindFigure = blindFigure;
        return blind;
    }
}
