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


    public bool isInVent = false;

    public bool isStealthed = false;

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
       // DontDestroyOnLoad(gameObject);
        if (playerController != null)
        {
            Destroy(gameObject);
        }
        else
        {
            playerController = this;
        }


        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        playerRobot = GetComponent<Robot>();
        playerRC = GetComponent<RobotController>();
    }

    void Start()
    {

        player = GetComponent<Player>();

        playerRobot.robotDiedEvent.AddListener(OnDeath);
    }


	
	// Update is called once per frame
	void Update () {

        if (movementEnabled)
        {

            
            if (Input.GetKey(KeyCode.D))
            {
                if (rightMovementEnabled)
                {
                    playerRC.MoveHorizontal(playerRobot.speed);
                    LeaveStealth();
                }

            }
            else if (Input.GetKey(KeyCode.A))
            {
                if (leftMovementEnabled)
                {
                    playerRC.MoveHorizontal(-1 * playerRobot.speed);
                    LeaveStealth();
                }


            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerRC.Jump();
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            //playerRobot.headLampActive = !playerRobot.headLampActive;
            if (playerRobot.headLamp.headLampOn)
            {
                playerRobot.headLamp.TurnOff(true);
            }
            else
            {
                playerRobot.headLamp.TurnOn(true);
            }

            if(playerRobot.headLamp.headLampOn == true)
            {
                LeaveStealth();
            }
            //playerRC.SetHeadlightOn(true);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {

            Vector2 mousePosition = PlayerController.GetMouseInWorldSpace();

            playerRC.FireRightArm(mousePosition);

            
            LeaveStealth();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            playerRC.Punch();
            LeaveStealth();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
           // ScriptableSequence.PlaySequenceByName("IntroSequence");
        }


        //Allows players to drop body parts
        if (Input.GetKeyDown(KeyCode.T))
        {
            if(player.GetComponent<Robot>().getNumberOfParts() > 0)
                player.GetComponent<Robot>().takeDamage(1, gameObject, false);
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            if(GetComponent<Rigidbody2D>().velocity.magnitude <= 1f && playerRC.state == RobotState.OnGround)
            {
                EnterStealth();
                if (playerRobot.GetComponentInChildren<RobotHeadLamp>())
                {
                    playerRobot.GetComponentInChildren<RobotHeadLamp>().TurnOff();
                }
            }
        }


        //Looting body parts
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider2D[] colls;
            if(playerRobot.legs != null)
            {

                Vector2 pos = new Vector2(transform.position.x + 0.5f * transform.lossyScale.x, transform.position.y);
                colls = Physics2D.OverlapBoxAll(pos, new Vector2(2f, 2.5f), 0);

            }else
            {
                Vector2 pos = new Vector2(transform.position.x, transform.position.y);
                colls = Physics2D.OverlapBoxAll(pos, new Vector2(3, 2.5f), 0);

            }


            foreach(Collider2D col in colls)
            {
                if (col.gameObject.GetComponent<BodyPart>())
                {
                    bool success = PlayerController.player.GetComponent<Robot>().EquipBodyPart(col.gameObject.GetComponent<BodyPart>());

                    if (success == true)
                        break;
                }
            }
        }
	}




    public void EnterStealth()
    {
        isStealthed = true;
        gameObject.layer = LayerMask.NameToLayer("Stealth");
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            var color = sr.color;
            color.a = 0.5f;
            sr.color = color;
        }
       

    }

    public void LeaveStealth()
    {
        isStealthed = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            var color = sr.color;
            color.a = 1.0f;
            sr.color = color;
        }

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
    }
}
