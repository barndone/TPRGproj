using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    //  flag for whether it is a player or CPU turn
    //  default to true
    public bool playerTurn = true;

    //  reference to the player controller (stores each unit the player has control over)
    [SerializeField] public PlayerController player;

    //  reference to the computer controller (stores each unit the computer has contorl over)
    [SerializeField] public ComputerController cpu;

    GridManager gridManager;

    //  Fields for controlling audio:
    bool shouldExit = false;

    [SerializeField] AudioSource buttonSource;
    [SerializeField] AudioClip buttonSound;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip musicIntro;
    [SerializeField] AudioClip victoryClip;
    [SerializeField] AudioClip defeatClip;

    //  method called at the start of every turn, makes sure if it is the player turn, enemies can't be selected
    //  and vice versa
    private void TurnStart()
    {
        //  if it is the players turn
        if (playerTurn)
        {
            //  for each unit in the player party
            foreach(Unit unit in player.party)
            {
                if (!unit.IsDead)
                {
                    //  reset the flags on that unit to allow for movement and action
                    unit.StartOfTurn();
                    unit.selectable = true;
                }

            }
            //  mark that the player has actions to take
            player.noMoreActions = false;

            foreach(Unit unit in cpu.party)
            {
                unit.selectable = false;
                unit.EndOfUnitActions();
            }

            gridManager.CalculateDangerZone(cpu.party);
            
        }

        //  otherwise, it's the CPU turn
        else
        {
            //  for each unit in the CPU party
            foreach (Unit unit in cpu.party)
            {
                unit.accessibleTiles.Clear();

                if (!unit.IsDead)
                {
                    //  reset the flags on that unit to allow for movement and action
                    unit.StartOfTurn();
                    unit.selectable = true;
                }
            }
            //  mark that the cpu controller has actions to take
            cpu.noMoreActions = false;

            foreach (Unit unit in player.party)
            {
                unit.selectable = false;
                unit.EndOfUnitActions();
            }
        }
    }

    //  used for signifying the end of the turn
    public void TurnEnd()
    {
        //  flip the value of the bool playerTurn
        playerTurn = !playerTurn;
        TurnStart();
    }

    void LateUpdate()
    {
        //  if the CPU noMoreActions is false and it is NOT the player's turn
        if (!cpu.noMoreActions && !playerTurn)
        {
            //  initialize a counter
            int counter = 0;
            //  check each unit in the list
            foreach (Unit unit in cpu.party)
            {
                //  if that unit is waiting:
                if (unit.waiting || unit.IsDead)
                {
                    //  increment the counter
                    counter++;
                }
            }

            //  after iteration, if the count is equal to the size of the party
            //  AKA all units are waiting
            if (counter == cpu.party.Count)
            {
                //  we can flag that there are no more actions so we can end the turn
                cpu.noMoreActions = true;
            }
        }

        //  otherwise, if the player has more actions to take and it is currently their turn
        else if (!player.noMoreActions && playerTurn)
        {
            //  initialize a counter
            int counter = 0;
            //  check each unit in the list
            foreach (Unit unit in player.party)
            {
                //  if that unit is waiting:
                if (unit.waiting || unit.IsDead)
                {
                    //  increment the counter
                    counter++;
                }
            }

            //  if the counter is equal to the size of the party
            //  all units are waiting
            if (counter == player.party.Count)
            {
                //  we can flag that there are no more actions so we can end the turn
                player.noMoreActions = true;
            }
        }
    }


    void Update()
    {
        if (!shouldExit)
        {
            //  if there are no more actions the CPU could take AND it is NOT currently the player's turn
            if (cpu.noMoreActions && !playerTurn)
            {
                //  time to end it all
                TurnEnd();
            }

            //  otherwise if there are no more actions the player could take and it is currently the player's turn
            else if (player.noMoreActions && playerTurn)
            {
                //  time to end it all
                TurnEnd();
            }
        }
    }

    private void Awake()
    {
        StartCoroutine(PlayMusicIntro(musicIntro));

        gridManager = FindObjectOfType<GridManager>();
    }

    //  Called at the end of every frame to see if the victory condition is met
    //      all enemies defeated? hell yeah brother
    public void VictoryCondition()
    {
        //  the player has won!
        //  queue victory fanfare!

        SaveSystem.instance.SaveStats();
        StartCoroutine(DelayedLoad(victoryClip, 3));
    }

    //  Called at the end of every freame to see if the defeat condition is met:
    //      all units defeated? hell no brother
    public void DefeatCondition()
    {
        //  the player has lost...
        //  queue sad trombone :(

        SaveSystem.instance.SaveStats();
        StartCoroutine(DelayedLoad(defeatClip, 3));
    }

    //Delayed load for a scene with parameters for an audio clip, and a scene index
    IEnumerator DelayedLoad(AudioClip clip, int sceneIndex)
    {
        if (!shouldExit)
        {
            shouldExit = true;

            audioSource.Stop();
            audioSource.PlayOneShot(clip);

            yield return new WaitForSecondsRealtime(clip.length);

            shouldExit = false;
            SceneManager.LoadScene(sceneIndex);
        }
    }

    //  Play the intro to the scene track before starting to play the loopable part of the track
    IEnumerator PlayMusicIntro(AudioClip intro)
    {
        audioSource.PlayOneShot(intro);

        yield return new WaitForSecondsRealtime(intro.length);

        audioSource.Play();
    }
}
