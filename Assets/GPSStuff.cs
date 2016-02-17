using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class GPSStuff : MonoBehaviour
{
    public GameObject TilePrefab;
    public GameObject PosMarkerPrefab;

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
    public GameObject GoTouch1;
    public GameObject GoTouch2;
    public GameObject GoTouchAngle;
    private GameObject _goMapRoot;
    private GameObject _goPosMarker;

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
    private Text _textTouch1;
    private Text _textTouch2;
    private Text _textTouchAngle;

    private Vector2 _tile;
    private Vector2 _cameraOffset;
    private float _angle;

    private bool _initialize = true;

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
        _textTouch1 = GoTouch1.GetComponent<Text>();
        _textTouch2 = GoTouch2.GetComponent<Text>();
        _textTouchAngle = GoTouchAngle.GetComponent<Text>();
        
        _fixTimer = Config.TimeToGoodGPSFix;
    }

    // Update is called once per frame
    void Update ()
    {
        _textTouch1.text = Vector2.zero.ToString();
        _textTouch2.text = Vector2.zero.ToString();

        if (Config.UseDebugGPSPosition)
        {
            Config.CurrentGPSPosition = Config.DebugGPSPosition;

            SetDebugInfos("Debugging", Config.DebugGPSPosition.x.ToString(CultureInfo.InvariantCulture), Config.DebugGPSPosition.y.ToString(CultureInfo.InvariantCulture), Config.DebugGPSPosition.z.ToString(), "0", "1", "1");
            _tile = Config.WorldToTilePos(Config.DebugGPSPosition.x, Config.DebugGPSPosition.y, Config.Zoom);

            if (_initialize)
            {
                LoadMap();
            }
        }
        else
        {
            _locStatus = Input.location.status;
            _locInfo = Input.location.lastData;

            Config.CurrentGPSPosition.x = _locInfo.longitude;
            Config.CurrentGPSPosition.y = _locInfo.latitude;

            SetDebugInfos(_locStatus, _locInfo);
            _tile = Config.WorldToTilePos(_locInfo.longitude, _locInfo.latitude, Config.Zoom);

            if (_locStatus == LocationServiceStatus.Running && _locInfo.horizontalAccuracy <= Config.MinGPSAcc && _locInfo.verticalAccuracy <= Config.MinGPSAcc && _fixTimer > 0 && !Config.GoodGPSFix)
            {
                _fixTimer -= Time.deltaTime;
            }
            else
            {
                _fixTimer = Config.TimeToGoodGPSFix;
            }

            _textTimer.text = _fixTimer.ToString(CultureInfo.InvariantCulture);

            if (_fixTimer < 0 && Config.GoodGPSFix == false)
            {
                Config.GoodGPSFix = true;
            }

            if (Config.GoodGPSFix && _initialize)
            {
                LoadMap();
            }
        }

        _textZoom.text = Config.Zoom.ToString();
        _textTilex.text = _tile.x.ToString(CultureInfo.InvariantCulture);
        _textTiley.text = _tile.y.ToString(CultureInfo.InvariantCulture);
#if UNITY_EDITOR
        _cameraOffset.x = Mathf.Clamp(_cameraOffset.x - Input.GetAxis("Horizontal") * Time.deltaTime, -Config.MaxCameraOffset.x, Config.MaxCameraOffset.x);
        _cameraOffset.y = Mathf.Clamp(_cameraOffset.y + Input.GetAxis("Vertical") * Time.deltaTime, -Config.MaxCameraOffset.y, Config.MaxCameraOffset.y);
        Camera.main.transform.Rotate(Vector3.forward, Input.GetAxis("Rotation"));

#elif UNITY_ANDROID
        if (Input.touchCount == 1)
        {
            _textTouch1.text = Input.GetTouch(0).position.ToString();
            _cameraOffset.x = Mathf.Clamp(_cameraOffset.x - Input.GetTouch(0).deltaPosition.x * Time.deltaTime, -Config.MaxCameraOffset.x, Config.MaxCameraOffset.x);
            _cameraOffset.y = Mathf.Clamp(_cameraOffset.y + Input.GetTouch(0).deltaPosition.y * Time.deltaTime, -Config.MaxCameraOffset.y, Config.MaxCameraOffset.y);
        }

        if (Input.touchCount == 2)
        {
            var _newAngle = Vector2.Angle(Input.GetTouch(0).position, Input.GetTouch(1).position);
            _textTouch1.text = Input.GetTouch(0).position.ToString();
            _textTouch2.text = Input.GetTouch(1).position.ToString();


            _textTouchAngle.text = _angle.ToString(CultureInfo.InvariantCulture);
            if (_angle != 0)
            {
                Camera.main.transform.Rotate(Vector3.forward, _angle - _newAngle);
            }
            _angle = _newAngle;

            _textTouchAngle.text = _angle.ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            _angle = 0;
        }
#endif

        if (_goMapRoot != null && Config.GoodGPSFix)
        {
            _goMapRoot.transform.position = new Vector3((5 - 10*(_tile.x - Mathf.FloorToInt(_tile.x)) - _cameraOffset.x), (-5 + 10*(_tile.y - Mathf.FloorToInt(_tile.y)) + _cameraOffset.y), 0);
            _goPosMarker.transform.position = new Vector3(-_cameraOffset.x, _cameraOffset.y, 0);
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

    private void LoadMap()
    {
        Config.GoodGPSFix = true;
        _goMapRoot = new GameObject("MapRoot");

        var go = Instantiate(TilePrefab);
        go.transform.parent = _goMapRoot.transform;

        _goPosMarker = Instantiate(PosMarkerPrefab);
        _initialize = false;
    }
}
