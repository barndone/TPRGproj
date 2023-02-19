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

    [SerializeField] private float timeToWait = 0.5f;
    private float timer;

    int pathIndex = 1;

    private void Start()
    {
        mapPosition = CoordinateUtils.ConvertToIsometric(transform.position);
        currentTile = gridManager.map[mapPosition];
        currentTile.Occupied = true;
        animator = GetComponentInChildren<Animator>();
        timer = timeToWait;
    }

    private void Update()
    {
        //  if this unit is selected
        if (isSelected)
        {
            //  check if it has a path
            if (hasPath)
            {
                //  if the wait time is less than or equal to 0
                if (timer <= 0.0f)
                {
                    timer = timeToWait;
                    //  if so, mark the current tile is unoccupied
                    gridManager.map[mapPosition].Occupied = false;

                    //  update the mapPosition to the difference between the current point along the math and the current map position
                    //  ex: mapPos = {0,0}; point = {1,0}; delta is {1,0}
                    mapPosition += (pathToMove[pathIndex] - mapPosition);

                    //  update the transform of the unit in world space to the world space position of the new tile
                    transform.position = gridManager.map[mapPosition].gameObj.transform.position;
                    //  update currentTile reference
                    currentTile = gridManager.map[mapPosition];

                    //  incrememnt pathIndex
                    pathIndex++;

                    //  if the pathIndex is greater than or equal to the number of entries in pathToMove:
                    if (pathIndex >= pathToMove.Count())
                    {
                        //  start cleanup
                        //  clear path to move
                        pathToMove.Clear();
                        //  reset pathIndex to 1
                        pathIndex = 1;
                        //  mark that there is no longer a path
                        hasPath = false;

                        //  toggle this unit to not be selected
                        isSelected = false;
                        //  update the animation to not be selected
                        animator.SetBool("selected", isSelected);

                        //  hide the accessible tiles of the unit
                        gridManager.HideAccessibleTiles(this, accessibleTiles);
                        //  mark that there is no longer an active unit
                        gridManager.activeUnit = null;
                        //  clear the accessible tiles of the unit
                        accessibleTiles.Clear();

                        //  iterate through each tile on the map and reset the flags from Dijkstra algo
                        foreach (Tile tile in gridManager.map.Values)
                        {
                            //  mark each tile as not visited
                            tile.Visited = false;
                            //  reset the current score
                            tile.curScore = 0;
                            //  remove the highlight
                            tile.highlight = false;
                        }
                    }

                    //  mark the current tile as occupied
                    gridManager.map[mapPosition].Occupied = true;
                }
                else
                {
                    timer -= Time.deltaTime;
                }
            }
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
