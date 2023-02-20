using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //  stores the units position as a row/col index
    [SerializeField] public Vector2 mapPosition;

    //  reference to the gridManager script, used for pathfinding
    [SerializeField] GridManager gridManager;
    [SerializeField] public UIController uiController;

    //  flag for if the unit is selected
    [SerializeField] private bool isSelected;

    //  reference to the tile this unit currently occupies
    public Tile currentTile;

    //  reference to the animator attached to the unit
    [SerializeField] private Animator animator;

    //  reference for accessible tiles
    //  used in checking for valid movements
    public List<Tile> accessibleTiles = new List<Tile>();
    //  list containing the Dijkstra generated path the unit will move through
    public List<Vector2> pathToMove = new List<Vector2>();

    [SerializeField] private SpriteRenderer sprite;
    //  flag for if the unit has a path
    public bool hasPath = false;
    //  index for iteration through path list
    private int pathIndex = 1;

    public bool hasMoved = false;
    public bool hasActed = false;

    public bool hasAction = false;

    private bool acting = false;

    //  attack range of the unit
    [SerializeField] private int atkRange = 1;
    public int AttackRange { get { return atkRange; } set { atkRange = value; } }

    //  skill range of the unit
    [SerializeField] private int skillRange = 2;
    public int SkillRange { get { return skillRange; } set { skillRange = value; } }

    //  movement range of the unit
    [SerializeField] private int maxMove;
    public int MaxMove { get { return maxMove; } set { maxMove = value; } }

    //  Timer used for delay between moving from tile to tile
    [SerializeField] private float timeToWait = 0.5f;
    private float timer;



    private void Start()
    {
        //  cache the map position of the unit
        mapPosition = CoordinateUtils.ConvertToIsometric(transform.position);
        //  assign the current tile
        currentTile = gridManager.map[mapPosition];
        //  flag the current tile as occupied
        currentTile.Occupied = true;
        //  get the animator component
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        //  initialize the timer to the configurable wait time
        timer = timeToWait;
    }

    private void Update()
    {
        //  if this unit is selected
        if (isSelected)
        {
            switch (uiController.moveWish)
            {
                case true:
                    //  check if it has a path
                    if (!hasMoved && !acting)
                    {
                        gridManager.ShowAccessibleTiles(this, out accessibleTiles);
                    }
                    break;
                case false:
                    gridManager.HideAccessibleTiles(this, accessibleTiles);
                    break;
            }


            //  check if it has a path
            if (!hasMoved)
            {
                if (hasPath)
                {
                    acting = true;

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

                            //  Time to star the action Process
                            uiController.moveWish = false;
                            hasMoved = true;
                            acting = false;
                            //  hide the accessible tiles of the unit
                            gridManager.HideAccessibleTiles(this, accessibleTiles);
                            accessibleTiles.Clear();
                            gridManager.ResetTiles();
                            if (hasActed)
                            {
                                EndOfUnitActions();
                            }
                        }
            
                        //  mark the current tile as occupied
                        gridManager.map[mapPosition].Occupied = true;
                    }
                    //  otherwise, the unit should wait before acting
                    else
                    {
                        //  decrement the timer
                        timer -= Time.deltaTime;
                    }
                }
            }
            if (uiController.attackWish)
            {           
                if (!hasActed)
                {
                    gridManager.HideAccessibleTiles(this, accessibleTiles);
                    accessibleTiles.Clear();
                    gridManager.ResetTiles();
            
                    gridManager.ShowAccessibleTiles(this, out accessibleTiles);
                    if (hasAction)
                    {
                        hasAction = false;
                        hasActed = true;
                        Debug.Log("Attacking!");
                        if (hasMoved)
                        {
                            EndOfUnitActions();
                        }
                    }
                }
            }
            if (uiController.skillWish)
            {
                if (!hasActed)
                {
                    gridManager.HideAccessibleTiles(this, accessibleTiles);
                    accessibleTiles.Clear();
                    gridManager.ResetTiles();
            
                    gridManager.ShowAccessibleTiles(this, out accessibleTiles);
                    if (hasAction)
                    {
                        hasAction = false;
                        hasActed = true;
                        Debug.Log("Used Skill!");
                        if (hasMoved)
                        {
                            EndOfUnitActions();
                        }
                    }
                }
            }
            if (uiController.cancelWish)
            {
            
            }
            if (uiController.waitWish)
            {
            
            }

        }
        //  if the unit has acted and moved
        if (hasActed && hasMoved)
        {
            sprite.color = Color.grey;
        }
    }

    private void OnMouseEnter()
    {
        //  on mouse entering the collider of this unit:
        //  begin highlighted animation
        animator.SetBool("highlighted", true);
    }

    private void OnMouseExit()
    {
        //  on mouse exiting the collider of this unit:
        //  stop the highlighted animation
        animator.SetBool("highlighted", false);
    }

    private void OnMouseOver()
    {
        //  if LMB is pressed while hovering over this unit:
        if (Input.GetMouseButtonDown(0))
        {
            //  check if the unit has not moved
            if (!hasMoved)
            {               
                //  then we can go through the selection process!
                //  swap whether it was selected
                isSelected = !isSelected;

                //  update the selected animation depending on isSelected being T/F
                animator.SetBool("selected", isSelected);
                //  if the unit is now selected
                if (isSelected)
                {
                    //  if the grid doesn't have an active unit
                    if (gridManager.activeUnit == null)
                    {
                        //  pass that this is the active unit to the grid manager
                        gridManager.activeUnit = this;
                        uiController.unitSelected = isSelected;

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
                    //  mark that no unit is active
                    gridManager.activeUnit = null;
                    //  clear the accessible tiles list
                    accessibleTiles.Clear();
                    uiController.unitSelected = isSelected;
                }
            }
            //  otherwise, do nothing
        }
    }

    //  check if the passed tile position is an accessible tile
    public bool ValidTile(Vector2 tilePos)
    {
        //  return whether the accessible tiles list contains the tile pointed to by that position
        return accessibleTiles.Contains(gridManager.map[tilePos]);
    }

    //  Used to reset the state of the unit after a unit has taken all available actions
    private void EndOfUnitActions()
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
        uiController.unitSelected = isSelected;
        //  update the animation to not be selected
        animator.SetBool("selected", isSelected);

        //  hide the accessible tiles of the unit
        gridManager.HideAccessibleTiles(this, accessibleTiles);
        //  mark that there is no longer an active unit
        gridManager.activeUnit = null;
        //  clear the accessible tiles of the unit
        accessibleTiles.Clear();
        gridManager.ResetTiles();
    }
}
