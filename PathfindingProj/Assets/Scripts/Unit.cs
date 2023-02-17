using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] public Vector2 isoPos;

    [SerializeField] GridManager gridManager;

    private void Start()
    {
        isoPos = CoordinateUtils.ConvertToIsometric(transform.position);
    }

    private void Update()
    {

        if (Input.GetKeyUp(KeyCode.A))
        {
            gridManager.map[isoPos].Occupied = false;
            isoPos.y -= 1;
            if (isoPos.y < 0)
            {
                isoPos.y = 0;
            }
            //  if the tile is an obstacle:
            if (gridManager.map[isoPos].Obstacle || gridManager.map[isoPos].Occupied)
            {
                //  reverse the change
                isoPos.y += 1;
            }
            gridManager.map[isoPos].Occupied = true;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            gridManager.map[isoPos].Occupied = false;
            isoPos.y += 1;
            if (isoPos.y >= gridManager.Rows)
            {
                isoPos.y = gridManager.Rows - 1;
            }
            //  if the tile is an obstacle:
            if (gridManager.map[isoPos].Obstacle || gridManager.map[isoPos].Occupied)
            {
                //  reverse the change
                isoPos.y -= 1;
            }
            gridManager.map[isoPos].Occupied = true;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            gridManager.map[isoPos].Occupied = false;
            isoPos.x -= 1;
            if (isoPos.x < 0)
            {
                isoPos.x = 0;
            }
            //  if the tile is an obstacle:
            if (gridManager.map[isoPos].Obstacle || gridManager.map[isoPos].Occupied)
            {
                //  reverse the change
                isoPos.x += 1;
            }
            gridManager.map[isoPos].Occupied = true;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            gridManager.map[isoPos].Occupied = false;
            isoPos.x += 1;
            if (isoPos.x >= gridManager.Columns)
            {
                isoPos.x = gridManager.Columns - 1;
            }
            //  if the tile is an obstacle:
            if (gridManager.map[isoPos].Obstacle || gridManager.map[isoPos].Occupied)
            {
                //  reverse the change
                isoPos.x -= 1;
            }
            gridManager.map[isoPos].Occupied = true;
        }
        transform.position = gridManager.map[isoPos].gameObj.transform.position;
    }

}
