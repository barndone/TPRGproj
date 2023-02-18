using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IComparer<Tile>
{
    //  private field to mark whether the tile is occupied
    [SerializeField] private bool occupied;

    //  private field to mark whether or not the tile is an obstacle
    [SerializeField] private bool obstacle;

    [SerializeField] private GridManager gridManager;

    public GameObject gameObj;

    //  public access methods for whether this tile is an obstacle
    public bool Obstacle { get { return obstacle; } set { obstacle = value; } }

    //  public access methods for whether this tile is occupied
    public bool Occupied { get { return occupied; } set { occupied = value; } }

    //  private fields to mark adjacent tiles
    [SerializeField] private Tile north;
    [SerializeField] private Tile east;
    [SerializeField] private Tile south;
    [SerializeField] private Tile west;

    //  Methods to check if the tile has a valid adjacent tile in each cardinal direction
    public bool hasNorth() { return north != null; }
    public bool hasEast() { return east != null; }
    public bool hasSouth() { return south != null; }
    public bool hasWest() { return west != null; }


    //  public access methods for adjacent tiles
    public Tile North { get { return north; } set { north = value; } }
    public Tile East { get { return east; } set { east = value; } }
    public Tile South { get { return south; } set { south = value; } }
    public Tile West { get { return west; } set { west = value; } }

    [SerializeField] private Vector2 mapPos;
    public Vector2 MapPos { get { return mapPos; } set { mapPos = value; } }

    //  reference to the renderer for the tile
    public SpriteRenderer rend;
    //  reference to the color of the highlighted sprite
    private Color highlightColor;

    [SerializeField] private int moveScore = 1;
    [SerializeField] public int defaultDistance = 1;

    public int curScore = 0;
    public Tile prevTile = null;

    public int MoveScore { get { return moveScore; } set { moveScore = value; } }
    public int DefaultDistance { get { return defaultDistance; } }

    [SerializeField] private bool visited;
    public bool Visited { get { return visited; } set { visited = value; } }

    void Start()
    {
        gridManager = GetComponentInParent<GridManager>();
        rend = GetComponent<SpriteRenderer>();
        ColorUtility.TryParseHtmlString("#F3ED8B", out highlightColor);
    }

    //  implementation of Compare for IComparer<Tile>
    public int Compare(Tile a, Tile b)
    {
        //  sorts based off of distanceVal
        return a.moveScore.CompareTo(b.moveScore);
    }

    void OnMouseEnter()
    {

        //rend.color = highlightColor;
    }

    void OnMouseExit()
    {
        //rend.color = Color.white;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //  is there an active unit?
            if (gridManager.activeUnit != null)
            {
                Unit activeUnit = gridManager.activeUnit;

                //  if so: check if this tile is accessible to the unit
                if (activeUnit.ValidTile(mapPos))
                {
                    Debug.Log("Path from: " + activeUnit.mapPosition + " to: " + CoordinateUtils.IsoWorldToDictionaryKey(this.transform.position));
                    //  since it's accessible initialize a path list
                    List<Vector2> path = new List<Vector2>();
                    //  calc the path from the active unit to this tile's position on the map
                    //  mark that the active unit has a path
                    activeUnit.hasPath = gridManager.CalculatePath(activeUnit.mapPosition, mapPos, out path);
                    //  check if the active unit actually has a path (just in case)
                    if (activeUnit.hasPath)
                    {
                        //  copy assign
                        activeUnit.pathToMove = path;
                    }

                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            FindObjectOfType<GridManager>().UpdateTile(this.transform.position);
        }
    }
}
