using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public List<Vector2> pathToMove = new List<Vector2>();
    public bool hasPath = false;

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

            if (hasPath)
            {
                gridManager.map[mapPosition].Occupied = false;

                foreach (Vector2 point in pathToMove)
                {
                    StartCoroutine(MoveToTile(gridManager.map[point]));
                }

                //  clean up the data
                hasPath = false;
                mapPosition = pathToMove.Last();
                pathToMove.Clear();

                isSelected = false;
                animator.SetBool("selected", isSelected);
                gridManager.HideAccessibleTiles(this, accessibleTiles);
                gridManager.activeUnit = null;
                accessibleTiles.Clear();

                foreach (Tile tile in gridManager.map.Values)
                {
                    tile.Visited = false;
                    tile.curScore = 0;
                }

                gridManager.map[mapPosition].Occupied = true;
            }
            transform.position = gridManager.map[mapPosition].gameObj.transform.position;
            currentTile = gridManager.map[mapPosition];
        }
    }

    IEnumerator MoveToTile(Tile tile)
    {

        transform.position = Vector2.Lerp(transform.position, tile.gameObj.transform.position, Time.deltaTime);
        yield return null;

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
                //  if the grid doesn't have an active unit
                if ( gridManager.activeUnit == null)
                {
                    //  pass that this is the active unit to the grid manager
                    gridManager.activeUnit = this;
                    //  show movement options
                    gridManager.ShowAccessibleTiles(this, out accessibleTiles);
                }
                //  otherwise there is already an active unit
                else
                {
                    //  dont wanna select this
                    isSelected = false;
                }

            }
            //  otherwise, they were unselected
            else
            {
                //  hide movement options
                gridManager.HideAccessibleTiles(this, accessibleTiles);
                gridManager.activeUnit = null;
                accessibleTiles.Clear();
            }
        }
    }

    //  check if the passed tile position is an accessible tile
    public bool ValidTile(Vector2 tilePos)
    {
        //  return whether the accessible tiles list contains the tile pointed to by that position
        return accessibleTiles.Contains(gridManager.map[tilePos]);
    }
}
