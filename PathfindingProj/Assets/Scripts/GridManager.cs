using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    [SerializeField] private int rows = 5; 
    [SerializeField] private int cols = 5;
    public int Rows { get { return rows; } }
    public int Columns { get { return cols; } }

    [SerializeField] private float tileSize = 1;

    //  Serialized array of prefabs
    [SerializeField] private GameObject[] gridTilePrefab;
    //  Serialized array of possible sprites
    [SerializeField] private Sprite[] sprites;
    
    /// <summary>
    /// Map representing the level:
    /// Will start from top left and end at bottom right
    /// </summary>
    public Dictionary<Vector2, Tile> map;

    private void GenerateGrid()
    {
        map = new Dictionary<Vector2, Tile>();

        Tile newTile;

        //  while row is less than rows:
        for (int row = 0; row < rows; row++)
        {
            //  while col is less than cols
            for (int col = 0; col < cols; col++)
            {
                var randomTile = gridTilePrefab[Random.Range(0, gridTilePrefab.Length)];
                GameObject newTileObj = Instantiate(randomTile, transform);

                newTile = newTileObj.GetComponent<Tile>();

                float xPos = (row * tileSize + col * tileSize) / 2.0f;
                float yPos = (row * tileSize - col * tileSize) / 4.0f;

                SpriteRenderer rend = newTileObj.GetComponent<SpriteRenderer>();

                //  sprites[1] will be the water sprite
                if (rend.sprite == sprites[1])
                {
                    //  if it is a water sprite, mark it as an obstacle
                    newTile.Obstacle = true;
                    yPos -= 0.2f;
                }

                newTileObj.transform.position = new Vector2(xPos, yPos);

                newTileObj.gameObject.name = "Grid Space: " + row.ToString() + ", " + col.ToString();

                newTile.gameObj = newTileObj;

                map.Add(new Vector2(row, col), newTile);

                //  if the row is not the first row:
                if (row > 0)
                {
          
                    Tile prevTile;
                    map.TryGetValue(new Vector2(row - 1, col), out prevTile);
                    prevTile.East = newTile;
                    newTile.West = newTile;
                }
                //  if this is not the first column:
                if (col > 0)
                {
                    Tile prevTile;
                    map.TryGetValue(new Vector2(row, col - 1), out prevTile);
                    prevTile.South = newTile;
                    newTile.North = newTile;
                }
            }
        }
    }

    void Start()
    {
        GenerateGrid();
    }

    public void UpdateTile (Vector2 pos)
    {
        Vector2 dictKey = CoordinateUtils.IsoWorldToDictionaryKey(pos);

        Tile tile = map[dictKey];

        SpriteRenderer rend = tile.gameObject.GetComponent<SpriteRenderer>();

        if (rend != null)
        {
            //  if it is an obstacle (water)
            if (tile.Obstacle)
            {
                //  convert it to LAND
                rend.sprite = sprites[0];
                tile.Obstacle = false;
            }
            //  otherwise it is LAND
            else
            {
                //  make it.... NOT LAND
                rend.sprite = sprites[1];
                tile.Obstacle = true;
            }
        }
    }

    public bool CalculatePath(Vector2 startPos, Vector2 destPos, out Queue<Vector2> path)
    {
        
    }
}
