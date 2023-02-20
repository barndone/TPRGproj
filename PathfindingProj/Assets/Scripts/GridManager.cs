using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public Unit activeUnit = null;

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
                    newTile.MoveScore = WATER_MOVE_COST;
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
                tile.MoveScore = tile.DefaultDistance;
            }
            //  otherwise it is LAND
            else
            {
                //  make it.... NOT LAND
                rend.sprite = sprites[1];
                tile.Obstacle = true;
                tile.MoveScore = WATER_MOVE_COST;
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

        int range = 0;

        //  if the unit has NOT moved
        if (!movingUnit.hasMoved && movingUnit.uiController.moveWish)
        {
            range = movingUnit.MaxMove;
        }
        //  if they have moved then they must need to act
        else if (!movingUnit.hasActed && movingUnit.uiController.attackWish)
        {
            range = movingUnit.AttackRange;
        }
        else if (!movingUnit.hasActed && movingUnit.uiController.skillWish)
        {
            range = movingUnit.SkillRange;
        }

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
                        && (tilesToTraverse.Peek().North.MoveScore + tilesToTraverse.Peek().curScore) <= range
                        && !tilesToTraverse.Peek().North.Occupied && !tilesToTraverse.Peek().North.Obstacle)
                    {
                        //  update the score of the tile
                        tilesToTraverse.Peek().North.curScore += tilesToTraverse.Peek().North.MoveScore + tilesToTraverse.Peek().curScore;
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
                        && (tilesToTraverse.Peek().East.MoveScore + tilesToTraverse.Peek().curScore) <= range
                        && !tilesToTraverse.Peek().East.Occupied && !tilesToTraverse.Peek().East.Obstacle)
                    {
                        //  update the score of the tile
                        tilesToTraverse.Peek().East.curScore += tilesToTraverse.Peek().East.MoveScore + tilesToTraverse.Peek().curScore;
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
                        && (tilesToTraverse.Peek().South.MoveScore + tilesToTraverse.Peek().curScore) <= range
                        && !tilesToTraverse.Peek().South.Occupied && !tilesToTraverse.Peek().South.Obstacle)
                    {
                        //  update the score of the tile
                        tilesToTraverse.Peek().South.curScore += tilesToTraverse.Peek().South.MoveScore + tilesToTraverse.Peek().curScore;
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
                        && (tilesToTraverse.Peek().West.MoveScore + tilesToTraverse.Peek().curScore) <= range
                        && !tilesToTraverse.Peek().West.Occupied && !tilesToTraverse.Peek().West.Obstacle)
                    {
                        //  update the score of the tile
                        tilesToTraverse.Peek().West.curScore += tilesToTraverse.Peek().West.MoveScore + tilesToTraverse.Peek().curScore;
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

        //  if the unit has not moved:
        if (!movingUnit.hasMoved && movingUnit.uiController.moveWish)
        {
            //  mark all of the accessible tiles magenta to show movement range
            HighlightTiles(accessibleTiles, Color.magenta);
        }
        //  if they have not acted:
        else if (!movingUnit.hasActed && (movingUnit.uiController.attackWish || movingUnit.uiController.skillWish))
        {
            HighlightTiles(accessibleTiles, Color.red);
        }
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

    //  given a starting point, an endpoint, and a list to output as the path, traverse to the destination point
    public bool CalculatePath(Vector2 startPos, Vector2 destPos, out List<Vector2> path)
    {
        //  open list for nodes we are currently processing
        Queue<Tile> tilesToProcess = new Queue<Tile>();
        //  closed list for already processed nodes
        List<Vector2> processedTilePositions = new List<Vector2>();

        path = new List<Vector2>();

        //  push the tile at the startPos to the front of the open list
        tilesToProcess.Enqueue(map[startPos]);

        //  iterate from the start position until we find the destPos tile (visited == true)
        do
        {
            //  cachce the current tile as Current (help readability)
            Tile Current = tilesToProcess.Peek();
            //  check if the tile we are processing has a tile to the north of it
            if (Current.hasNorth())
            {
                //  cache the current North tile as North (help readability)
                Tile North = Current.North;
                //  check if the tile to the north of it has already been visited
                if (!North.Visited)
                {
                    //  check if that tile position is not in processedTilePositions
                    //  check if that tile is NOT Occupied or an Obstacle
                    if (!processedTilePositions.Contains(North.MapPos) 
                        && !North.Occupied
                        && !North.Obstacle)
                    {
                        //  check if the tile we are looking at exists in the tilesToProcess queue
                        if (tilesToProcess.Contains(North))
                        {
                            //  if it does, check if its score is greater than the current path:
                            if (North.curScore > North.MoveScore + Current.curScore)
                            {
                                //  if so, re-assign current score to the lower score
                                North.curScore = North.MoveScore + Current.curScore;
                                //  change the prevTile ref to this tile
                                North.prevTile = Current;
                            }
                            //  otherwise, do nothing
                        }
                        //  otherwise:
                        else
                        {
                            //  update its current score
                            North.curScore = Current.curScore + North.MoveScore;
                            //  assign its previous tile to this one
                            North.prevTile = Current;
                            //  push it to the queue
                            tilesToProcess.Enqueue(North);
                        }
                    }
                }
            }

            //  check if the tile we are processing has a tile to the east of it
            if (Current.hasEast())
            {
                //  cache the current east tile as east (help readability)
                Tile East = Current.East;
                //  check if the tile to the east of it has already been visited
                if (!East.Visited)
                {
                    //  check if that tile position is not in processedTilePositions
                    //  check if that tile is NOT Occupied or an Obstacle
                    if (!processedTilePositions.Contains(East.MapPos)
                        && !East.Occupied
                        && !East.Obstacle)
                    {
                        //  check if the tile we are looking at exists in the tilesToProcess queue
                        if (tilesToProcess.Contains(East))
                        {
                            //  if it does, check if its score is greater than the current path:
                            if (East.curScore > East.MoveScore + Current.curScore)
                            {
                                //  if so, re-assign current score to the lower score
                                East.curScore = East.MoveScore + Current.curScore;
                                //  change the prevTile ref to this tile
                                East.prevTile = Current;
                            }
                            //  otherwise, do nothing
                        }
                        //  otherwise:
                        else
                        {
                            //  update its current score
                            East.curScore = Current.curScore + East.MoveScore;
                            //  assign its previous tile to this one
                            East.prevTile = Current;
                            //  push it to the queue
                            tilesToProcess.Enqueue(East);
                        }
                    }
                }
            }

            //  check if the tile we are processing has a tile to the south of it
            if (Current.hasSouth())
            {
                //  cache the current south tile as south (help readability)
                Tile South = Current.South;
                //  check if the tile to the south of it has already been visited
                if (!South.Visited)
                {
                    //  check if that tile position is not in processedTilePositions
                    //  check if that tile is NOT Occupied or an Obstacle
                    if (!processedTilePositions.Contains(South.MapPos)
                        && !South.Occupied
                        && !South.Obstacle)
                    {
                        //  check if the tile we are looking at exists in the tilesToProcess queue
                        if (tilesToProcess.Contains(South))
                        {
                            //  if it does, check if its score is greater than the current path:
                            if (South.curScore > South.MoveScore + Current.curScore)
                            {
                                //  if so, re-assign current score to the lower score
                                South.curScore = South.MoveScore + Current.curScore;
                                //  change the prevTile ref to this tile
                                South.prevTile = Current;
                            }
                            //  otherwise, do nothing
                        }
                        //  otherwise:
                        else
                        {
                            //  update its current score
                            South.curScore = Current.curScore + South.MoveScore;
                            //  assign its previous tile to this one
                            South.prevTile = Current;
                            //  push it to the queue
                            tilesToProcess.Enqueue(South);
                        }
                    }
                }
            }

            //  check if the tile we are processing has a tile to the west of it
            if (Current.hasWest())
            {
                //  cache the current west tile as west (help readability)
                Tile West = Current.West;
                //  check if the tile to the west of it has already been visited
                if (!West.Visited)
                {
                    //  check if that tile position is not in processedTilePositions
                    //  check if that tile is NOT Occupied or an Obstacle
                    if (!processedTilePositions.Contains(West.MapPos)
                        && !West.Occupied
                        && !West.Obstacle)
                    {
                        //  check if the tile we are looking at exists in the tilesToProcess queue
                        if (tilesToProcess.Contains(West))
                        {
                            //  if it does, check if its score is greater than the current path:
                            if (West.curScore > West.MoveScore + Current.curScore)
                            {
                                //  if so, re-assign current score to the lower score
                                West.curScore = West.MoveScore + Current.curScore;
                                //  change the prevTile ref to this tile
                                West.prevTile = Current;
                            }
                            //  otherwise, do nothing
                        }
                        //  otherwise:
                        else
                        {
                            //  update its current score
                            West.curScore = Current.curScore + West.MoveScore;
                            //  assign its previous tile to this one
                            West.prevTile = Current;
                            //  push it to the queue
                            tilesToProcess.Enqueue(West);
                        }
                    }
                }
            }
            //  mark the current node as visited
            Current.Visited = true;

            //  Add the position of the current tile to the processedTilePositions list and pop it from the queue
            processedTilePositions.Add(tilesToProcess.Dequeue().MapPos);
            tilesToProcess = new Queue<Tile>(tilesToProcess.OrderBy(x => x.MoveScore));

        } while (!map[destPos].Visited || tilesToProcess.Count != 0);

        //  check if the destination was reached
        if (map[destPos].Visited)
        {
            //  if so, add the position to the path list
            path.Add(destPos);

            //  mark the current tile as the tile at the destPos
            Tile current = map[destPos];
            current.Visited = false;
            //  while the path list does NOT contain the startPos
            do
            {
                //  move current to the prevTile
                current = current.prevTile;
                //  add the position of the new current to the list
                path.Add(current.MapPos);
                current.Visited = false;

            } while (!path.Contains(startPos));

            //  reverse the list to get our path
            path.Reverse();

            //  return that we found a path
            return true;
        }
        //  otherwise the destination could not be reached
        else
        {
            //  return false with an empty list (for now)
            return false;
        }
    }

    public void ResetTiles()
    {
        //  iterate through each tile on the map and reset the flags from Dijkstra algo
        foreach (Tile tile in map.Values)
        {
            //  mark each tile as not visited
            tile.Visited = false;
            //  reset the current score
            tile.curScore = 0;
            //  remove the highlight
            tile.highlight = false;
            tile.prevTile = null;
            tile.rend.color = Color.white;
        }
    }
}
