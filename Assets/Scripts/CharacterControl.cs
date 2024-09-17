using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    [SerializeField] GameObject AtaHuHaAmongusPOVCamera;
    [SerializeField] GameObject OriginalCamera;
    bool onOff = false;

    [SerializeField] float moveSpeed = 2;
    private float startingSpeed;
    private float accelSpeed = 0;
    private float deAccelSpeed = 0;

    [SerializeField] Animator lArmAnim;
    private int animState = 0;
    private float animTimer = 0;
    //0=idle
    //1=punch1 windup
    //2=punch1 active frames
    //3=punch1 recovery
    //4=punch2 windup
    //5=punch2 active frames
    //6=punch2 recovery
    //7=punch3 windup
    //8=punch3 active frames
    //9=punch3 recovery

    [SerializeField] Animator lFootAnim;
    [SerializeField] Animator rFootAnim;

    [SerializeField] Transform rArm;
    float powerPunchWindup = 0;
    void Start()
    {
        startingSpeed = moveSpeed;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (onOff)
            {
                AtaHuHaAmongusPOVCamera.SetActive(true);
                OriginalCamera.SetActive(false);
            }
            else
            {
                AtaHuHaAmongusPOVCamera.SetActive(false);
                OriginalCamera.SetActive(true);
            }
            onOff = !onOff;
        }


        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            //anim.SetBool("Moving", true);
            lFootAnim.SetBool("Walk", true);
            rFootAnim.SetBool("Walk", true);

            if (moveSpeed < startingSpeed)
            {
                accelSpeed += Time.deltaTime * 5;
                deAccelSpeed = 0;
                moveSpeed += accelSpeed;
            }

            if (Input.GetKey(KeyCode.W))
            {
                if (Input.GetKey(KeyCode.A))
                {
                    transform.Translate((Vector3.forward - Vector3.right).normalized * moveSpeed * Time.deltaTime, Space.World);
                    transform.rotation = Quaternion.Euler(0, 315, 0);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    transform.Translate((Vector3.forward + Vector3.right).normalized * moveSpeed * Time.deltaTime, Space.World);
                    transform.rotation = Quaternion.Euler(0, 45, 0);
                }
                else
                {
                    transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                if (Input.GetKey(KeyCode.W))
                {
                    transform.Translate((Vector3.forward + Vector3.right).normalized * moveSpeed * Time.deltaTime, Space.World);
                    transform.rotation = Quaternion.Euler(0, 45, 0);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    transform.Translate((-Vector3.forward + Vector3.right).normalized * moveSpeed * Time.deltaTime, Space.World);
                    transform.rotation = Quaternion.Euler(0, 135, 0);
                }
                else
                {
                    transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                }
            }
            else if (Input.GetKey(KeyCode.S))
            {
                if (Input.GetKey(KeyCode.D))
                {
                    transform.Translate((-Vector3.forward + Vector3.right).normalized * moveSpeed * Time.deltaTime, Space.World);
                    transform.rotation = Quaternion.Euler(0, 135, 0);
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    transform.Translate((-Vector3.forward - Vector3.right).normalized * moveSpeed * Time.deltaTime, Space.World);
                    transform.rotation = Quaternion.Euler(0, 225, 0);
                }
                else
                {
                    transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime, Space.World);
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }
            }
            else if (Input.GetKey(KeyCode.A))
            {
                if (Input.GetKey(KeyCode.S))
                {
                    transform.Translate((-Vector3.forward - Vector3.right).normalized * moveSpeed * Time.deltaTime, Space.World);
                    transform.rotation = Quaternion.Euler(0, 225, 0);
                }
                else if (Input.GetKey(KeyCode.W))
                {
                    transform.Translate((Vector3.forward - Vector3.right).normalized * moveSpeed * Time.deltaTime, Space.World);
                    transform.rotation = Quaternion.Euler(0, 315, 0);
                }
                else
                {
                    transform.Translate(-Vector3.right * moveSpeed * Time.deltaTime, Space.World);
                    transform.rotation = Quaternion.Euler(0, 270, 0);

                }
            }
        }
        else
        {
            //anim.SetBool("Moving", false);
            lFootAnim.SetBool("Walk", false);
            rFootAnim.SetBool("Walk", false);

            if (moveSpeed > 1)
            {
                accelSpeed = 0;
                deAccelSpeed += Time.deltaTime * 5;
                moveSpeed -= deAccelSpeed;

                if (moveSpeed < 1)
                    moveSpeed = 1;
            }
        }





        if (Input.GetMouseButtonDown(0))
        {
            Attack(0); //random number until implemented

            if (animState == 0 || animState == 9)
            {
                animTimer = 0;
                animState = 1;
                lArmAnim.Play("Punch1");
            }
            else if (animState == 2 || animState == 3)
            {
                animTimer = 0;
                animState = 4;
                lArmAnim.Play("Punch2");
            }
            else if (animState == 5 || animState == 6)
            {
                animTimer = 0;
                animState = 7;
                lArmAnim.Play("Punch3");
            }
        }
        if (animState != 0)
        {
            animTimer += Time.deltaTime;

            if (animState == 1 || animState == 2 || animState == 3)
            {
                if (animTimer >= 0.383f) // 23/60
                {
                    animState = 3;
                }
                else if (animTimer >= 0.3f) // 18/60
                {
                    animState = 2;
                }
            }

            if (animState == 4 || animState == 5 || animState == 6)
            {
                if (animTimer >= 0.4166f) // 25/60
                {
                    animState = 6;
                }
                else if (animTimer >= 0.333f) // 20/60
                {
                    animState = 5;
                }
            }


            if (animState == 7 || animState == 8 || animState == 9)
            {
                if (animTimer >= 0.5166f) // 31/60
                {
                    animState = 9;
                }
                else if (animTimer >= 0.233f) // 14/60
                {
                    animState = 8;
                }
            }

            if (animTimer >= 0.75f) // 45/60
            {
                animState = 0; //idle
                animTimer = 0;
                lArmAnim.Play("Idle");
            }
        }

        if (Input.GetMouseButton(1))
        {
            if (animState == 0)
            {
                if (powerPunchWindup<0.75)
                {
                    powerPunchWindup += Time.deltaTime;
                    rArm.localPosition = new Vector3(0.75f, 0, -powerPunchWindup);
                }
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (powerPunchWindup >= 0.75)
            {
                rArm.localPosition = new Vector3(0.75f, 0, 2 * powerPunchWindup);
                powerPunchWindup = 0;
            }
            else
            {
                rArm.localPosition = new Vector3(0.75f, 0, 0);
                powerPunchWindup = 0;
            }
        }


        ///add raycast code to check pickups, we just need a weapon index, maybe do with with enums so we'll know what the weapons are
    }

    private void Attack(int WeaponUsed)
    {
        ///add code for each weapon usage
    }
}



/**
if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
{
    if (moveSpeed < startingSpeed)
    {
        accelSpeed += Time.deltaTime*5;
        deAccelSpeed = 0;
        moveSpeed += accelSpeed;
    }
    if (Input.GetKey(KeyCode.W))
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    else if (Input.GetKey(KeyCode.S))
    {
        transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime, Space.World);
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }
    else if (Input.GetKey(KeyCode.D))
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);
        transform.rotation = Quaternion.Euler(0, 90, 0);
    }
    else if (Input.GetKey(KeyCode.A))
    {
        transform.Translate(-Vector3.right * moveSpeed * Time.deltaTime, Space.World);
        transform.rotation = Quaternion.Euler(0, 270, 0);
    }
}
else
{
    if (moveSpeed > 1)
    {
        accelSpeed = 0;
        deAccelSpeed += Time.deltaTime * 5;
        moveSpeed -= deAccelSpeed;

        if (moveSpeed < 1)
            moveSpeed = 1;
    }
}
*/
