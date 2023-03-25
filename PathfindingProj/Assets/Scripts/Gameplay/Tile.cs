using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IComparer<Tile>
{
    //  private field to mark whether the tile is occupied
    [SerializeField] private bool occupied;

    //  private field to mark whether or not the tile is an obstacle
    [SerializeField] private bool obstacle;
    //  public access methods for whether this tile is an obstacle
    public bool Obstacle { get { return obstacle; } set { obstacle = value; } }

    //  public access methods for whether this tile is occupied
    public bool Occupied { get { return occupied; } set { occupied = value; } }

    public Unit occupyingUnit;

    //  reference to the gridManager
    [SerializeField] private GridManager gridManager;

    //  reference to the gameObj this script is attached to
    public GameObject gameObj;

    //  reference to the renderer for the tile
    public SpriteRenderer rend;

    //  private fields to mark adjacent tiles
    [SerializeField] private Tile north;
    [SerializeField] private Tile east;
    [SerializeField] private Tile south;
    [SerializeField] private Tile west;

    //  public access methods for adjacent tiles
    public Tile North { get { return north; } set { north = value; } }
    public Tile East { get { return east; } set { east = value; } }
    public Tile South { get { return south; } set { south = value; } }
    public Tile West { get { return west; } set { west = value; } }

    //  Methods to check if the tile has a valid adjacent tile in each cardinal direction
    public bool hasNorth() { return north != null; }
    public bool hasEast() { return east != null; }
    public bool hasSouth() { return south != null; }
    public bool hasWest() { return west != null; }

    //  index location in map space 
    [SerializeField] private Vector2 mapPos;
    public Vector2 MapPos { get { return mapPos; } set { mapPos = value; } }

    //  the movement cost of this tile (g score)
    [SerializeField] private int moveScore = 1;
    //  used for test purposes (swapping tiles from ground to water)
    [SerializeField] public int defaultDistance = 1;

    //  public accesser for the moveScore
    public int MoveScore { get { return moveScore; } set { moveScore = value; } }
    //  public accesser for the default distance
    public int DefaultDistance { get { return defaultDistance; } }

    //  the current F-score of the tile (used for pathfinding)
    public int curScore = 0;

    //  the current H-score of the tile (used for pathfinding)
    public int hScore = 0;

    //  reference to the previous tile (used for pathfinding)
    public Tile prevTile = null;

    //  flag for if this tile has been visited by the pathfinding algorithm 
    [SerializeField] private bool visited;
    public bool Visited { get { return visited; } set { visited = value; } }

    //  internal flags for changing the highlight color of the tile
    //  can the unit reach this tile by movement?
    public bool moveRange = false;
    //  can the unit reach this tile by action?
    public bool actionRange = false;

    //  Both can be true, tiles will be BLUE if moveRange is true
    //  tiles will be RED if moveRange is false AND actionRange is true

    public Color moveRangeColor = Color.blue;
    public Color actionRangeColor = Color.red;
    public Color defaultColor = Color.white;

    //  should this tile be highlighted?
    public bool highlight = false;
    //  reference to the mouseover color (initialized in start)
    private Color mouseoverColor;
    //  reference to the color of the tile before being highlighted
    public Color prevColor;

    public bool focusedTile = false;



    void Start()
    {
        //  if the gridManager is not assigned, find it
        gridManager = GetComponentInParent<GridManager>();
        //  get access to the renderer
        rend = GetComponent<SpriteRenderer>();
        //  initialize the mouseover color
        ColorUtility.TryParseHtmlString("#F3ED8B", out mouseoverColor);
    }

    //  implementation of Compare for IComparer<Tile>
    public int Compare(Tile a, Tile b)
    {
        //  sorts based off of distanceVal
        return a.curScore.CompareTo(b.curScore);
    }

    //  on the mouse entering the collider of the tile:
    void OnMouseEnter()
    {
        //  cache the previous color
        prevColor = rend.color;
        //  change the mouseover color
        rend.color = mouseoverColor;

        if (this.occupied)
        {
            Unit highlightUnit = this.occupyingUnit;

            //  on mouse entering the collider of this unit:
            //  begin highlighted animation
            highlightUnit.animator.SetBool("highlighted", true);

            //  if no unit is selected, update/show the "player" frame on entering collider
            if (gridManager.activeUnit == null)
            {
                highlightUnit.uiController.unitFrame.SetActive(true);
                highlightUnit.playerFrame.UpdateUnitFrame(highlightUnit);
            }
            //  otherwise, update/show the "target" frame on entering collider
            else
            {
                highlightUnit.uiController.targetFrame.SetActive(true);
                highlightUnit.targetFrame.UpdateUnitFrame(highlightUnit);
            }

            gridManager.ShowAccessibleTiles(highlightUnit, out highlightUnit.accessibleTiles);
        }

    }

    //  on the mouse exiting the collider of the tile
    void OnMouseExit()
    {
        //  change the tile to the previous color
        rend.color = prevColor;

        if (this.occupied)
        {
            Unit highlightUnit = this.occupyingUnit;

            //  on mouse exiting the collider of this unit:
            //  stop the highlighted animation
            highlightUnit.animator.SetBool("highlighted", false);

            //  if no unit is selected, hide the unit frame on mouse leaving collider
            if (gridManager.activeUnit == null)
            {
                highlightUnit.uiController.unitFrame.SetActive(false);
                highlightUnit.playerFrame.target = null;
            }
            //  otherwise, hide the "target" frame on leaving collider
            else
            {
                highlightUnit.uiController.targetFrame.SetActive(false);
                highlightUnit.targetFrame.target = null;
            }

            gridManager.HideAccessibleTiles(highlightUnit, highlightUnit.accessibleTiles);

        }
    }

    //  while the mouse is over the collider for the tile:
    private void OnMouseOver()
    {
        //  if left mouse button is pressed:
        if (Input.GetMouseButtonDown(0))
        {
            //  is there an active unit?
            if (gridManager.activeUnit != null)
            {
                //  if so cache that unit
                Unit activeUnit = gridManager.activeUnit;

                //  if so: check if this tile is accessible to the unit
                if (activeUnit.ValidTile(mapPos))
                {
                    //  Logic for moving
                    //  can the unit move? is there a move wish?
                    if (!activeUnit.hasMoved && activeUnit.uiController.moveWish)
                    {
                        gridManager.ResetTiles();
                        Debug.Log("Path from: " + activeUnit.mapPosition + " to: " + CoordinateUtils.IsoWorldToDictionaryKey(this.transform.position));
                        //  since it's accessible initialize a path list
                        List<Vector2> path = new List<Vector2>();
                        //  calc the path from the active unit to this tile's position on the map
                        //  mark that the active unit has a path
                        activeUnit.hasPath = gridManager.CalculateAStarPath(activeUnit.mapPosition, mapPos, out path);

                        //  check if the active unit actually has a path (just in case)
                        if (activeUnit.hasPath)
                        {
                            //  copy assign
                            activeUnit.pathToMove = path;
                        }
                    }
                    //  otherwise they should act
                    else
                    {
                        //  check if the unit would like to attack
                        if (activeUnit.uiController.attackWish)
                        {
                            activeUnit.hasAction = true;
                            activeUnit.Attacking(gridManager.map[mapPos]);

                        }

                        //  check if the unit would like to use their skill
                        if (activeUnit.uiController.skillWish)
                        {
                            activeUnit.hasAction = true;
                            activeUnit.Skill(gridManager.map[mapPos]);

                        }
                    }
                }
            }

            else
            {
                if (this.occupied)
                {
                    Unit newActiveUnit = this.occupyingUnit;
                    //  check if this unit is on an ally
                    //  AND it is your turn
                    if (newActiveUnit.turnManager.playerTurn && newActiveUnit.ally)
                    {
                        //  check if the unit has not taken its actions
                        if (!newActiveUnit.waiting)
                        {
                        //  then we can go through the selection process!
                        //  swap whether it was selected


                        newActiveUnit.isSelected = !newActiveUnit.isSelected;
                        //  update the selected animation depending on isSelected being T/F
                        newActiveUnit.animator.SetBool("selected", newActiveUnit.isSelected);

                            //  if the unit is now selected
                            if (newActiveUnit.isSelected)
                            {
                                //  pass that this is the active unit to the grid manager
                                gridManager.activeUnit = newActiveUnit;
                                //  and the unit controller
                                newActiveUnit.partyManager.activeUnit = newActiveUnit;
                                //  and the ui controller
                                newActiveUnit.uiController.unitSelected = newActiveUnit.isSelected;

                                newActiveUnit.playerFrame.UpdateUnitFrame(newActiveUnit);
                            }
                        }
                    }
                }
            }
        }
    }
}
