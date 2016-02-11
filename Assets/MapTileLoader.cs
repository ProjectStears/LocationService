using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapTileLoader : MonoBehaviour {

    public string url;
    public Vector2 pos;

    // Use this for initialization
    IEnumerator Start () {
        pos = Config.WorldToTilePos(Input.location.lastData.longitude, Input.location.lastData.latitude, Config.Zoom);
        url = "http://a.tile.openstreetmap.org/" + Config.Zoom + "/" + Mathf.FloorToInt(pos.x) + "/" + Mathf.FloorToInt(pos.y) + ".png";
        GameObject.Find("url").GetComponent<Text>().text = url;
        WWW www = new WWW(url);
        yield return www;
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = www.texture;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
//http://a.tile.openstreetmap.org/15/17131/11383.png