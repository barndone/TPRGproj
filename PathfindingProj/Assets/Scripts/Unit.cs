using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] public Vector2 mapPosition;

    [SerializeField] GridManager gridManager;

    [SerializeField] private bool isSelected;

    public Tile currentTile;

    [SerializeField] private int maxMove;
    public int MaxMove { get { return maxMove; } set { maxMove = value; } }

    [SerializeField] private Animator animator;

    public List<Tile> accessibleTiles = new List<Tile>();

    private void Start()
    {
        mapPosition = CoordinateUtils.ConvertToIsometric(transform.position);
        currentTile = gridManager.map[mapPosition];
        currentTile.Occupied = true;
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (isSelected)
        {
            //  testing purposes: handle movement
            if (Input.GetKeyUp(KeyCode.A))
            {
                gridManager.map[mapPosition].Occupied = false;
                mapPosition.y -= 1;
                if (mapPosition.y < 0)
                {
                    mapPosition.y = 0;
                }
                //  if the tile is an obstacle:
                if (gridManager.map[mapPosition].Obstacle || gridManager.map[mapPosition].Occupied
                    || !accessibleTiles.Contains(gridManager.map[mapPosition]))
                {
                    //  reverse the change
                    mapPosition.y += 1;
                }
                gridManager.map[mapPosition].Occupied = true;
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                gridManager.map[mapPosition].Occupied = false;
                mapPosition.y += 1;
                if (mapPosition.y >= gridManager.Rows)
                {
                    mapPosition.y = gridManager.Rows - 1;
                }
                //  if the tile is an obstacle:
                if (gridManager.map[mapPosition].Obstacle || gridManager.map[mapPosition].Occupied
                    || !accessibleTiles.Contains(gridManager.map[mapPosition]))
                {
                    //  reverse the change
                    mapPosition.y -= 1;
                }
                gridManager.map[mapPosition].Occupied = true;
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                gridManager.map[mapPosition].Occupied = false;
                mapPosition.x -= 1;
                if (mapPosition.x < 0)
                {
                    mapPosition.x = 0;
                }
                //  if the tile is an obstacle:
                //  if the tile is occupied:
                //  OR if the tile is out of range
                if (gridManager.map[mapPosition].Obstacle || gridManager.map[mapPosition].Occupied
                    || !accessibleTiles.Contains(gridManager.map[mapPosition]))
                {
                    //  reverse the change
                    mapPosition.x += 1;
                }
                gridManager.map[mapPosition].Occupied = true;
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                gridManager.map[mapPosition].Occupied = false;
                mapPosition.x += 1;
                if (mapPosition.x >= gridManager.Columns)
                {
                    mapPosition.x = gridManager.Columns - 1;
                }
                //  if the tile is an obstacle:
                if (gridManager.map[mapPosition].Obstacle || gridManager.map[mapPosition].Occupied
                    || !accessibleTiles.Contains(gridManager.map[mapPosition]))
                {
                    //  reverse the change
                    mapPosition.x -= 1;
                }
                gridManager.map[mapPosition].Occupied = true;
            }
            transform.position = gridManager.map[mapPosition].gameObj.transform.position;
            currentTile = gridManager.map[mapPosition];
        }
    }

    private void OnMouseEnter()
    {
        animator.SetBool("highlighted", true);
    }

    private void OnMouseExit()
    {
        animator.SetBool("highlighted", false);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isSelected = !isSelected;
            animator.SetBool("selected", isSelected);
            //  if the unit is now selected
            if (isSelected)
            {
                //  show movement options
                gridManager.ShowAccessibleTiles(this, out accessibleTiles);
            }
            //  otherwise, they were unselected
            else
            {
                //  hide movement options
                gridManager.HideAccessibleTiles(this, accessibleTiles);
                accessibleTiles.Clear();
            }
        }
    }
}
