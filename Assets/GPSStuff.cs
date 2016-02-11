using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class GPSStuff : MonoBehaviour
{
    //private LocationService locSrv;

    public Vector2 DummyPosition;

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

    public float TimeToGoodFix;
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

        _fixTimer = TimeToGoodFix;
    }

    // Update is called once per frame
    void Update ()
	{
	    _textStatus.text = Input.location.status.ToString();
	    _textLat.text = Input.location.lastData.latitude.ToString(CultureInfo.InvariantCulture);
	    _textLon.text = Input.location.lastData.longitude.ToString(CultureInfo.InvariantCulture);
	    _textAlt.text = Input.location.lastData.altitude.ToString(CultureInfo.InvariantCulture);
	    _textTime.text = UnixTimeStampToDateTime(Input.location.lastData.timestamp).ToString(CultureInfo.InvariantCulture);
	    _textHacc.text = Input.location.lastData.horizontalAccuracy.ToString(CultureInfo.InvariantCulture);
	    _textVacc.text = Input.location.lastData.verticalAccuracy.ToString(CultureInfo.InvariantCulture);

        _tile = Config.WorldToTilePos(Input.location.lastData.longitude, Input.location.lastData.latitude, Config.Zoom);
        //_tile = WorldToTilePos(DummyPosition.x, DummyPosition.y, _zoom);

        if (Input.location.status == LocationServiceStatus.Running && Input.location.lastData.horizontalAccuracy <= 20 &&
            Input.location.lastData.verticalAccuracy <= 20 && _fixTimer > 0 && !_goodFix)
        {
            _fixTimer -= Time.deltaTime;
        }
        else
        {
            _fixTimer = TimeToGoodFix;
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
