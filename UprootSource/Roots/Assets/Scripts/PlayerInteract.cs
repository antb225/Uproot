using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerInteract : MonoBehaviour {



    public Root selectedRoot;
    public bool interacting;
    public LayerMask rootMask;

    Vector3 mousePos;

    void Update(){

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0)) {
            CheckInteraction();
        }
        if (Input.GetMouseButtonUp(0)) {
            interacting = false;
        }

        if (interacting) {
            MoveRoot();
        }
    }

    private void MoveRoot() {
        selectedRoot.GrowStem(mousePos);
    }

    private void CheckInteraction() {
        
        RaycastHit2D[] collection;
        collection = Physics2D.RaycastAll(mousePos, Vector2.zero, Mathf.Infinity, rootMask);

        if(collection.Length != 0) { //Find closest root clicked on.
            RaycastHit2D winner = collection[0];
            float winner_dist = -1;
            foreach (RaycastHit2D root in collection) {
                float dist = Vector3.Distance(mousePos, root.transform.position); 
                if (dist < winner_dist || winner_dist == -1) {
                    winner = root;
                    winner_dist = dist;
                }    
            }
            selectedRoot = winner.transform.GetComponent<Root>();
            interacting = true;

        }
    }
}
