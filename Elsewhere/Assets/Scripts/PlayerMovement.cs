using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : TacticsMove
{
    //public float moveSpeed = 5f;

    public LayerMask whatStopsMovement;
    public Vector2 gridPosition = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!takingTurn)
        {
            return;
        }

        if (!moving)
        {
            FindSelectableTiles();
            CheckMouse();
        }
        // select tile during turn within movement range and move to that tile
        else
        {
            Move();
        }
    }

    void CheckMouse()
    {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>();

                    if (t.selectable)
                    {
                        GeneratePathToTile(t);
                    }
                }

                // Can extend to check if they clicked on player etc.
            }
        }
    }
}


/* Deprecated movement controls

public Transform movePoint;

Debug.Log("MOVE");
movePoint.parent = null;

transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
{
    if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
    {
        if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), 0.2f, whatStopsMovement))
        {
            movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
        }
    }

    else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
    {
        if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), 0.2f, whatStopsMovement))
        {
        movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
        }
    }
}
*/
