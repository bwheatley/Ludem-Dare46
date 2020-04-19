using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficulty : MonoBehaviour {

    //0 - Easy 1 - Normal 2 - Hard 3 - Impossible
    public int theDifficulty;
    public List<GameObject> dontDestroyObjects;
    public static Difficulty instance;


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
        dontDestroyObjects = new List<GameObject>();
        dontDestroyObjects.Add(this.gameObject);
        DontDestroyObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DontDestroyObjects() {
        for (int i = 0; i < dontDestroyObjects.Count; i++) {
            var _myItem = dontDestroyObjects[i];
            DontDestroyOnLoad(_myItem);
        }
    }

    public int GetDifficulty() {
        return theDifficulty;
    }


    public void SetDifficulty(int _difficulty) {
        theDifficulty = _difficulty;

    }

}
