using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blind
{
    public string Name { get; set; }
    public Figure BlindFigure { get; set; }

    public Blind(string name, Figure blindFigure)
    {
        Name = name;
        BlindFigure = blindFigure;
    }
}
