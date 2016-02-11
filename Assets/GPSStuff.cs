using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class GPSStuff : MonoBehaviour
{
    public GameObject TilePrefab;

    public GameObject GoStatus;
    public GameObject GoLat;
    public GameObject GoLon;
    public GameObject GoAlt;
    public GameObject GoTime;
    public GameObject GoHacc;
    public GameObject GoVacc;
    public GameObject GoZoom;
    public GameObject GoTilex;
    public GameObject GoTiley;
    public GameObject GoTimer;
    private GameObject _goMapTile;

    private Text _textStatus;
    private Text _textLat;
    private Text _textLon;
    private Text _textAlt;
    private Text _textTime;
    private Text _textHacc;
    private Text _textVacc;
    private Text _textZoom;
    private Text _textTilex;
    private Text _textTiley;
    private Text _textTimer;

    private Vector2 _tile;

    private bool _initialize = true;
    private bool _goodFix = false;

    private LocationInfo _locInfo;
    private LocationServiceStatus _locStatus;
    
    private float _fixTimer;

    // Use this for initialization
    void Start () {
	    //locSrv = new LocationService();

	    Input.location.Start(0.1f, 0.01f);

	    _textStatus = GoStatus.GetComponent<Text>();
	    _textLat = GoLat.GetComponent<Text>();
	    _textLon = GoLon.GetComponent<Text>();
	    _textAlt = GoAlt.GetComponent<Text>();
	    _textTime = GoTime.GetComponent<Text>();
	    _textHacc = GoHacc.GetComponent<Text>();
        _textVacc = GoVacc.GetComponent<Text>();
        _textZoom = GoZoom.GetComponent<Text>();
        _textTilex = GoTilex.GetComponent<Text>();
        _textTiley = GoTiley.GetComponent<Text>();
        _textTimer = GoTimer.GetComponent<Text>();

        _fixTimer = Config.TimeToGoodGPSFix;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Config.UseDebugGPSPosition)
        {
            SetDebugInfos("Debugging", Config.DebugGPSPosition.x.ToString(CultureInfo.InvariantCulture), Config.DebugGPSPosition.y.ToString(CultureInfo.InvariantCulture), Config.DebugGPSPosition.z.ToString(), "0", "1", "1");
            _tile = Config.WorldToTilePos(Config.DebugGPSPosition.x, Config.DebugGPSPosition.y, Config.Zoom);

            if (_initialize)
            {
                _goodFix = true;
                _goMapTile = Instantiate(TilePrefab);
                _initialize = false;
            }
        }
        else
        {
            _locStatus = Input.location.status;
            _locInfo = Input.location.lastData;

            SetDebugInfos(_locStatus, _locInfo);
            _tile = Config.WorldToTilePos(_locInfo.longitude, _locInfo.latitude, Config.Zoom);

            if (_locStatus == LocationServiceStatus.Running && _locInfo.horizontalAccuracy <= Config.MinGPSAcc && _locInfo.verticalAccuracy <= Config.MinGPSAcc && _fixTimer > 0 && !_goodFix)
            {
                _fixTimer -= Time.deltaTime;
            }
            else
            {
                _fixTimer = Config.TimeToGoodGPSFix;
            }

            _textTimer.text = _fixTimer.ToString(CultureInfo.InvariantCulture);

            if (_fixTimer < 0 && _goodFix == false)
            {
                _goodFix = true;
            }

            if (_goodFix && _initialize)
            {
                _goMapTile = Instantiate(TilePrefab);
                _initialize = false;
            }
        }

        _textZoom.text = Config.Zoom.ToString();
        _textTilex.text = _tile.x.ToString(CultureInfo.InvariantCulture);
        _textTiley.text = _tile.y.ToString(CultureInfo.InvariantCulture);

        if (_goMapTile != null && _goodFix)
        {
            _goMapTile.transform.position = new Vector3(5 - 10*(_tile.x - Mathf.FloorToInt(_tile.x)), -5 + 10*(_tile.y - Mathf.FloorToInt(_tile.y)), 0);
        }
	}

    void OnApplicationQuit()
    {
        Input.location.Stop();
    }

    private void SetDebugInfos(LocationServiceStatus locStatus, LocationInfo locInfo)
    {
        SetDebugInfos(locStatus.ToString(), locInfo.latitude.ToString(CultureInfo.InvariantCulture), locInfo.longitude.ToString(CultureInfo.InvariantCulture), locInfo.altitude.ToString(CultureInfo.InvariantCulture),
            UnixTimeStampToDateTime(locInfo.timestamp).ToString(CultureInfo.InvariantCulture), locInfo.horizontalAccuracy.ToString(CultureInfo.InvariantCulture), locInfo.verticalAccuracy.ToString(CultureInfo.InvariantCulture));
    }

    private void SetDebugInfos(string status, string lat, string lon, string alt, string time, string hacc, string vacc)
    {
        _textStatus.text = status;
        _textLat.text = lat;
        _textLon.text = lon;
        _textAlt.text = alt;
        _textTime.text = time;
        _textHacc.text = hacc;
        _textVacc.text = vacc;

    }

    private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }

    public void EndThis()
    {
        Application.Quit();
    }
}
