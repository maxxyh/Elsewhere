using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private bool _left;
    private bool _right;
    private void Update()
    {
        _left = false;
        _right = false;
        if (Input.GetMouseButtonDown(0))
            _left = true;
        if (Input.GetMouseButtonDown(1))
            _right = true;
        
        if (_left || _right)
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity);
            if (hits != null)
            {
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.gameObject.CompareTag("tile"))
                    {
                        Tile temp = hit.collider.gameObject.GetComponent<Tile>();
                        if (_left)
                            temp.MouseDown();
                        else if (_right)
                            temp.RightClick();
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