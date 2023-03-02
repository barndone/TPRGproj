using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoordinateUtils
{
    //  Converts a vector2 from cartesian coordinates to Isometric coordinates
    public static Vector2 ConvertToIsometric(Vector2 pos)
    {
        return new Vector2(pos.x - pos.y, (pos.x + pos.y) / 2);
    }

    //  Converts a Vector2 from Isometric coordinates to Cartesian coordinates
    public static Vector2 ConvertToCartesian(Vector2 pos)
    {
        return new Vector2((pos.x - pos.y) / 1.5f, (pos.x / 3.0f) + (pos.y / 1.5f));
    }

    //  Converts from Isometric world space to a usable index for the Map dictionary key
    public static Vector2 IsoWorldToDictionaryKey(Vector2 pos)
    {
        return new Vector2(Mathf.Abs(2 * pos.y + pos.x), Mathf.Abs(2 * pos.y - pos.x));
    }

    //  calcuate an H score using the Manhattan Distance Heuristic
    //  h = AbsVal(cur.x - dest.x) + AbsVal(cur.y - dest.y)
    public static int CalcManhattanDistance(Vector2 curPos, Vector2 destPos)
    {
        return (int)(Mathf.Abs(curPos.x - destPos.x) + Mathf.Abs(curPos.y - destPos.y));
    }

    public static int CalcEuclideanDistance(Vector2 curPos, Vector2 destPos)
    {
        return (int)Mathf.Sqrt(((destPos.x - curPos.x) * (destPos.x - curPos.x)) + ((destPos.y - curPos.y) * (destPos.y - curPos.y)));
    }
}
