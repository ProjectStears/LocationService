using System;
using UnityEngine;

public static class Config
{
    public static int Zoom = 16;
    public static float MinGPSAcc = 40;
    public static float TimeToGoodGPSFix = 10;

    public static bool UseDebugGPSPosition;
    public static Vector3 DebugGPSPosition;

    static Config()
    {
        DebugGPSPosition = new Vector3(48.050144f, 8.201419f, 100f);

#if UNITY_EDITOR
        UseDebugGPSPosition = true;
#elif UNITY_ANDROID
        UseDebugGPSPosition = false;
#endif
    }

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
