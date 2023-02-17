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

    //  public access methods for adjacent tiles
    public Tile North { get { return north; } set { north = value; } }
    public Tile East { get { return east; } set { east = value; } }
    public Tile South { get { return south; } set { south = value; } }
    public Tile West { get { return west; } set { west = value; } }

    private SpriteRenderer rend;

    private Color highlightColor;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        ColorUtility.TryParseHtmlString("#F3ED8B", out highlightColor);
    }

    void OnMouseEnter()
    {

        rend.color = highlightColor;

    }

    void OnMouseExit()
    {
        rend.color = Color.white;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Path from: " + FindObjectOfType<Unit>().isoPos + " to: " + CoordinateUtils.IsoWorldToDictionaryKey(this.transform.position));
        }
        if (Input.GetMouseButtonDown(1))
        {
            FindObjectOfType<GridManager>().UpdateTile(this.transform.position);
        }
    }
}
