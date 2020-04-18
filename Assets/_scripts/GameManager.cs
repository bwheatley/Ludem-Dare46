using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public Vector2 Map_BL = new Vector2(-21, -12);
    public Vector2 Map_TL = new Vector2(-21, 10);
    public Vector2 Map_BR = new Vector2(21, -12);
    public Vector2 Map_TR = new Vector2(21, 10);



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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
