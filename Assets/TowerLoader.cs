using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

public class TowerLoader : MonoBehaviour
{
    private List<GameObject> towers;
    private MySqlCommand DbCommand;
    private bool reload;
    private bool loadInProgress;
    public GameObject towerParent;
    public GameObject towerPrefab;


	// Use this for initialization
	void Start () {

        towers = new List<GameObject>();
	    DbCommand = new MySqlCommand();
	    DbCommand.Connection = Config.DbConnection;
	    reload = true;

	}
	
	// Update is called once per frame
	void Update () {

	    if (Config.GoodGPSFix)
	    {
	        if (reload && !loadInProgress)
	        {
	            reload = false;
	            loadInProgress = true;
	            StartCoroutine(LoadTowers());
	        }
	        else
	        {
	            reload = false;
	        }
	    }
	}


    IEnumerator LoadTowers()
    {
        foreach (var tower in towers)
        {
            Destroy(tower);
        }
        towers.Clear();

        DbCommand.Connection = Config.DbConnection;
        DbCommand.CommandText = "call getTowersInArea(" + (Config.CurrentGPSPosition.x - 1) + "," + (Config.CurrentGPSPosition.y - 1) + "," + (Config.CurrentGPSPosition.x + 1) + "," + (Config.CurrentGPSPosition.y + 1) + ")";
        Debug.Log(DbCommand.CommandText);
        MySqlDataReader data = DbCommand.ExecuteReader();

        yield return DbCommand;

        if (data.HasRows)
        {
            while (data.Read())
            {
                GameObject go = Instantiate(towerPrefab);
                go.name = data[2] + "x" + data[3];
                go.transform.parent = towerParent.transform;
                towers.Add(go);
            }
        }
        loadInProgress = false;
    }




}
