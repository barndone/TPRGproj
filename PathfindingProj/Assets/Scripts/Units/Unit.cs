using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //  stores the units position as a row/col index
    [SerializeField] public Vector2 mapPosition;

    //  position of the unit at the start of a turn
    private Vector2 startOfTurnPos;
    //  public get/set for startOfTurnPos
    public Vector2 StartOfTurnPos { get { return startOfTurnPos; } set { startOfTurnPos = value; } }

    //  reference to the gridManager script, used for pathfinding
    [SerializeField] public GridManager gridManager;
    [SerializeField] public UIController uiController;
    [SerializeField] public TurnManager turnManager;

    //  flag for if the unit is selected
    [SerializeField] public bool isSelected;

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

    public bool waiting = false;

    public bool selectable = true;

    //  attack range of the unit
    [SerializeField] private int atkRange = 1;
    public int AttackRange { get { return atkRange; } set { atkRange = value; } }

    //  skill range of the unit
    [SerializeField] private int skillRange = 1;
    public int SkillRange { get { return skillRange; } set { skillRange = value; } }

    //  movement range of the unit
    [SerializeField] private int maxMove = 3;
    public int MaxMove { get { return maxMove; } set { maxMove = value; } }

    //  Timer used for delay between moving from tile to tile
    [SerializeField] private float timeToWait = 0.5f;
    private float timer;

    public bool ally = false;

    //  define the stats shared between all classes
    //  current health of this unit
    [SerializeField] protected int curHealth = 10;
    //  maximum health of this class
    [SerializeField] protected int maxHealth = 10;
    //  attack stat of this class
    [SerializeField] protected int attack = 2;
    //  defence stat of this class
    [SerializeField] protected int defence = 1;

    //  accessor for the current health
    public int CurHealth { get { return curHealth; } set { curHealth = value; } }
    //  accessor for the max health
    public int MaxHealth { get { return maxHealth; } }
    //  accessor for the attack
    public int Attack { get { return attack; } }
    //  accessor for the defence
    public int Defence { get { return defence; } }

    //  which direction this unit is facing
    public enum direction
    {
        north,
        east,
        south,
        west
    }


    void Start()
    {
        //  assign the turn manager, grid manager, and UI controller
        turnManager = FindObjectOfType<TurnManager>();
        gridManager = FindObjectOfType<GridManager>();
        uiController = FindObjectOfType<UIController>();

        //  assign the current tile
        currentTile = gridManager.map[mapPosition];
        transform.position = currentTile.transform.position;
        //  flag the current tile as occupied
        currentTile.Occupied = true;
        //  mark the occupying unit of the current tile as this unit
        currentTile.occupyingUnit = this;
        //  get the animator component
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        //  initialize the timer to the configurable wait time
        timer = timeToWait;
    }

    void Update()
    {
        //  if this unit is selected
        if (isSelected)
        {
            //  look for if the player wants to take a given action!
            if (uiController.moveWish)
            {
                //  check if it has a path
                if (!hasMoved)
                {
                    gridManager.HideAccessibleTiles(this, accessibleTiles);
                    accessibleTiles.Clear();
                    gridManager.ResetTiles();

                    gridManager.ShowAccessibleTiles(this, out accessibleTiles);

                    if (hasPath)
                    {
                        acting = true;

                        gridManager.HideAccessibleTiles(this, accessibleTiles);
                        accessibleTiles.Clear();
                        gridManager.ResetTiles();

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
                            if (pathIndex > MaxMove || pathIndex >= pathToMove.Count)
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
                            //  mark the occupying unit of the current tile as this unit
                            currentTile.occupyingUnit = this;
                        }
                        //  otherwise, the unit should wait before acting
                        else
                        {
                            //  decrement the timer
                            timer -= Time.deltaTime;
                        }
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
                        else
                        {
                            accessibleTiles.Clear();
                            gridManager.ResetTiles();
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
                        else
                        {
                            accessibleTiles.Clear();
                            gridManager.ResetTiles();
                        }
                    }
                }
            }
            if (uiController.cancelWish)
            {
                //  start of turn, store the 
            }
            if (uiController.waitWish)
            {
                EndOfUnitActions();
            }
        }
        //  if the unit has acted and moved
        if (hasActed && hasMoved)
        {
            sprite.color = Color.grey;
            waiting = true;
        }
        //  otherwise, no color change
        else { sprite.color = Color.white; }
    }

    protected void OnMouseEnter()
    {
        //  on mouse entering the collider of this unit:
        //  begin highlighted animation
        animator.SetBool("highlighted", true);
    }

    protected void OnMouseExit()
    {
        //  on mouse exiting the collider of this unit:
        //  stop the highlighted animation
        animator.SetBool("highlighted", false);
    }

    protected void OnMouseOver()
    {
        //  if LMB is pressed while hovering over this unit:
        if (Input.GetMouseButtonDown(0))
        {
            //  check if this unit is on an ally
            //  AND it is your turn
            if (turnManager.playerTurn && ally)
            {
                //  then we can do the song and dance
                //  check if we can make this the active unit:
                if (gridManager.activeUnit == null)
                {
                    //  check if the unit has not taken its actions
                    if (!waiting)
                    {
                        //  then we can go through the selection process!
                        //  swap whether it was selected
                        isSelected = !isSelected;
                        //  update the selected animation depending on isSelected being T/F
                        animator.SetBool("selected", isSelected);

                        //  if the unit is now selected
                        if (isSelected)
                        {
                            //  pass that this is the active unit to the grid manager
                            gridManager.activeUnit = this;
                            uiController.unitSelected = isSelected;
                            //gridManager.ShowAccessibleTiles(this, out accessibleTiles);
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
                }
            }

            //  otherwise, just show their stats
            else
            {
                //  do the stats thing

                    //  temp for testing

                //  then we can do the song and dance
                //  check if we can make this the active unit:
                if (gridManager.activeUnit == null)
                {
                    //  check if the unit has not taken its actions
                    if (!waiting)
                    {
                        //  then we can go through the selection process!
                        //  swap whether it was selected 
                        isSelected = !isSelected;
                        //  update the selected animation depending on isSelected being T/F
                        animator.SetBool("selected", isSelected);

                        //  if the unit is now selected
                        if (isSelected)
                        {
                            //  pass that this is the active unit to the grid manager
                            gridManager.activeUnit = this;
                            uiController.unitSelected = isSelected;
                            //gridManager.ShowAccessibleTiles(this, out accessibleTiles);
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
                }
            }

            //
            
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
    public void EndOfUnitActions()
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

        hasActed = true;
        hasMoved = true;

        //  hide the accessible tiles of the unit
        gridManager.HideAccessibleTiles(this, accessibleTiles);
        //  mark that there is no longer an active unit
        gridManager.activeUnit = null;
        //  clear the accessible tiles of the unit
        accessibleTiles.Clear();
        gridManager.ResetTiles();
        uiController.ResetFlags();
        waiting = true;
    }

    public void StartOfTurn()
    {
        waiting = false;
        hasActed = false;
        hasMoved = false;
        isSelected = false;

        //  update the animation to not be selected
        animator.SetBool("selected", isSelected);
        uiController.unitSelected = isSelected;

        if (gridManager.activeUnit != null)
        {
            gridManager.activeUnit = null;
        }

        uiController.ResetFlags();
    }

    //  functionality for attacking
    public void Attacking(Tile targetTile) 
    {
        //  check if it is a valid tile to attack
        if (ValidTile(targetTile.MapPos))
        {
            //  does that tile actually have a unit on it?
            if (targetTile.occupyingUnit != null)
            {
                Unit other = targetTile.occupyingUnit;

                //  check if that unit is an enemy
                if (this.ally != other.ally)
                {
                    //  time to attack!
                    other.CurHealth -= (this.Attack - other.Defence);
                }
                else
                {
                    this.hasAction = false;
                    this.hasActed = false;
                    Debug.Log("Target tile occupied by ally");
                }
            }

            else
            {
                this.hasAction = false;
                this.hasActed = false;
                Debug.Log("Target tile empty");
            }
        }
    }
    //  functionality for their skill
    public virtual void Skill(Tile targetTile) { /* Intentionally left blank - to be implemented in derived classes */ }
}
