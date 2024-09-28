using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;


public class CharacterControl : MonoBehaviour
{
    public bool isTargetDummy = false;

    public PlayerTypes PlayerID;

    [SerializeField] GameObject AtaHuHaAmongusPOVCamera;
    [SerializeField] GameObject OriginalCamera;
    bool onOff = false;

    public int hp = 10;

    RaycastHit pickupHit;
    [SerializeField] LayerMask pickupMask;

    [SerializeField] LayerMask collisionMask;
    Collider[] wallSearch;
    Collider[] projSearch;

    [SerializeField] float moveSpeed = 2;
    private float startingSpeed;
    private float accelSpeed = 0;
    private float deAccelSpeed = 0;

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
    [SerializeField] GameObject[] weaponList;

    [Foldout("Limb Animators")]
    [SerializeField] Animator lArmAnim;
    [SerializeField] Animator rArmAnim;
    [SerializeField] Animator lFootAnim;
    [SerializeField] Animator rFootAnim;
    [SerializeField] Transform rArm;
    [EndFoldout]


    float powerPunchWindup = 0;
    public enum Weapons
    {
        Fist,
        Gun,
        Sword,
        Boomerang,
        Lazer
    }
    public enum PlayerTypes
    {
        Red,
        Green,
        Blue,
        Yellow
    }

    private Weapons equippedWeapon;
    private Weapons previousWeapon;

    public static int weaponID;


    CharacterController CC;
    bool useWeapon;
    private Vector2 moveInput;


    void Start()
    {
        if (isTargetDummy)
            return;

        startingSpeed = moveSpeed;
        weaponID = 0;

        CC = GetComponent<CharacterController>();

        for (int i = 0; i < weaponList.Length; i++)
        {
            weaponList[i].GetComponent<WeaponBase>().playerID = PlayerID;
        }
    }

    public void Weapon(InputAction.CallbackContext context)
    {
        useWeapon = context.action.triggered;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (context.canceled)
        {
            moveInput = Vector2.zero;
        }
    }

    void Update()
    {

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
        projSearch = Physics.OverlapBox(transform.position, new Vector3(0.5f, 1f, 0.5f), Quaternion.identity, collisionMask);
        if (projSearch.Length > 0)
        {
            for (int i = 0; i < projSearch.Length; i++)
            {
                WeaponBase WB = projSearch[i].GetComponent<WeaponBase>();

                TakeDamage(WB.playerID, WB.damage, WB.damageType);
                if (projSearch[i].GetComponent<WeaponBase>().damageType == WeaponBase.damageTypes.destructableProjectile)
                {
                    Destroy(projSearch[i].gameObject);
                }
            }
        }


        if (isTargetDummy)
            return;

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


        if (moveInput != Vector2.zero)
        {
            lFootAnim.SetBool("Walk", true);
            rFootAnim.SetBool("Walk", true);

            if (moveSpeed < startingSpeed)
            {
                accelSpeed += Time.deltaTime * 5;
                deAccelSpeed = 0;
                moveSpeed += accelSpeed;
            }

            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            CC.Move(moveDirection * moveSpeed * Time.deltaTime);
            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);
        }

        else
        {
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





        if (useWeapon)
        {
            if (equippedWeapon != Weapons.Fist) //no punch if other weapon equipped
                Attack(equippedWeapon, previousWeapon);
            else
            {
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
                if (powerPunchWindup < 0.75)
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
                //rArm.localPosition = new Vector3(0.75f, 0, 2 * powerPunchWindup);
                rArm.localPosition = new Vector3(0.75f, 0, 0);
                rArmAnim.Play("StrongPunch");
                powerPunchWindup = 0;
            }
            else
            {
                rArm.localPosition = new Vector3(0.75f, 0, 0);
                powerPunchWindup = 0;
            }
        }


        if (Physics.Raycast(transform.position, transform.forward, out pickupHit, 1f, pickupMask))
        {
            if (Enum.TryParse<Weapons>(pickupHit.transform.name, out Weapons weapon))
            {
                previousWeapon = equippedWeapon;
                equippedWeapon = Enum.Parse<Weapons>(pickupHit.transform.name);
                pickupHit.transform.gameObject.SetActive(false);

                weaponList[(int)previousWeapon].SetActive(false);
                weaponList[(int)equippedWeapon].SetActive(true);
                weaponID = (int)equippedWeapon;
            }
            else
            {
                Debug.Log("Change the object's name to the correct weapon name");
            }
        } //pickup raycast


        /** // in case of collision check with raycast
        wallSearch = Physics.OverlapBox(transform.position, new Vector3(0.5f, 1f, 0.5f), Quaternion.identity, collisionMask);
        if (wallSearch.Length > 0)
        {
            for (int i = 0; i < wallSearch.Length; i++)
            {
                Debug.Log(wallSearch[i].name);
                Vector3 direction = wallSearch[i].transform.position - transform.position;
                direction.Normalize();
                direction *= 0.1f;
                Debug.Log(direction);
                transform.position = new Vector3(transform.position.x - direction.x, transform.position.y, transform.position.z - direction.z);
            }
        }
        */
    }

