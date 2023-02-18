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

    [SerializeField] private static int WATER_MOVE_COST = 5;

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
                    newTile.DistanceVal = WATER_MOVE_COST;
                }

                newTileObj.transform.position = new Vector2(xPos, yPos);

                newTileObj.gameObject.name = "Grid Space: " + row.ToString() + ", " + col.ToString();

                newTile.gameObj = newTileObj;
                newTile.MapPos = new Vector2(row, col);

                map.Add(new Vector2(row, col), newTile);

                //  if the row is not the first row:
                if (row > 0)
                {
          
                    Tile prevTile;
                    map.TryGetValue(new Vector2(row - 1, col), out prevTile);
                    prevTile.East = newTile;
                    newTile.West = prevTile;
                }
                //  if this is not the first column:
                if (col > 0)
                {
                    Tile prevTile;
                    map.TryGetValue(new Vector2(row, col - 1), out prevTile);
                    prevTile.South = newTile;
                    newTile.North = prevTile;
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
                tile.DistanceVal = tile.DefaultDistance;
            }
            //  otherwise it is LAND
            else
            {
                //  make it.... NOT LAND
                rend.sprite = sprites[1];
                tile.Obstacle = true;
                tile.DistanceVal = WATER_MOVE_COST;
            }
        }
    }

    //  Method for showing which tiles on the grid a given unit is able to access
    public void ShowAccessibleTiles(Unit movingUnit, out List<Tile> accessibleTiles)
    {
        //  current tiles moved in relation to MaxMove
        //  list of accessible tiles (closed)
        accessibleTiles = new List<Tile>();
        //  queue for containing the valid tiles within range of this unit
        Queue<Tile> tilesToTraverse = new Queue<Tile>();

        //  push the current tile to the front of the queue to begin iteration
        tilesToTraverse.Enqueue(movingUnit.currentTile);

        do
        {
            //  if the front tile of the queue has a valid north connection
            if (tilesToTraverse.Peek().hasNorth())
            {
                //  check to see if the north connection has NOT been visited
                if (!tilesToTraverse.Peek().North.Visited)
                {
                    //  check to see if the north value is contained in the CLOSED list
                    //  AND check if the score of that tile plus the tile of the current score would be GREATER than max move
                    //  check if that tile is NOT occupied or an obstacle
                    if (!accessibleTiles.Contains(tilesToTraverse.Peek().North) && !tilesToTraverse.Contains(tilesToTraverse.Peek().North)
                        && (tilesToTraverse.Peek().North.DistanceVal + tilesToTraverse.Peek().curScore) <= movingUnit.MaxMove
                        && !tilesToTraverse.Peek().North.Occupied && !tilesToTraverse.Peek().North.Obstacle)
                    {
                        //  update the score of the tile
                        tilesToTraverse.Peek().North.curScore += tilesToTraverse.Peek().North.DistanceVal + tilesToTraverse.Peek().curScore;
                        //  if so, we can add it to the queue
                        tilesToTraverse.Enqueue(tilesToTraverse.Peek().North);
                    }
                }
            }
            //  if the front tile of the queue has a valid east connection
            if (tilesToTraverse.Peek().hasEast())
            {
                //  check to see if the east connection has NOT been visited
                if (!tilesToTraverse.Peek().East.Visited)
                {
                    //  check to see if the east value is contained in the CLOSED list
                    //  AND check if the score of that tile plus the tile of the current score would be GREATER than max move
                    //  check if that tile is NOT occupied or an obstacle
                    if (!accessibleTiles.Contains(tilesToTraverse.Peek().East) && !tilesToTraverse.Contains(tilesToTraverse.Peek().East)
                        && (tilesToTraverse.Peek().East.DistanceVal + tilesToTraverse.Peek().curScore) <= movingUnit.MaxMove
                        && !tilesToTraverse.Peek().East.Occupied && !tilesToTraverse.Peek().East.Obstacle)
                    {
                        //  update the score of the tile
                        tilesToTraverse.Peek().East.curScore += tilesToTraverse.Peek().East.DistanceVal + tilesToTraverse.Peek().curScore;
                        //  if so, we can add it to the queue
                        tilesToTraverse.Enqueue(tilesToTraverse.Peek().East);
                    }
                }
            }
            //  if the front tile of the queue has a valid south connection
            if (tilesToTraverse.Peek().hasSouth())
            {
                //  check to see if the south connection has NOT been visited
                if (!tilesToTraverse.Peek().South.Visited)
                {
                    //  check to see if the north value is contained in the CLOSED list
                    //  AND check if the score of that tile plus the tile of the current score would be GREATER than max move
                    //  check if that tile is NOT occupied or an obstacle
                    if (!accessibleTiles.Contains(tilesToTraverse.Peek().South) && !tilesToTraverse.Contains(tilesToTraverse.Peek().South)
                        && (tilesToTraverse.Peek().South.DistanceVal + tilesToTraverse.Peek().curScore) <= movingUnit.MaxMove
                        && !tilesToTraverse.Peek().South.Occupied && !tilesToTraverse.Peek().South.Obstacle)
                    {
                        //  update the score of the tile
                        tilesToTraverse.Peek().South.curScore += tilesToTraverse.Peek().South.DistanceVal + tilesToTraverse.Peek().curScore;
                        //  if so, we can add it to the queue
                        tilesToTraverse.Enqueue(tilesToTraverse.Peek().South);
                    }
                }
            }
            //  if the front tile of the queue has a valid west connection
            if (tilesToTraverse.Peek().hasWest())
            {
                //  check to see if the west connection has NOT been visited
                if (!tilesToTraverse.Peek().West.Visited)
                {
                    //  check to see if the north value is contained in the CLOSED list
                    //  AND check if the score of that tile plus the tile of the current score would be GREATER than max move
                    //  check if that tile is NOT occupied or an obstacle
                    if (!accessibleTiles.Contains(tilesToTraverse.Peek().West) && !tilesToTraverse.Contains(tilesToTraverse.Peek().West)
                        && (tilesToTraverse.Peek().West.DistanceVal + tilesToTraverse.Peek().curScore) <= movingUnit.MaxMove
                        && !tilesToTraverse.Peek().West.Occupied && !tilesToTraverse.Peek().West.Obstacle)
                    {
                        //  update the score of the tile
                        tilesToTraverse.Peek().West.curScore += tilesToTraverse.Peek().West.DistanceVal + tilesToTraverse.Peek().curScore;
                        //  if so, we can add it to the queue
                        tilesToTraverse.Enqueue(tilesToTraverse.Peek().West);
                    }
                }
            }
            //  assign the current tile as visited
            tilesToTraverse.Peek().Visited = true;
            //  pop from the front of the queue and add it to the output list
            accessibleTiles.Add(tilesToTraverse.Dequeue());
            //  break the loop if the open list is empty!
        } while (tilesToTraverse.Count != 0);

        //  mark all of the accessible tiles magenta to show movement range
        HighlightTiles(accessibleTiles, Color.magenta);
    }

    //  Method for hiding which tiles on the grid a unit is able to access
    public void HideAccessibleTiles(Unit movingUnit, List<Tile> accessibleTiles)
    {
        HighlightTiles(accessibleTiles, Color.white);
    }

    //  highlight the given list of Tiles
    public void HighlightTiles(List<Tile> tiles, Color color)
    {
        //  for each tile in the list tiles:
        foreach (Tile tile in tiles)
        {
            tile.rend.color = color;
            tile.Visited = false;
            tile.curScore = 0;
        }
    }
}
