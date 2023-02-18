using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //  private field to mark whether the tile is occupied
    [SerializeField] private bool occupied;

    //  private field to mark whether or not the tile is an obstacle
    [SerializeField] private bool obstacle;

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

    [SerializeField] private int distanceVal = 1;
    [SerializeField] public int defaultDistance = 1;

    public int curScore = 0;

    public int DistanceVal { get { return distanceVal; } set { distanceVal = value; } }
    public int DefaultDistance { get { return defaultDistance; } }

    [SerializeField] private bool visited;
    public bool Visited { get { return visited; } set { visited = value; } }

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        ColorUtility.TryParseHtmlString("#F3ED8B", out highlightColor);
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
            Debug.Log("Path from: " + FindObjectOfType<Unit>().mapPosition + " to: " + CoordinateUtils.IsoWorldToDictionaryKey(this.transform.position));
        }
        if (Input.GetMouseButtonDown(1))
        {
            FindObjectOfType<GridManager>().UpdateTile(this.transform.position);
        }
    }
}
