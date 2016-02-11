using System;
using UnityEngine;

public static class Config
{

    public static int Zoom = 16;

    public static Vector2 WorldToTilePos(double lon, double lat, int zoom)
    {
        Vector2 p = new Vector2
        {
            x = (float)((lon + 180.0) / 360.0 * (1 << zoom)),
            y = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom))
        };

        return p;
    }

}
