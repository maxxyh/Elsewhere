using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PlayerUnit : Unit
{

    public Vector2 gridPosition = Vector2.zero;


    // Update is called once per frame
    private void Update()
    {     
        // simply update currentTile if not taking turn
        if (CurrState == UnitState.ENDTURN)
        {
            if (currentTile == null && map != null)
            {
                currentTile = map.GetCurrentTile(transform.position);
            }
            return;
        }
        
        if (CurrState == UnitState.IDLING)
        {
            currentTile.hasPlayer = true;
            //CheckMoveMouse();
        }
        
        else if (CurrState == UnitState.MOVING)
        {
            Move();
        }
    }

    void CheckMoveMouse()
    {   
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>();
                    if (t.selectable)
                    {
                        GetPathToTile(t);
                    }
                }

            }
        }
    }
    

    /*protected override void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        if (collision.tag == "crystal")
        { 
            StartCoroutine(SparkleAndDestroyCrystal(collision.gameObject));
            Debug.Log("Aft collected in PlayerUnit");
            if (OnCrystalCollected == null)
            {
                Debug.Log("Null OnCrystalCollected");
            }
            OnCrystalCollected();
        }
        #1#
        if (collision.tag == "door")
        {
            Debug.Log("Collide Door here");
            foreach(GameObject go in GameAssets.MyInstance.houseInterior)
            {
                go.SetActive(true);
            }
            collision.transform.root.gameObject.SetActive(false);
        }
    }*/
}