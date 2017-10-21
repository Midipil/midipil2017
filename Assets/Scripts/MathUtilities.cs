using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MathUtilities
{
	static System.Random rand = new System.Random();

    public static TileHandler Draw(List<TileHandler> weightedTiles)
    {
        Dictionary<object, float> dic = new Dictionary<object, float>();
        foreach (var tile in weightedTiles) dic.Add(tile, tile.Weight);

        return Draw(dic) as TileHandler;
    }
    public static object Draw(Dictionary<object, float> weightedObjects)
    {
        double r = rand.NextDouble() * weightedObjects.Sum(a => a.Value);
        double min = 0;
        double max = 0;
        object winner = null;
        foreach (var ticket in weightedObjects)
        {
            max += ticket.Value;
            //-----------
            if (min <= r && r < max)
            {
                winner = ticket.Key;
                break;
            }
            //-----------
            min = max;
        }

        if (winner == null) throw new System.Exception("error with the weighted draw");

        return winner;
    }
}