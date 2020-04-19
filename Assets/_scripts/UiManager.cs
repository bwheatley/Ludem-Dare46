using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour {

    public static UiManager instance;
    public GameObject fishLeftCount;
    public GameObject enemiesLeftCount;
    public GameObject timeLeftCount;

    void Awake () {
        //We only ever want 1 game manager
        if (instance == null ) {
            instance = this;
        }
        else if ( instance != this ) {
            Debug.Log( string.Format( "More then one copy of Gamemanager..i must end myself!" ) );
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start() {
        fishLeftCount.GetComponent<TMP_Text>().text = FishJobs.instance.fishcount.ToString();
        enemiesLeftCount.GetComponent<TMP_Text>().text = FishJobs.instance.enemyCount.ToString();
    }

    // Update is called once per frame
    void Update()
    {


    }


    /// <summary>
    /// Remove a fish from the counter
    /// </summary>
    public void RemoveFish() {
        var   txt   = fishLeftCount.GetComponent<TMP_Text>().text;
        Int32 count = Int32.Parse(txt);

        count                                       = count - 1;
        fishLeftCount.GetComponent<TMP_Text>().text = count.ToString();
        // Debug.Log(string.Format("Count is {0}", count));
    }

    /// <summary>
    /// Remove a fish from the counter
    /// </summary>
    public void RemoveEnemy() {
        var   txt   = enemiesLeftCount.GetComponent<TMP_Text>().text;
        Int32 count = Int32.Parse(txt);

        count                                       = count - 1;
        enemiesLeftCount.GetComponent<TMP_Text>().text = count.ToString();
        // Debug.Log(string.Format("Count is {0}", count));
    }

    public int GetFishCount() {
        var   txt   = fishLeftCount.GetComponent<TMP_Text>().text;
        Int32 count = Int32.Parse(txt);
        return count;
    }

    public int GetEnemyCount() {
        var   txt   = enemiesLeftCount.GetComponent<TMP_Text>().text;
        Int32 count = Int32.Parse(txt);
        return count;
    }


    public void SetTheTimer(float theTime) {
        if (timeLeftCount != null) {
            timeLeftCount.GetComponent<TMP_Text>().text = theTime.ToString("N2");
        }
    }


}
