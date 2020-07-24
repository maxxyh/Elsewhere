using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity);
            if (hits != null)
            {
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.gameObject.CompareTag("tile"))
                    {
                        hit.collider.gameObject.GetComponent<Tile>().MouseDown();
                    }
                }
            }
        }
        
        RaycastHit2D[] mouseOverHits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity);
        if (mouseOverHits != null)
        {
            foreach (RaycastHit2D hit in mouseOverHits)
            {
                if (hit.collider.gameObject.CompareTag("highlightTile"))
                {
                    hit.collider.gameObject.GetComponent<HighlightTile>().MouseOver();
                }
            }
        }
        
    }
}