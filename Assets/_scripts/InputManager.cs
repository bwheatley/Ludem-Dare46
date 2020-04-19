using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public LayerMask SharkBody;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Left click!
        if (Input.GetMouseButtonDown(0)) {
            Vector2 screenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ObjectCheck(screenPos);
        }
    }


    void ObjectCheck(Vector2 screenPos) {
        RaycastHit2D hit = Physics2D.Raycast( screenPos, Vector2.zero, Mathf.Infinity, SharkBody);

        if (hit.collider) {
            var _go = hit.collider.gameObject;
            // Debug.Log(string.Format("We hit something {0}", hit.collider.gameObject));
            // //Is it a sharky
            // if (_go.GetComponent<Shark>()) {
            FishJobs.instance.DeleteEnemy(_go.transform.parent.gameObject);

            //Add time baby!
            GameManager.instance.AddTime();

            //Spawn new enemies


            Debug.Log("Hit: " + hit.collider.gameObject.transform.parent.gameObject.name);
            // }
        }


    }



}
