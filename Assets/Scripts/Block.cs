using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public float swipeAngle = 0;
    public int col;
    public int row;
    public int targetX;
    public int targetY;



    private Vector2 initialTouchPosition;
    private Vector2 finalTouchPosition;

    private Vector2 tempPosition;

    private Board board;
    private GameObject otherBlock;


	// Use this for initialization
	void Start ()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        col = targetX;
        row = targetY;
	}
	
	// Update is called once per frame
	void Update ()
    {
        targetX = col;
        targetY = row;

        if(Mathf.Abs(targetX - transform.position.x) > .1f)
        {
            // Move block to target position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        } else
        {
            // Set to final position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            board.allBlocks[col, row] = this.gameObject;
        }

        if(Mathf.Abs(targetY - transform.position.y) > .1f)
        {
            // Move block to target position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        } else
        {
            // Set to final position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            board.allBlocks[col, row] = this.gameObject;
        }
    }

    private void OnMouseDown()
    {
        // Convert position to Unity units
        initialTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }


    void CalculateAngle()
    {
        if(Vector2.Distance(initialTouchPosition, finalTouchPosition) < .3f)
            return;

        swipeAngle = Mathf.Atan2(
            finalTouchPosition.y - initialTouchPosition.y,
            finalTouchPosition.x - initialTouchPosition.x
            ) * 180 / Mathf.PI;
        MoveBlocks();
    }

    void MoveBlocks()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && col < board.width)
        {
            // Right Swipe
            otherBlock = board.allBlocks[col + 1, row];
            otherBlock.GetComponent<Block>().col -= 1;
            col += 1;
        } else if(swipeAngle > 45 && swipeAngle <= 135 && row < board.height)
        {
            // Up Swipe
            otherBlock = board.allBlocks[col, row + 1];
            otherBlock.GetComponent<Block>().row -= 1;
            row += 1;
        } else if(swipeAngle > 135 || swipeAngle <= -135 && col > 0)
        {
            // Left Swipe
            otherBlock = board.allBlocks[col - 1, row];
            otherBlock.GetComponent<Block>().col += 1;
            col -= 1;
        } else if(swipeAngle <= -45 && swipeAngle > -135 && row > 0)
        {
            // Down Swipe
            otherBlock = board.allBlocks[col, row - 1];
            otherBlock.GetComponent<Block>().row += 1;
            row -= 1;
        }
    }
}
