using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utils
{
    public static List<Card> ConCatBoardHand(List<Card> boardCards, Player player)
    => (new List<Card>(boardCards).Concat(player.atributes.Hand)).ToList();

}
