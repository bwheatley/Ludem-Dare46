using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public LayerMask SharkBody;
    public Vector2 startPos;
    public Vector2 direction;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        //Left click!
        if (Input.GetMouseButtonDown(0) ) {
            Vector2 screenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ObjectCheck(screenPos);
        }
        // Track a single touch as a direction control.
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Handle finger movements based on TouchPhase
            switch (touch.phase)
            {
                //When a touch has first been detected, change the message and record the starting position
                case TouchPhase.Began:
                    // Record initial touch position.
                    startPos = touch.position;
                    Debug.Log(string.Format("Begun "));
                    Vector2 screenPos = Camera.main.ScreenToWorldPoint(touch.position);
                    ObjectCheck(screenPos);

                    break;

                //Determine if the touch is a moving touch
                case TouchPhase.Moved:
                    // Determine direction by comparing the current touch position with the initial one
                    direction = touch.position - startPos;
                    Debug.Log(string.Format( "Moving "));
                    break;

                case TouchPhase.Ended:
                    // Report that the touch has ended when it ends
                    Debug.Log("Ending");
                    break;
            }
        }

    }


    void ObjectCheck(Vector2 screenPos) {
        RaycastHit2D hit = Physics2D.Raycast( screenPos, Vector2.zero, Mathf.Infinity, SharkBody);

        if (hit.collider) {
            var _go = hit.collider.gameObject;
            // Debug.Log(string.Format("We hit something {0}", hit.collider.gameObject));
            // //Is it a sharky
            // if (_go.GetComponent<Shark>()) {

            string soundToPlay = "";
            //What type of enemy
            if (_go.transform.parent.GetComponent<Shark>().type == Shark.EnemyType.Shark) {
                soundToPlay = "shark_hit";
            }
            else if (_go.transform.parent.GetComponent<Shark>().type == Shark.EnemyType.Squid) {
                soundToPlay = "squid_hit";
            }
            GameManager.instance.PlayClip(soundToPlay);
            FishJobs.instance.DeleteEnemy(_go.transform.parent.gameObject);

            //Add time baby!
            GameManager.instance.AddTime();

            //Spawn new enemies


            Debug.Log("Hit: " + hit.collider.gameObject.transform.parent.gameObject.name);
            // }
        }


    }



}
