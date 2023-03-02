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

}
