using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    //  How many rows will the map be?
    [SerializeField] private int rows = 5;
    //  how many columns will the map be?
    [SerializeField] private int cols = 5;
    //  public access field for rows
    public int Rows { get { return rows; } }
    //  public access field for cols
    public int Columns { get { return cols; } }

    //  size of the tiles
    [SerializeField] private float tileSize = 1;

    //  Serialized array of prefabs
    [SerializeField] private GameObject[] gridTilePrefab;
    //  Serialized array of possible sprites
    [SerializeField] private Sprite[] sprites;
    
    //  map representing the level
    //  position of tile (0,0) maps to the tile NOT the gameObj
    public Dictionary<Vector2, Tile> map;

    //  const field for water move cost
    [SerializeField] const int WATER_MOVE_COST = 5;
    [SerializeField] List<Vector2> waterTilePos = new List<Vector2>();

    //  reference to the currently active unit
    public Unit activeUnit = null;

    [SerializeField] GameObject sceneCamera;

    //  method for generating the tile map
    private void GenerateGrid()
    {
        //  initialize new map
        map = new Dictionary<Vector2, Tile>();

        //  initialize a new tile
        Tile newTile;

        //  while row is less than rows:
        for (int row = 0; row < rows; row++)
        {
            //  while col is less than cols
            for (int col = 0; col < cols; col++)
            {
                GameObject newTileObj;
                //  create a random tile from the prefab list
                if (waterTilePos.Contains(new Vector2(row, col)))
                {
                    //  instantiate a new object of that tile at the transform of the parent (grid manager)
                    newTileObj = Instantiate(gridTilePrefab[1], transform);
                }
                else
                {   //  instantiate a new object of that tile at the transform of the parent (grid manager)
                    newTileObj = Instantiate(gridTilePrefab[0], transform);
                }
                


                //  get the Tile component
                newTile = newTileObj.GetComponent<Tile>();

                //  calculate the X offset from the gridManager for the isometric grid
                float xPos = (row * tileSize + col * tileSize) / 2.0f;
                //  calculate the Y offset from the gridManager for the isometric grid
                float yPos = (row * tileSize - col * tileSize) / 4.0f;

                //  access to the renderer of the tile
                SpriteRenderer rend = newTileObj.GetComponent<SpriteRenderer>();

                //  check wether the tile will be a water block or a regular block
                //  sprites[1] will be the water sprite
                if (rend.sprite == sprites[1])
                {
                    //  if it is a water sprite, mark it as an obstacle
                    newTile.Obstacle = true;
                    //  change its movescore to the const field for water cost
                    newTile.MoveScore = WATER_MOVE_COST;
                }

                //  transform the position of the new tile gameobj to the x and y offsets
                newTileObj.transform.position = new Vector2(xPos, yPos);

                //  rename the object for the inspector
                newTileObj.gameObject.name = "Grid Space: " + row.ToString() + ", " + col.ToString();

                //  assign the reference of the tile gameObj in the Tile class
                newTile.gameObj = newTileObj;
                //  assign the mapPos of the tile to the row, col
                newTile.MapPos = new Vector2(row, col);

                //  add this tile to the map
                map.Add(new Vector2(row, col), newTile);

                //  checks for applying connections
                //  if the row is not the first row:
                if (row > 0)
                {
                    //  initialize previous tile ref
                    Tile prevTile;
                    //  try to get the value of the tile a row before this tile
                    map.TryGetValue(new Vector2(row - 1, col), out prevTile);
                    //  assign the east ref in prevtile to this tile
                    prevTile.East = newTile;
                    //  assign the west ref in this tile to prevtile
                    newTile.West = prevTile;
                }
                //  if this is not the first column:
                if (col > 0)
                {
                    //  initialize previous tile ref
                    Tile prevTile;
                    //  try to get the value of the tile a column before this tile
                    map.TryGetValue(new Vector2(row, col - 1), out prevTile);
                    //  assign the south ref in prevTile to this tile
                    prevTile.South = newTile;
                    //  assign the north ref in this tile to prevTile
                    newTile.North = prevTile;
                }
            }
        }
    }

    //  on starting
    void Awake()
    {
        //  generate the grid!
        GenerateGrid();
        Vector3 xCameraOffset = new Vector3(map[new Vector2(rows - 1, cols - 1)].gameObj.transform.position.x / 2.0f ,0.0f,0.0f);
        sceneCamera.transform.position += xCameraOffset;
    }

    //  function to swap the tile between states (for testing purposes only)
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

    //  Method for showing which tiles on the grid a given unit is able to access INCLUDING attack range
    public void ShowAccessibleTiles(Unit movingUnit, out List<Tile> accessibleTiles)
    {
        //  current tiles moved in relation to MaxMove
        //  list of accessible tiles (closed)
        accessibleTiles = new List<Tile>();

        //  queue for containing the valid tiles within range of this unit
        Queue<Tile> tilesToTraverse = new Queue<Tile>();

        //  push the current tile to the front of the queue to begin iteration
        tilesToTraverse.Enqueue(movingUnit.currentTile);


        //  variable for tracking how far the unit can move, attack, or use their skill
        int range = 0;
        //  separate variable for move cost to account for differences between movement and attacking/skills
        int moveCost = 0;

        //  if the unit has not moved:
        if (!movingUnit.hasMoved)
        {
            //  add the movement range to the accessibleTiles range
            range += movingUnit.MaxMove;
        }

        //  if the unit has not acted
        if (!movingUnit.hasActed)
        {
            //  add the attack range to the accessibleTiles range
            range += movingUnit.AttackRange;
        }

        do
        {
            //  cache the current tile
            Tile cur = tilesToTraverse.Peek();
            //  if the front tile of the queue has a valid north connection
            if (cur.hasNorth())
            {
                //  cache the north tile
                Tile north = cur.North;
                //  check to see if the north connection has NOT been visited
                if (!north.Visited)
                {
                    //  if the current score is less than the moveRange of the unit
                    if (cur.curScore < movingUnit.MaxMove)
                    {
                        //  make the move cost the cost of the tile
                        moveCost = north.MoveScore;
                    }
                    //  otherwise, ignore the cost of the tile (default to one)
                    else if (cur.curScore >= movingUnit.MaxMove)
                    {
                        //  set the move cost to 1
                        moveCost = 1;
                    }

                    //  check to see if the north value is contained in the CLOSED list
                    //  AND check if the score of that tile plus the tile of the current score would be GREATER than max move
                    //  check if that tile is NOT an obstacle

                    if (!accessibleTiles.Contains(north)
                        && (moveCost + cur.curScore) <= range
                        && !north.Obstacle
                        && !north.Occupied)
                    {
                        //  check if the tile we are looking at is currently in the tilesToTraverse queue
                        if (tilesToTraverse.Contains(north))
                        {
                            //  if so, check if we should overwrite the score
                            if (north.curScore > moveCost + cur.curScore)
                            {
                                north.curScore = moveCost + cur.curScore;
                            }
                        }
                        else
                        {
                            //  update the score of the tile
                            north.curScore += moveCost + cur.curScore;

                            //  update flags of the tile
                            //  if the curScore of the checked tile is less than or equal to the units movement range
                            //  AND the tile is not occupied:
                            if (north.curScore <= movingUnit.MaxMove)
                            {
                                //  if the unit hasn't moved
                                if (!movingUnit.hasMoved && !north.Occupied)
                                {
                                    //  mark the tile as within movement range
                                    north.moveRange = true;
                                }

                                //  if the unit hasn't acted
                                if (!movingUnit.hasActed)
                                {
                                    //  since it is within movement range it is also within attack range
                                    north.actionRange = true;
                                }

                                //  check if the player wants to attack or use their skill
                                if (movingUnit.uiController.attackWish || movingUnit.uiController.skillWish)
                                {
                                    //  reset moveRange flag
                                    north.moveRange = false;
                                }
                            }

                            //  if the curScore of the checked tile is GREATER THAN the units movement range:
                            else if (north.curScore > movingUnit.MaxMove)
                            {
                                //  then it is only within action range
                                north.actionRange = true;
                            }

                            //  if so, we can add it to the queue
                            tilesToTraverse.Enqueue(north);
                        }
                    }
                    if (!accessibleTiles.Contains(north)
                        && (moveCost + cur.curScore) <= range
                        && !north.Obstacle
                        && north.Occupied)
                    {
                        north.moveRange = false;
                        north.actionRange = true;
                        accessibleTiles.Add(north);
                    }
                }
            }
            //  if the front tile of the queue has a valid east connection
            if (cur.hasEast())
            {
                Tile east = cur.East;
                //  check to see if the east connection has NOT been visited
                if (!east.Visited)
                {
                    //  if the current score is less than the moveRange of the unit
                    if (cur.curScore < movingUnit.MaxMove)
                    {
                        //  make the move cost the cost of the tile
                        moveCost = east.MoveScore;
                    }
                    //  otherwise, ignore the cost of the tile (default to one)
                    else if (cur.curScore >= movingUnit.MaxMove)
                    {
                        //  set the move cost to 1
                        moveCost = 1;
                    }

                    //  check to see if the north value is contained in the CLOSED list
                    //  AND check if the score of that tile plus the tile of the current score would be GREATER than max move
                    //  check if that tile is NOT an obstacle

                    if (!accessibleTiles.Contains(east)
                        && (moveCost + cur.curScore) <= range
                        && !east.Obstacle
                        && !east.Occupied)
                    {
                        //  check if the tile we are looking at is currently in the tilesToTraverse queue
                        if (tilesToTraverse.Contains(east))
                        {
                            //  if so, check if we should overwrite the score
                            if (east.curScore > moveCost + cur.curScore)
                            {
                                east.curScore = moveCost + cur.curScore;
                            }
                        }
                        else
                        {
                            //  update the score of the tile
                            east.curScore += moveCost + cur.curScore;

                            //  update flags of the tile
                            //  if the curScore of the checked tile is less than or equal to the units movement range
                            //  AND the tile is not occupied:
                            if (east.curScore <= movingUnit.MaxMove)
                            {
                                //  if the unit hasn't moved
                                if (!movingUnit.hasMoved && !east.Occupied)
                                {
                                    //  mark the tile as within movement range
                                    east.moveRange = true;
                                }

                                //  if the unit hasn't acted
                                if (!movingUnit.hasActed)
                                {
                                    //  since it is within movement range it is also within attack range
                                    east.actionRange = true;
                                }
                            }

                            //  if the curScore of the checked tile is GREATER THAN the units movement range:
                            else if (east.curScore > movingUnit.MaxMove)
                            {
                                //  then it is only within action range
                                east.actionRange = true;
                            }

                            //  check if the player wants to attack or use their skill
                            if (movingUnit.uiController.attackWish || movingUnit.uiController.skillWish)
                            {
                                //  reset moveRange flag
                                east.moveRange = false;
                            }

                            //  if so, we can add it to the queue
                            tilesToTraverse.Enqueue(east);
                        }
                    }
                    if (!accessibleTiles.Contains(east)
                        && (moveCost + cur.curScore) <= range
                        && !east.Obstacle
                        && east.Occupied)
                    {
                        east.moveRange = false;
                        east.actionRange = true;
                        accessibleTiles.Add(east);
                    }
                }
            }
            //  if the front tile of the queue has a valid south connection
            if (cur.hasSouth())
            {
                Tile south = cur.South;
                //  check to see if the south connection has NOT been visited
                if (!south.Visited)
                {
                    //  if the current score is less than the moveRange of the unit
                    if (cur.curScore < movingUnit.MaxMove)
                    {
                        //  make the move cost the cost of the tile
                        moveCost = south.MoveScore;
                    }
                    //  otherwise, ignore the cost of the tile (default to one)
                    else if (cur.curScore >= movingUnit.MaxMove)
                    {
                        //  set the move cost to 1
                        moveCost = 1;
                    }

                    //  check to see if the north value is contained in the CLOSED list
                    //  AND check if the score of that tile plus the tile of the current score would be GREATER than max move
                    //  check if that tile is NOT an obstacle

                    if (!accessibleTiles.Contains(south)
                        && (moveCost + cur.curScore) <= range
                        && !south.Obstacle
                        && !south.Occupied)
                    {
                        //  check if the tile we are looking at is currently in the tilesToTraverse queue
                        if (tilesToTraverse.Contains(south))
                        {
                            //  if so, check if we should overwrite the score
                            if (south.curScore > moveCost + cur.curScore)
                            {
                                south.curScore = moveCost + cur.curScore;
                            }
                        }
                        else
                        {
                            //  update the score of the tile
                            south.curScore += moveCost + cur.curScore;

                            //  update flags of the tile
                            //  if the curScore of the checked tile is less than or equal to the units movement range
                            //  AND the tile is not occupied:
                            if (south.curScore <= movingUnit.MaxMove)
                            {
                                //  if the unit hasn't moved
                                if (!movingUnit.hasMoved && !south.Occupied)
                                {
                                    //  mark the tile as within movement range
                                    south.moveRange = true;
                                }

                                //  if the unit hasn't acted
                                if (!movingUnit.hasActed)
                                {
                                    //  since it is within movement range it is also within attack range
                                    south.actionRange = true;
                                }
                            }

                            //  if the curScore of the checked tile is GREATER THAN the units movement range:
                            else if (south.curScore > movingUnit.MaxMove)
                            {
                                //  then it is only within action range
                                south.actionRange = true;
                            }

                            //  check if the player wants to attack or use their skill
                            if (movingUnit.uiController.attackWish || movingUnit.uiController.skillWish)
                            {
                                //  reset moveRange flag
                                south.moveRange = false;
                            }

                            //  if so, we can add it to the queue
                            tilesToTraverse.Enqueue(south);
                        }
                    }
                    if (!accessibleTiles.Contains(south)
                        && (moveCost + cur.curScore) <= range
                        && !south.Obstacle
                        && south.Occupied)
                    {
                        south.moveRange = false;
                        south.actionRange = true;
                        accessibleTiles.Add(south);
                    }
                }
            }
            //  if the front tile of the queue has a valid west connection
            if (cur.hasWest())
            {
                Tile west = cur.West;
                //  check to see if the west connection has NOT been visited
                if (!west.Visited)
                {
                    //  if the current score is less than the moveRange of the unit
                    if (cur.curScore < movingUnit.MaxMove)
                    {
                        //  make the move cost the cost of the tile
                        moveCost = west.MoveScore;
                    }
                    //  otherwise, ignore the cost of the tile (default to one)
                    else if (cur.curScore >= movingUnit.MaxMove)
                    {
                        //  set the move cost to 1
                        moveCost = 1;
                    }

                    //  check to see if the north value is contained in the CLOSED list
                    //  AND check if the score of that tile plus the tile of the current score would be GREATER than max move
                    //  check if that tile is NOT an obstacle

                    if (!accessibleTiles.Contains(west)
                        && (moveCost + cur.curScore) <= range
                        && !west.Obstacle
                        && !west.Occupied)
                    {
                        //  check if the tile we are looking at is currently in the tilesToTraverse queue
                        if (tilesToTraverse.Contains(west))
                        {
                            //  if so, check if we should overwrite the score
                            if (west.curScore > moveCost + cur.curScore)
                            {
                                west.curScore = moveCost + cur.curScore;
                            }
                        }
                        else
                        {
                            //  update the score of the tile
                            west.curScore += moveCost + cur.curScore;

                            //  update flags of the tile
                            //  if the curScore of the checked tile is less than or equal to the units movement range
                            //  AND the tile is not occupied:
                            if (west.curScore <= movingUnit.MaxMove)
                            {
                                //  if the unit hasn't moved
                                if (!movingUnit.hasMoved)
                                {
                                    //  mark the tile as within movement range
                                    west.moveRange = true;
                                }

                                //  if the unit hasn't acted
                                if (!movingUnit.hasActed)
                                {
                                    //  since it is within movement range it is also within attack range
                                    west.actionRange = true;
                                }
                            }

                            //  if the curScore of the checked tile is GREATER THAN the units movement range:
                            else if (west.curScore > movingUnit.MaxMove)
                            {
                                //  then it is only within action range
                                west.actionRange = true;
                            }

                            //  check if the player wants to attack or use their skill
                            if (movingUnit.uiController.attackWish || movingUnit.uiController.skillWish)
                            {
                                //  reset moveRange flag
                                west.moveRange = false;
                            }

                            //  if so, we can add it to the queue
                            tilesToTraverse.Enqueue(west);
                        }
                    }
                    if (!accessibleTiles.Contains(west)
                        && (moveCost + cur.curScore) <= range
                        && !west.Obstacle
                        && west.Occupied)
                    {
                        west.moveRange = false;
                        west.actionRange = true;
                        accessibleTiles.Add(west);
                    }
                }
            }

            //  assign the current tile as visited
            cur.Visited = true;

            //  if the current tile is occupied
            if (cur.Occupied)
            {
                //  if it is occupied by an ally
                if (cur.occupyingUnit.ally == movingUnit.ally)
                {
                    //  it is not an accessible tile, so don't add it to the list
                    //  just remove from the queue
                    tilesToTraverse.Dequeue();
                }
                else
                {
                    //  pop from the front of the queue and add it to the output list
                    accessibleTiles.Add(tilesToTraverse.Dequeue());
                }
            }
            else
            {
                //  pop from the front of the queue and add it to the output list
                accessibleTiles.Add(tilesToTraverse.Dequeue());
            }
            //  break the loop if the open list is empty!
        } while (tilesToTraverse.Count != 0);


        //  after gathering the accessible tile:
        //  iterate through each accessible tile
        foreach (Tile tile in accessibleTiles)
        {
            tile.Visited = false;
            tile.curScore = 0;

            //  if that tile lies within the moveRange of the unit:
            if (tile.moveRange)
            {
                //  change the color of the tile to the moveRangeColor (default to blue)
                tile.rend.color = tile.moveRangeColor;
            }
            //  otherwise, if moveRange is false and this tile lies within action range:
            else if (!tile.moveRange && tile.actionRange)
            {
                //  change the color of the tile to the actionRangeColor (default to red)
                tile.rend.color = tile.actionRangeColor;
            }
        }
    }


    //  Method for showing which tiles on the grid a given unit is able to attack!
    //  ignores if the tile is occupied
    public void ShowAttackableTiles(Unit attackingUnit, out List<Tile> accessibleTiles)
    {
        //  list of accessible tiles (closed)
        accessibleTiles = new List<Tile>();

        //  queue for containing the valid tiles within range of this unit
        Queue<Tile> tilesToTraverse = new Queue<Tile>();

        //  push the current tile to the front of the queue to begin iteration
        tilesToTraverse.Enqueue(attackingUnit.currentTile);


        //  variable for tracking how far the unit can move, attack, or use their skill
        int range = 0;

        //  traversal cost of the tiles for attacking (will always be one)
        int traversalCost = 1;

        //  if the unit has not acted
        if (!attackingUnit.hasActed)
        {
            if (attackingUnit.uiController.attackWish)
            {
                //  add the attack range to the accessibleTiles range
                range += attackingUnit.AttackRange;
            }

            else if (attackingUnit.uiController.skillWish)
            {
                range += attackingUnit.SkillRange;
            }
        }

        //  otherwise, we should exit
        else
        {
            return;
        }

        do
        {
            //  cache the current tile
            Tile cur = tilesToTraverse.Peek();
            //  if the front tile of the queue has a valid north connection
            if (cur.hasNorth())
            {
                //  cache the north tile
                Tile north = cur.North;
                //  check to see if the north connection has NOT been visited
                if (!north.Visited)
                {
                    //  check to see if the north value is contained in the CLOSED list
                    //  AND check if the score of that tile plus the tile of the current score would be GREATER than max move
                    //  check if that tile is NOT an obstacle

                    if (!accessibleTiles.Contains(north)
                        && (traversalCost + cur.curScore) <= range
                        && !north.Obstacle)
                    {
                        //  check if the tile we are looking at is currently in the tilesToTraverse queue
                        if (tilesToTraverse.Contains(north))
                        {
                            //  if so, check if we should overwrite the score
                            if (north.curScore > traversalCost + cur.curScore)
                            {
                                north.curScore = traversalCost + cur.curScore;
                            }
                        }
                        else
                        {
                            //  update the score of the tile
                            north.curScore += traversalCost + cur.curScore;



                            //  if so, we can add it to the queue
                            tilesToTraverse.Enqueue(north);
                        }
                    }
                }
            }
            //  if the front tile of the queue has a valid east connection
            if (cur.hasEast())
            {
                Tile east = cur.East;
                if (!east.Visited)
                {
                    //  check to see if the north value is contained in the CLOSED list
                    //  AND check if the score of that tile plus the tile of the current score would be GREATER than max move
                    //  check if that tile is NOT an obstacle

                    if (!accessibleTiles.Contains(east)
                        && (traversalCost + cur.curScore) <= range
                        && !east.Obstacle)
                    {
                        //  check if the tile we are looking at is currently in the tilesToTraverse queue
                        if (tilesToTraverse.Contains(east))
                        {
                            //  if so, check if we should overwrite the score
                            if (east.curScore > traversalCost + cur.curScore)
                            {
                                east.curScore = traversalCost + cur.curScore;
                            }
                        }
                        else
                        {
                            //  update the score of the tile
                            east.curScore += traversalCost + cur.curScore;



                            //  if so, we can add it to the queue
                            tilesToTraverse.Enqueue(east);
                        }
                    }
                }
            }
            //  if the front tile of the queue has a valid south connection
            if (cur.hasSouth())
            {
                Tile south = cur.South;
                if (!south.Visited)
                {
                    //  check to see if the north value is contained in the CLOSED list
                    //  AND check if the score of that tile plus the tile of the current score would be GREATER than max move
                    //  check if that tile is NOT an obstacle

                    if (!accessibleTiles.Contains(south)
                        && (traversalCost + cur.curScore) <= range
                        && !south.Obstacle)
                    {
                        //  check if the tile we are looking at is currently in the tilesToTraverse queue
                        if (tilesToTraverse.Contains(south))
                        {
                            //  if so, check if we should overwrite the score
                            if (south.curScore > traversalCost + cur.curScore)
                            {
                                south.curScore = traversalCost + cur.curScore;
                            }
                        }
                        else
                        {
                            //  update the score of the tile
                            south.curScore += traversalCost + cur.curScore;



                            //  if so, we can add it to the queue
                            tilesToTraverse.Enqueue(south);
                        }
                    }
                }
            }
            //  if the front tile of the queue has a valid west connection
            if (cur.hasWest())
            {
                Tile west = cur.West;
                if (!west.Visited)
                {
                    //  check to see if the north value is contained in the CLOSED list
                    //  AND check if the score of that tile plus the tile of the current score would be GREATER than max move
                    //  check if that tile is NOT an obstacle

                    if (!accessibleTiles.Contains(west)
                        && (traversalCost + cur.curScore) <= range
                        && !west.Obstacle)
                    {
                        //  check if the tile we are looking at is currently in the tilesToTraverse queue
                        if (tilesToTraverse.Contains(west))
                        {
                            //  if so, check if we should overwrite the score
                            if (west.curScore > traversalCost + cur.curScore)
                            {
                                west.curScore = traversalCost + cur.curScore;
                            }
                        }
                        else
                        {
                            //  update the score of the tile
                            west.curScore += traversalCost + cur.curScore;



                            //  if so, we can add it to the queue
                            tilesToTraverse.Enqueue(west);
                        }
                    }
                }
            }

            //  assign the current tile as visited
            cur.Visited = true;
            //  pop from the front of the queue and add it to the output list
            accessibleTiles.Add(tilesToTraverse.Dequeue());
            //  break the loop if the open list is empty!
        } while (tilesToTraverse.Count != 0);

        //  if it contains the currentTile
        if (accessibleTiles.Contains(attackingUnit.currentTile))
        {
            //  remove the tile occupied by this unit (since it can't attack itself!)
            accessibleTiles.Remove(attackingUnit.currentTile);

            //  if the unit cannot attack adjacent tiles (IE is a ranger)
            if (!attackingUnit.canAttackAdjacent)
            {
                //  for each cardinal direction, check if it has a tile in that direction
                if (attackingUnit.currentTile.hasNorth())
                {
                    accessibleTiles.Remove(attackingUnit.currentTile.North);
                }
                if (attackingUnit.currentTile.hasEast())
                {
                    accessibleTiles.Remove(attackingUnit.currentTile.East);
                }
                if (attackingUnit.currentTile.hasSouth())
                {
                    accessibleTiles.Remove(attackingUnit.currentTile.South);
                }
                if (attackingUnit.currentTile.hasWest())
                {
                    accessibleTiles.Remove(attackingUnit.currentTile.West);
                }
            }
        }

        //  after gathering the accessible tiles:
        //  iterate through each accessible tile to update the color of the tiles
        foreach (Tile tile in accessibleTiles)
        {
            tile.Visited = false;
            tile.curScore = 0;

            //  change the color of the tile to the actionRangeColor (default to red)
            if (activeUnit.unitID == 1 && activeUnit.uiController.skillWish)
            {
                tile.rend.color = Color.green;
            }
            else
            {
                tile.rend.color = tile.actionRangeColor;

            }
        }
    }

    //  Method for hiding which tiles on the grid a unit is able to access
    public void HideAccessibleTiles(Unit movingUnit, List<Tile> accessibleTiles)
    {
        //  for each tile in the list tiles:
        foreach (Tile tile in accessibleTiles)
        {
            if (movingUnit.uiController.showDangerZone)
            {
                if (!movingUnit.ally)
                {
                    tile.rend.color = Color.magenta;
                }

                else
                {
                    tile.rend.color = tile.defaultColor;

                    CalculateDangerZone(movingUnit.turnManager.cpu.party);
                }
            }
            else
            {
                tile.rend.color = tile.defaultColor;
            }
            tile.Visited = false;
            tile.curScore = 0;
            tile.moveRange = false;
            tile.actionRange = false;
        }
    }


    //  given a starting point, an endpoint, and a list to output as the path, traverse to the destination point
    //  using Dijkstra's
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

    //  given a starting point, an end point, and an output list for the path- Utilize the Manhattan Heuristic to calculate a path
    //  with A*
    //  h = difference in X positions + difference in Y positions
    //  g = move cost
    //  f = g + h
    public bool CalculateAStarPath(Vector2 startPos, Vector2 destPos, out List<Vector2> path)
    {
        if(activeUnit == null)
        {
            Debug.LogError("Can't find a path for nothing!", this);
            path = new List<Vector2>();
            return false;
        }

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
            if (tilesToProcess.Count == 0)
            {
                return false;
            }

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
                    if (!processedTilePositions.Contains(North.MapPos))
                    {
                        //  calculate the hScore of the North tile
                        North.hScore = CoordinateUtils.CalcManhattanDistance(North.MapPos, destPos);
                        //  calculate the fScore (curScore) => hScore + gScore (MoveScore)
                        North.curScore = North.hScore + North.MoveScore;

                        //  check if that tile is not occupied or an obstacle
                        if (!North.Obstacle)
                        {
                            if (North.Occupied)
                            {
                                if (North.occupyingUnit == activeUnit.target)
                                {
                                    North.prevTile = Current;
                                    //  if not, add to the tiles to process
                                    tilesToProcess.Enqueue(North);
                                }
                            }
                            else
                            {
                                North.prevTile = Current;
                                //  if not, add to the tiles to process
                                tilesToProcess.Enqueue(North);
                            }
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
                    if (!processedTilePositions.Contains(East.MapPos))
                    {
                        //  calculate the hScore of the East tile
                        East.hScore = CoordinateUtils.CalcManhattanDistance(East.MapPos, destPos);
                        //  calculate the fScore (curScore) => hScore + gScore (MoveScore)
                        East.curScore = East.hScore + East.MoveScore;

                        //  check if that tile is not occupied or an obstacle
                        if (!East.Obstacle)
                        {
                            if (East.Occupied)
                            {
                                if (East.occupyingUnit == activeUnit.target)
                                {
                                    East.prevTile = Current;
                                    //  if not, add to the tiles to process
                                    tilesToProcess.Enqueue(East);
                                }
                            }
                            else
                            {
                                East.prevTile = Current;
                                //  if not, add to the tiles to process
                                tilesToProcess.Enqueue(East);
                            }
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
                    if (!processedTilePositions.Contains(South.MapPos))
                    {
                        //  calculate the hScore of the South tile
                        South.hScore = CoordinateUtils.CalcManhattanDistance(South.MapPos, destPos);
                        //  calculate the fScore (curScore) => hScore + gScore (MoveScore)
                        South.curScore = South.hScore + South.MoveScore;

                        //  check if that tile is not occupied or an obstacle
                        if (!South.Obstacle)
                        {
                            if (South.Occupied)
                            {
                                Debug.Assert(activeUnit != null, "Active unit was null! Bad!", this);

                                if (South.occupyingUnit == activeUnit.target)
                                {
                                    South.prevTile = Current;
                                    //  if not, add to the tiles to process
                                    tilesToProcess.Enqueue(South);
                                }
                            }
                            else
                            {
                                South.prevTile = Current;
                                //  if not, add to the tiles to process
                                tilesToProcess.Enqueue(South);
                            }
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
                    if (!processedTilePositions.Contains(West.MapPos))
                    {
                        //  calculate the hScore of the South tile
                        West.hScore = CoordinateUtils.CalcManhattanDistance(West.MapPos, destPos);
                        //  calculate the fScore (curScore) => hScore + gScore (MoveScore)
                        West.curScore = West.hScore + West.MoveScore;

                        //  check if that tile is not occupied or an obstacle
                        if (!West.Obstacle)
                        {
                            if (West.Occupied)
                            {
                                if (West.occupyingUnit == activeUnit.target)
                                {
                                    West.prevTile = Current;
                                    //  if not, add to the tiles to process
                                    tilesToProcess.Enqueue(West);
                                }
                            }
                            else
                            {
                                West.prevTile = Current;
                                //  if not, add to the tiles to process
                                tilesToProcess.Enqueue(West);
                            }
                        }
                    }
                }
            }
            //  mark the current node as visited
            Current.Visited = true;

            //  Add the position of the current tile to the processedTilePositions list and pop it from the queue
            processedTilePositions.Add(tilesToProcess.Dequeue().MapPos);

            //  sort the queue according to our F-score
            tilesToProcess = new Queue<Tile>(tilesToProcess.OrderBy(x => x.curScore));

            if(processedTilePositions.Count > rows * cols)
            {
                Debug.LogError("Oh no, we are somehow processing more tiles than tiles that exist, let's exit...");
                //  mark that a path could not be found
                return false;
            }

        } while (!map[destPos].Visited);

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
            //  reset flags for move/action range
            tile.moveRange = false;
            tile.actionRange = false;

            //  mark each tile as not visited
            tile.Visited = false;
            //  reset the current score
            tile.curScore = 0;

            tile.hScore = 0;

            tile.focusedTile = false;

            //  remove the highlight
            tile.highlight = false;
            //  set previous tile to null
            tile.prevTile = null;
            //  reset the color to white
            tile.rend.color = tile.defaultColor;
            tile.prevColor = tile.defaultColor;
        }
    }

    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (activeUnit != null)
            {
                activeUnit.isSelected = false;
                //  update the selected animation depending on isSelected being T/F
                activeUnit.animator.SetBool("selected", activeUnit.isSelected);

                //  hide movement options
                HideAccessibleTiles(activeUnit, activeUnit.accessibleTiles);
                //  clear the accessible tiles list
                activeUnit.accessibleTiles.Clear();
                activeUnit.uiController.unitSelected = false;
                //  mark that no unit is active
                activeUnit = null;
                //  and the unit controller
                activeUnit = null;

            }
            //  otherwise do nothing
        }
    }

    public void CalculateDangerZone(List<Unit> enemyTeam)
    {
        //  queue for containing the valid tiles within range of this unit
        Queue<Tile> tilesToTraverse = new Queue<Tile>();

        foreach (Unit unit in enemyTeam)
        {
            if (!unit.IsDead)
            {
                //  push the current tile to the front of the queue to begin iteration
                tilesToTraverse.Enqueue(unit.currentTile);


                //  variable for tracking how far the unit can move, attack, or use their skill
                int range = 0;

                range += unit.MaxMove + unit.AttackRange;

                //  traversal cost of the tiles for attacking (will always be one)
                int traversalCost = 1;

                do
                {
                    //  cache the current tile
                    Tile cur = tilesToTraverse.Peek();
                    //  if the front tile of the queue has a valid north connection
                    if (cur.hasNorth())
                    {
                        //  cache the north tile
                        Tile north = cur.North;
                        //  check to see if the north connection has NOT been visited
                        if (!north.Visited)
                        {
                            //  check to see if the north value is contained in the CLOSED list
                            //  AND check if the score of that tile plus the tile of the current score would be GREATER than max move
                            //  check if that tile is NOT an obstacle
                            if (!unit.accessibleTiles.Contains(north)
                                && (traversalCost + cur.curScore) <= range
                                && !north.Obstacle)
                            {
                                //  check if the tile we are looking at is currently in the tilesToTraverse queue
                                if (tilesToTraverse.Contains(north))
                                {
                                    //  if so, check if we should overwrite the score
                                    if (north.curScore > traversalCost + cur.curScore)
                                    {
                                        north.curScore = traversalCost + cur.curScore;
                                    }
                                }
                                else
                                {
                                    //  update the score of the tile
                                    north.curScore += traversalCost + cur.curScore;



                                    //  if so, we can add it to the queue
                                    tilesToTraverse.Enqueue(north);
                                }
                            }
                        }
                    }
                    //  if the front tile of the queue has a valid east connection
                    if (cur.hasEast())
                    {
                        Tile east = cur.East;
                        if (!east.Visited)
                        {
                            //  check to see if the north value is contained in the CLOSED list
                            //  AND check if the score of that tile plus the tile of the current score would be GREATER than max move
                            //  check if that tile is NOT an obstacle

                            if (!unit.accessibleTiles.Contains(east)
                                && (traversalCost + cur.curScore) <= range
                                && !east.Obstacle)
                            {
                                //  check if the tile we are looking at is currently in the tilesToTraverse queue
                                if (tilesToTraverse.Contains(east))
                                {
                                    //  if so, check if we should overwrite the score
                                    if (east.curScore > traversalCost + cur.curScore)
                                    {
                                        east.curScore = traversalCost + cur.curScore;
                                    }
                                }
                                else
                                {
                                    //  update the score of the tile
                                    east.curScore += traversalCost + cur.curScore;



                                    //  if so, we can add it to the queue
                                    tilesToTraverse.Enqueue(east);
                                }
                            }
                        }
                    }
                    //  if the front tile of the queue has a valid south connection
                    if (cur.hasSouth())
                    {
                        Tile south = cur.South;
                        if (!south.Visited)
                        {
                            //  check to see if the north value is contained in the CLOSED list
                            //  AND check if the score of that tile plus the tile of the current score would be GREATER than max move
                            //  check if that tile is NOT an obstacle

                            if (!unit.accessibleTiles.Contains(south)
                                && (traversalCost + cur.curScore) <= range
                                && !south.Obstacle)
                            {
                                //  check if the tile we are looking at is currently in the tilesToTraverse queue
                                if (tilesToTraverse.Contains(south))
                                {
                                    //  if so, check if we should overwrite the score
                                    if (south.curScore > traversalCost + cur.curScore)
                                    {
                                        south.curScore = traversalCost + cur.curScore;
                                    }
                                }
                                else
                                {
                                    //  update the score of the tile
                                    south.curScore += traversalCost + cur.curScore;


                                    //  if so, we can add it to the queue
                                    tilesToTraverse.Enqueue(south);
                                }
                            }
                        }
                    }
                    //  if the front tile of the queue has a valid west connection
                    if (cur.hasWest())
                    {
                        Tile west = cur.West;
                        if (!west.Visited)
                        {
                            //  check to see if the north value is contained in the CLOSED list
                            //  AND check if the score of that tile plus the tile of the current score would be GREATER than max move
                            //  check if that tile is NOT an obstacle

                            if (!unit.accessibleTiles.Contains(west)
                                && (traversalCost + cur.curScore) <= range
                                && !west.Obstacle)
                            {
                                //  check if the tile we are looking at is currently in the tilesToTraverse queue
                                if (tilesToTraverse.Contains(west))
                                {
                                    //  if so, check if we should overwrite the score
                                    if (west.curScore > traversalCost + cur.curScore)
                                    {
                                        west.curScore = traversalCost + cur.curScore;
                                    }
                                }
                                else
                                {
                                    //  update the score of the tile
                                    west.curScore += traversalCost + cur.curScore;


                                    //  if so, we can add it to the queue
                                    tilesToTraverse.Enqueue(west);
                                }
                            }
                        }
                    }

                    //  assign the current tile as visited
                    cur.Visited = true;
                    //  pop from the front of the queue and add it to the output list
                    unit.accessibleTiles.Add(tilesToTraverse.Dequeue());
                    //  break the loop if the open list is empty!
                } while (tilesToTraverse.Count != 0);
            }

            //  if the unit is not dead and not focused
            if (!unit.IsDead && unit.uiController.showDangerZone)
            {
                //  for each tile in the list tiles:
                foreach (Tile tile in unit.accessibleTiles)
                {

                    tile.rend.color = Color.magenta;

                    tile.Visited = false;
                    tile.curScore = 0;
                }
            }
        }
    }

    public void ChangeHighlight(List<Tile> tiles, Color color)
    {
        foreach (Tile tile in tiles)
        {
            tile.rend.color = color;
        }
    }
}
