﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour {
    public static PlayerController playerController;
    public static Player player;
    public Robot playerRobot;
    public RobotController playerRC;
    Rigidbody2D rb;

    public bool movementEnabled = true;
    public bool leftMovementEnabled = true;
    public bool rightMovementEnabled = true;
    static public Vector2 GetMouseInWorldSpace()
    {
        Vector2 pos;
        var temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.x = temp.x;
        pos.y = temp.y;
        return pos;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        playerRobot = GetComponent<Robot>();
        playerRC = GetComponent<RobotController>();
    }

    void Start()
    {
        if(playerController != null)
        {
            Destroy(gameObject);
        }else
        {
            playerController = this;
        }

        player = GetComponent<Player>();

        playerRobot.robotDiedEvent.AddListener(OnDeath);
    }


	
	// Update is called once per frame
	void Update () {

        if (movementEnabled)
        {
            if (Input.GetKey(KeyCode.D))
            {
                if(rightMovementEnabled)
                    playerRC.MoveHorizontal(playerRobot.speed * Time.deltaTime);

            }
            else if (Input.GetKey(KeyCode.A))
            {
                if(leftMovementEnabled)
                    playerRC.MoveHorizontal(-1 * playerRobot.speed * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerRC.Jump();
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            playerRobot.headLampActive = !playerRobot.headLampActive;
            //playerRC.SetHeadlightOn(true);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            playerRC.FireRightArm();
        }


        //Test function since enemies not implemented
        if (Input.GetKeyDown(KeyCode.T))
        {
            //For testing
            player.GetComponent<Robot>().takeDamage(1, gameObject);
        }


        //Looting body parts
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 pos = new Vector2(transform.position.x + 1, transform.position.y);
            var colls = Physics2D.OverlapBoxAll(pos, new Vector2(1, 1.5f), 0);

            foreach(Collider2D col in colls)
            {
                if (col.gameObject.GetComponent<BodyPart>())
                {
                    Debug.Log("Equipping: " + col.gameObject);
                    PlayerController.player.GetComponent<Robot>().EquipBodyPart(col.gameObject.GetComponent<BodyPart>());
                }
            }
        }
	}







    void OnTriggerEnter2D(Collider2D col)
    {

    }

    void resetPose(float xOffset = 0, float yOffset = 0)
    {
        transform.position += new Vector3(xOffset, yOffset);
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        transform.rotation = Quaternion.identity;
    }



    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
    }

    public void DisableMovementForDuration(float duration)
    {
        StartCoroutine(TimedDisable(duration));
    }

    public void DisableSingleSidedMovementForDuration(float duration, bool right)
    {
        StartCoroutine(TimedDisableSingleSided(duration, right));
    }

    //Disables movement and enables after 'duration' period of time
    IEnumerator TimedDisable(float duration)
    {
        SetMovementEnabled(false);
        yield return new WaitForSeconds(duration);
        SetMovementEnabled(true);
        yield return null;
    }

    IEnumerator TimedDisableSingleSided(float duration, bool right)
    {
        if(right == true)
        {
            rightMovementEnabled = false;
        }else
        {
            leftMovementEnabled = false;
        }
        yield return new WaitForSeconds(duration);

        if (right == true)
        {
            rightMovementEnabled = true;
        }
        else
        {
            leftMovementEnabled = true;
        }
        yield return null;
    }

    public void OnDeath(GameObject killer)
    {
        Debug.Log("You have died");
    }
}