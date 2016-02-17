using UnityEngine;
using MySql.Data.MySqlClient;

public class DatabaseStuff : MonoBehaviour
{
    private MySqlConnection conn;
    string myConnectionString;
    MySqlCommand cmd;

    private bool update;

    // Use this for initialization
    void Start () {
        myConnectionString = "server=127.0.0.1;uid=root;pwd=;database=stears;";
        conn = new MySqlConnection();
        conn.ConnectionString = myConnectionString;
        cmd = new MySqlCommand();

        try
        {
            conn.Open();
        }
        catch (MySqlException ex)
        {
            Debug.LogWarning(ex.Message);
        }
        update = true;
    }
	
	// Update is called once per frame
	void Update () {
	    if (update)
	    {
	        GetTowers();
	    }
	}
    void OnApplicationQuit()
    {
        conn.Close();
    }


    private void GetTowers()
    {
        update = false;
        try
        {
            cmd.Connection = conn;
            cmd.CommandText = "getAllUsers";
            MySqlDataReader data = cmd.ExecuteReader();

            if (data.HasRows)
            {
                while (data.Read())
                {
                    Debug.Log(data[2]);
                }
            }
        }
        catch (MySqlException ex)
        {
            Debug.LogWarning("Error " + ex.Number + " has occurred: " + ex.Message);
        }
    }
    
}
