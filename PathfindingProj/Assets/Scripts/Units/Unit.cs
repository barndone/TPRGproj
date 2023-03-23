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
    [SerializeField] public BaseController partyManager;

    public UnitFrame playerFrame;
    public UnitFrame targetFrame;

    //  flag for if the unit is selected
    [SerializeField] public bool isSelected;

    //  reference to the tile this unit currently occupies
    public Tile currentTile;

    //  reference to the animator attached to the unit
    [SerializeField] public Animator animator;

    //  reference for accessible tiles
    //  used in checking for valid movements
    public List<Tile> accessibleTiles = new List<Tile>();
    //  list containing the Dijkstra generated path the unit will move through
    public List<Vector2> pathToMove = new List<Vector2>();

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] public Sprite deadSprite;

    public int unitID;

    public bool stunned;

    //  flag for if the unit has a path
    public bool hasPath = false;
    //  index for iteration through path list
    private int pathIndex = 1;

    public bool hasMoved = false;
    public bool hasActed = false;

    public bool hasAction = false;

    public bool acting = false;

    public bool waiting = false;

    public bool selectable = true;

    public bool canAttackAdjacent = true;

    public bool unitCantBeReached = false;

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
    [SerializeField] private float timeToWait = 0.2f;
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

    //  check if a unit is dead
    public bool IsDead { get { return curHealth <= 0; } }

    private bool gravestone = false;
    //  accessor for the max health
    public int MaxHealth { get { return maxHealth; } }
    //  accessor for the attack
    public int Attack { get { return attack; } }
    //  accessor for the defence
    public int Defence { get { return defence; } }

    //  

    public int dmgDone_val;
    public int dmgTaken_val;
    public int healingDone_val;
    public int healingTaken_val;

    //  which direction this unit is facing
    public List<Tile> AdjacentTiles = new List<Tile>();

    [SerializeField] public AudioSource audioSource;

    [SerializeField] public AudioClip[] walkSounds;
    [SerializeField] public AudioClip[] atkSounds;
    [SerializeField] public AudioClip skillSound;
    [SerializeField] public AudioClip[] dmgTaken;
    [SerializeField] public AudioClip deathSound;


    [SerializeField] public bool canHeal = false;

    public Unit target = null;

    public IDecision unitDecisionRef;

    public bool canUseSkill = true;



    void Start()
    {
        //  assign the turn manager, grid manager, and UI controller
        turnManager = FindObjectOfType<TurnManager>();
        gridManager = FindObjectOfType<GridManager>();
        uiController = FindObjectOfType<UIController>();
        audioSource = FindObjectOfType<AudioSource>();
        playerFrame = uiController.unitFrame.GetComponent<UnitFrame>();
        targetFrame = uiController.targetFrame.GetComponent<UnitFrame>();

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
        //  if this unit is the active unit in the party manager
        if (partyManager.activeUnit == this)
        {
            //  check for move, action, or wait wish
            if (uiController.moveWish)
            {
                //  handle movement if it hasn't moved
                if (!hasMoved)
                {
                    //  check if it isn't currently moving
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
                            //  pick a random walk sound and play that sound
                            var randomWalk = walkSounds[Random.Range(0, walkSounds.Length)];
                            audioSource.PlayOneShot(randomWalk);

                            timer = timeToWait;
                            //  if so, mark the current tile is unoccupied
                            gridManager.map[mapPosition].Occupied = false;

                            //  update the mapPosition to the difference between the current point along the math and the current map position
                            //  ex: mapPos = {0,0}; point = {1,0}; delta is {1,0}

                            //  if the pathIndex is greater than or equal to the number of entries in pathToMove:
                            if (pathIndex > MaxMove || pathIndex >= pathToMove.Count)
                            {
                                //  Time to star the action Process
                                uiController.moveWish = false;
                                hasMoved = true;
                                acting = false;

                                //  mark that there is no longer a path
                                hasPath = false;

                                //  hide the accessible tiles of the unit
                                gridManager.HideAccessibleTiles(this, accessibleTiles);
                                accessibleTiles.Clear();
                                gridManager.ResetTiles();

                                if (hasActed)
                                {
                                    EndOfUnitActions();
                                }
                            }

                            else
                            {
                                //  if the next location on the path is the target position:
                                if (target != null && pathToMove[pathIndex] == target.mapPosition && hasPath)
                                {

                                    mapPosition = pathToMove[pathIndex - 1];
                                    //  update the transform of the unit in world space to the world space position of the new tile
                                    transform.position = gridManager.map[mapPosition].gameObj.transform.position;
                                    //  update currentTile reference
                                    currentTile = gridManager.map[mapPosition];

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

                                mapPosition += (pathToMove[pathIndex] - mapPosition);

                                //  update the transform of the unit in world space to the world space position of the new tile
                                transform.position = gridManager.map[mapPosition].gameObj.transform.position;
                                //  update currentTile reference
                                currentTile = gridManager.map[mapPosition];
                                //  incrememnt pathIndex
                                pathIndex++;
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

            if (uiController.attackWish || uiController.skillWish)
            {
                //  handle actions
                if (uiController.attackWish)
                {
                    if (!hasActed)
                    {
                        gridManager.HideAccessibleTiles(this, accessibleTiles);
                        accessibleTiles.Clear();
                        gridManager.ResetTiles();

                        gridManager.ShowAttackableTiles(this, out accessibleTiles);
                    }
                }

                if (uiController.skillWish)
                {
                    if (!hasActed)
                    {
                        gridManager.HideAccessibleTiles(this, accessibleTiles);
                        accessibleTiles.Clear();
                        gridManager.ResetTiles();

                        gridManager.ShowAttackableTiles(this, out accessibleTiles);

                        if (hasAction)
                        {
                            //  play the skill sound
                            audioSource.PlayOneShot(skillSound);


                            hasAction = false;
                            hasActed = true;

                            acting = false;
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
            }

            if (uiController.waitWish)
            {
                //  handle waiting
                EndOfUnitActions();

            }

            if (uiController.cancelWish)
            {
                //  handle cancelling movement
                //  if the unit has moved:
                if (hasMoved && !hasActed)
                {
                    //  re-assign hasMoved to false
                    hasMoved = false;
                    //  move the unit back to their position at the start of the turn:

                    //  mark the current tile is unoccupied
                    gridManager.map[mapPosition].Occupied = false;
                    //  assign mapPosition to startOfTurnPos
                    mapPosition = startOfTurnPos;
                    //  update the transform of the unit in world space to the world space position of the new tile
                    transform.position = gridManager.map[mapPosition].gameObj.transform.position;
                    //  update currentTile reference
                    currentTile = gridManager.map[mapPosition];
                    //  mark the current tile as occupied
                    gridManager.map[mapPosition].Occupied = true;
                    //  mark the occupying unit of the current tile as this unit
                    currentTile.occupyingUnit = this;

                    //  cleanup:
                    //  clear path to move
                    pathToMove.Clear();
                    //  reset pathIndex to 1
                    pathIndex = 1;
                    //  mark that there is no longer a path
                    hasPath = false;

                }
                //  otherwise, do nothing but reset cancelWish to false!
                uiController.cancelWish = false;
            }
        }
        //  if the unit has acted and moved
        if (hasActed && hasMoved)
        {
            waiting = true;
        }

        if (waiting) { sprite.color = Color.grey; }
        //  otherwise, no color change
        else { sprite.color = Color.white; }

        //  if this unit is NOT currently a gravestone:
        if (!gravestone)
        {        
            //  check if our health is <= 0
            if (IsDead)
            {
                //  play our death sound!
                audioSource.PlayOneShot(deathSound);

                //  swap the sprite to the gravestone
                sprite.sprite = deadSprite;
                sprite.sortingLayerName = "Background";
                sprite.sortingOrder = 1;
                this.selectable = false;
                animator.enabled = false;

                //  mark that it is now a gravestone (to avoid calling this code a second time!)
                gravestone = true;

                //  clean up tile references

                this.currentTile.Occupied = false;
                this.currentTile.Obstacle = false;
                this.currentTile.occupyingUnit = null;
                this.currentTile = null;
                
                //
                int counter = 0;

                //  if this unit is an ally
                if (this.ally)
                {
                    //  check each unit in the party
                    foreach (Unit unit in this.partyManager.party)
                    {
                        //  if its dead
                        if (unit.IsDead)
                        {
                            //  increment the counter
                            counter++;
                        }
                    }

                    //  if the counter is equal to the number in the party
                    if (counter == this.partyManager.party.Count)
                    {
                        //  defeat, party defeated
                        turnManager.DefeatCondition();

                    }
                }
                //  otherwise it is an enemy
                else
                {
                    //  check each unit in the cpu party
                    foreach (Unit unit in this.partyManager.party)
                    {
                        //  if its dead
                        if (unit.IsDead)
                        {
                            //  increment the counter
                            counter++;
                        }
                    }

                    //  if the counter is equal to the number in the party
                    if (counter == this.partyManager.party.Count)
                    {
                        //  victory, all enemies defeated
                        turnManager.VictoryCondition();

                    }
                }
            }
        }
    }

    protected void OnMouseEnter()
    {
        //  on mouse entering the collider of this unit:
        //  begin highlighted animation
        animator.SetBool("highlighted", true);

        //  if no unit is selected, update/show the "player" frame on entering collider
        if (gridManager.activeUnit == null)
        {
            uiController.unitFrame.SetActive(true);
            playerFrame.UpdateUnitFrame(this);
        }
        //  otherwise, update/show the "target" frame on entering collider
        else
        {
            uiController.targetFrame.SetActive(true);
            targetFrame.UpdateUnitFrame(this);
        }

        gridManager.ShowAccessibleTiles(this, out accessibleTiles);
    }

    protected void OnMouseExit()
    {
        //  on mouse exiting the collider of this unit:
        //  stop the highlighted animation
        animator.SetBool("highlighted", false);

        //  if no unit is selected, hide the unit frame on mouse leaving collider
        if (gridManager.activeUnit == null)
        {
            uiController.unitFrame.SetActive(false);
            playerFrame.target = null;
        }
        //  otherwise, hide the "target" frame on leaving collider
        else
        {
            uiController.targetFrame.SetActive(false);
            targetFrame.target = null;
        }

        gridManager.HideAccessibleTiles(this, accessibleTiles);
    }

    protected void OnMouseOver()
    {
        //  if LMB is pressed while hovering over this unit:
        if (Input.GetMouseButtonDown(0))
        {

            //  then we can do the song and dance
            //  check if we can make this the active unit:
            if (gridManager.activeUnit == null)
            {
                //  check if this unit is on an ally
                //  AND it is your turn
                if (turnManager.playerTurn)
                {
                    //  if it is an ally:
                    if (ally)
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
                                //  and the unit controller
                                partyManager.activeUnit = this;
                                //  and the ui controller
                                uiController.unitSelected = isSelected;

                                playerFrame.UpdateUnitFrame(this);
                            }
                        }
                    }
                }
            }

            //  otherwise, we have an active unit and should check for actions
            else
            {
                //  cache the active unit
                Unit activeUnit = gridManager.activeUnit;

                //  check if THIS unit occupies a tile that the active unit can interact with
                if (activeUnit.ValidTile(this.mapPosition))
                {
                    if (activeUnit.uiController.attackWish)
                    {
                        activeUnit.hasAction = true;
                        activeUnit.Attacking(this.currentTile);
                    }

                    else if (activeUnit.uiController.skillWish)
                    {
                        activeUnit.hasAction = true;
                        activeUnit.Skill(this.currentTile);
                    }    
                }
            }
            
            if (uiController.showDangerZone)
            {
                if (!ally)
                {
                    gridManager.CalculateDangerZone(turnManager.cpu.party);
                }
            }
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

        hasActed = false;
        hasMoved = false;

        //  hide the accessible tiles of the unit
        gridManager.HideAccessibleTiles(this, accessibleTiles);
        //  mark that there is no longer an active unit
        gridManager.activeUnit = null;
        partyManager.activeUnit = null;
        //  clear the accessible tiles of the unit
        accessibleTiles.Clear();
        gridManager.ResetTiles();
        uiController.ResetFlags();

        waiting = true;

        //  if the unit is stunned
        if (stunned)
        {
            //  mark them as no longer stunned
            stunned = !stunned;
        }

        if (ally)
        {
            if (uiController.showDangerZone)
            {
                gridManager.CalculateDangerZone(turnManager.cpu.party);
            }
        }
    }

    public void StartOfTurn()
    {
        //  if the unit is not stunned, reset the flags
        if (!stunned)
        {
            waiting = false;
            hasActed = false;
            hasMoved = false;
            isSelected = false;
            unitCantBeReached = false;
        }


        //  update the animation to not be selected
        animator.SetBool("selected", isSelected);
        uiController.unitSelected = isSelected;

        if (gridManager.activeUnit != null)
        {
            gridManager.activeUnit = null;
        }

        uiController.ResetFlags();

        startOfTurnPos = mapPosition;
    }

    //  functionality for attacking
    public void Attacking(Tile targetTile) 
    {
        if (!hasActed)
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
                        int damage = (this.Attack - other.Defence);
                        //  time to attack!
                        other.CurHealth -= damage;
                        this.dmgDone_val += damage;
                        other.dmgTaken_val += damage;

                        this.hasAction = false;
                        this.hasActed = true;
                        this.uiController.attackWish = false;
                        StartCoroutine(PlayRandomAttackAndDamageSounds(other));

                        Debug.Log("Attacking " + other + ", current health: " + other.curHealth);
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

                gridManager.HideAccessibleTiles(this, accessibleTiles);
            }
        }

        //  once the unit has acted:
        if (hasActed)
        {
            if (hasMoved)
            {
                EndOfUnitActions();
            }

        }
    }
    //  functionality for their skill
    public virtual void Skill(Tile targetTile) { /* Intentionally left blank - to be implemented in derived classes */ }

    //  Play the intro to the scene track before starting to play the loopable part of the track
    IEnumerator PlayRandomAttackAndDamageSounds(Unit other)
    {
        //  get a random attacking sound
        var randomAtk = this.atkSounds[Random.Range(0, atkSounds.Length)];
        audioSource.PlayOneShot(randomAtk);

        yield return new WaitForSecondsRealtime(randomAtk.length);

        //  find a random damage taken sound
        var randomDamageSound = other.dmgTaken[Random.Range(0, dmgTaken.Length)];
        //  play that random sound
        other.audioSource.PlayOneShot(randomDamageSound);
    }

    
    private void LateUpdate()
    {
        if (!this.IsDead)
        {
            this.PopulateAdjacentTiles();
        }
    }


    private void PopulateAdjacentTiles()
    {
        AdjacentTiles.Clear();

        if (currentTile.hasNorth())
        {
            AdjacentTiles.Add(currentTile.North);
        }

        if (currentTile.hasEast())
        {
            AdjacentTiles.Add(currentTile.East);
        }

        if (currentTile.hasSouth())
        {
            AdjacentTiles.Add(currentTile.South);
        }

        if (currentTile.hasWest())
        {
            AdjacentTiles.Add(currentTile.West);
        }
    }
}