    private void TakeDamage(PlayerTypes attackingPlayer, int damage, WeaponBase.damageTypes damageType)
    {
        if (attackingPlayer != PlayerID)
        {
            hp = hp - damage;
            Debug.Log("Ouch!, Player " + attackingPlayer.ToString() + " hurt me!");
        }
    }

    private void Attack(Weapons WeaponUsed, Weapons UnusedWeapon)
    {
        //weaponList[(int)UnusedWeapon].SetActive(false);
        //weaponList[(int)WeaponUsed].SetActive(true);

        /*
        switch (WeaponUsed)
        {
            case Weapons.Fist:
                Debug.Log("Error, check 'Attack' function code");
                break;
            case Weapons.Gun:
                Debug.Log("Pistol!");
                break;
            case Weapons.Sword:
                Debug.Log("Shwinggggggggg!");
                break;
            default:
                // code block
                break;
        }
        */
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



        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
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
                    //transform.Translate((Vector3.forward - Vector3.right).normalized * moveSpeed * Time.deltaTime, Space.World);
                    CC.Move((Vector3.forward - Vector3.right).normalized * moveSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, 315, 0);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    //transform.Translate((Vector3.forward + Vector3.right).normalized * moveSpeed * Time.deltaTime, Space.World);
                    CC.Move((Vector3.forward + Vector3.right).normalized * moveSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, 45, 0);
                }
                else
                {
                    //transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    CC.Move(Vector3.forward * moveSpeed * Time.deltaTime);
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                if (Input.GetKey(KeyCode.W))
                {
                    //transform.Translate((Vector3.forward + Vector3.right).normalized * moveSpeed * Time.deltaTime, Space.World);
                    CC.Move((Vector3.forward + Vector3.right).normalized * moveSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, 45, 0);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    //transform.Translate((-Vector3.forward + Vector3.right).normalized * moveSpeed * Time.deltaTime, Space.World);
                    CC.Move((-Vector3.forward + Vector3.right).normalized * moveSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, 135, 0);
                }
                else
                {
                    //transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);
                    CC.Move(Vector3.right * moveSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                }
            }
            else if (Input.GetKey(KeyCode.S))
            {
                if (Input.GetKey(KeyCode.D))
                {
                    //transform.Translate((-Vector3.forward + Vector3.right).normalized * moveSpeed * Time.deltaTime, Space.World);
                    CC.Move((-Vector3.forward + Vector3.right).normalized * moveSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, 135, 0);
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    //transform.Translate((-Vector3.forward - Vector3.right).normalized * moveSpeed * Time.deltaTime, Space.World);
                    CC.Move((-Vector3.forward - Vector3.right).normalized * moveSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, 225, 0);
                }
                else
                {
                    //transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime, Space.World);
                    CC.Move(-Vector3.forward * moveSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }
            }
            else if (Input.GetKey(KeyCode.A))
            {
                if (Input.GetKey(KeyCode.S))
                {
                    //transform.Translate((-Vector3.forward - Vector3.right).normalized * moveSpeed * Time.deltaTime, Space.World);
                    CC.Move((-Vector3.forward - Vector3.right).normalized * moveSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, 225, 0);
                }
                else if (Input.GetKey(KeyCode.W))
                {
                    //transform.Translate((Vector3.forward - Vector3.right).normalized * moveSpeed * Time.deltaTime, Space.World);
                    CC.Move((Vector3.forward - Vector3.right).normalized * moveSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, 315, 0);
                }
                else
                {
                    //transform.Translate(-Vector3.right * moveSpeed * Time.deltaTime, Space.World);
                    CC.Move(-Vector3.right * moveSpeed * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, 270, 0);

                }
            }
        }
*/
