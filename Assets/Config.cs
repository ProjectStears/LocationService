using System;
using System.Net;
using MySql.Data.MySqlClient;
using UnityEngine;

public static class Config
{
    public static int Zoom = 16;
    public static float MinGPSAcc = 40;
    public static float TimeToGoodGPSFix = 10;
    public static bool GoodGPSFix = false;
    public static Vector2 MaxCameraOffset;
    public static Vector2 CurrentGPSPosition;


    public static bool UseDebugGPSPosition;
    public static Vector3 DebugGPSPosition;

    public static MySqlConnection DbConnection;


    static Config()
    {
        DbConnection = new MySqlConnection("server=127.0.0.1;uid=root;pwd=;database=stears;");
        try
        {
            DbConnection.Open();
        }
        catch (MySqlException ex)
        {
            Debug.LogWarning(ex.Message);
        }

        DebugGPSPosition = new Vector3(48.050144f, 8.201419f, 100f);
        MaxCameraOffset = new Vector2(5f, 5f);
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
