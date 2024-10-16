using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;

using UnityEngine.UI;
using TMPro;


public class CharacterControl : MonoBehaviour
{
    public bool isTargetDummy = false;

    public PlayerTypes PlayerID;
    public static PlayerTypes discardingPlayerID;

    //[SerializeField] GameObject AtaHuHaAmongusPOVCamera;
    //[SerializeField] GameObject OriginalCamera;
    //bool onOff = false;

    public int hp = 10;
    public int coins = 0;

    RaycastHit pickupHit;
    [SerializeField] LayerMask pickupMask;
    private float speedBuffTimer;
    private float shieldBuffTimer;

    [SerializeField] LayerMask collisionMask;
    Collider[] projSearch;
    float identicalDamageCD;
    private  PlayerTypes lastPlayerID;

    Vector3 hitBoxSize = new Vector3(1, 2f, 1);

    [SerializeField] float moveSpeed;
    private float startingSpeed;
    private float currentMaxSpeed;
    private float accelSpeed = 0;
    private float deAccelSpeed = 0;
    private Vector3 moveDirection;
    private float targetAngle;
    private Vector3 attackDirection;
    private float attackMoveSpeed;

    //private bool holdPos;
    private float holdTimer;

    //private int animState = 0;
    private AS animState;
    private float animTimer = 0;

    private enum AS //animestate
    {
        idle =           0,
        Punch1Windup =   1, 
        Punch1Active =   2,
        Punch1Recovery = 3,
        Punch2Windup =   4,
        Punch2Active =   5,
        Punch2Recovery = 6,
        Punch3Windup =   7,
        Punch3Active =   8,
        Punch3Recovery = 9,
        StrongPunch =    10
    }

    [SerializeField] GameObject[] weaponList;
    [SerializeField] GameObject rightArmGFX;

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
        Lazer,
        Mine
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

    private static bool weaponDiscarded = false;


    CharacterController CC;
    bool useWeapon;
    private Vector2 moveInput;


    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI moneyText;


    void Start()
    {
        weaponList[0].GetComponent<SphereCollider>().enabled = false;
        rightArmGFX.GetComponent<SphereCollider>().enabled = false;

        //if (isTargetDummy)
        //return;

        startingSpeed = moveSpeed;
        currentMaxSpeed = startingSpeed;
        weaponID = 0;

        CC = GetComponent<CharacterController>();

        rightArmGFX.GetComponent<WeaponBase>().playerID = PlayerID; //special treatment as he does not act like the other hand

        for (int i = 0; i < weaponList.Length; i++)
        {
            weaponList[i].GetComponent<WeaponBase>().playerID = PlayerID;
        }

        animState = AS.idle;
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

        identicalDamageCD -= Time.deltaTime;

        projSearch = Physics.OverlapBox(transform.position, hitBoxSize, Quaternion.identity, collisionMask);
        if (projSearch.Length > 0)
        {
            for (int i = 0; i < projSearch.Length; i++)
            {
                WeaponBase attackWB = projSearch[i].GetComponent<WeaponBase>();

                TakeDamage(attackWB.playerID, attackWB.damage, attackWB.damageType);

                if (projSearch[i].GetComponent<WeaponBase>().damageType == WeaponBase.damageTypes.destructableProjectile && attackWB.playerID != PlayerID)
                {
                    Destroy(projSearch[i].gameObject);
                }

            }
        }


        if (isTargetDummy)
            return;

        speedBuffTimer -= Time.deltaTime;
        shieldBuffTimer -= Time.deltaTime;

        /**
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
        */

        if (moveInput != Vector2.zero)
        {
            if (speedBuffTimer <= 0)
            {
                currentMaxSpeed = startingSpeed;
            }
            if (moveSpeed < currentMaxSpeed)
            {
                accelSpeed += Time.deltaTime * 5;
                deAccelSpeed = 0;
                moveSpeed += accelSpeed;
            }

            moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            if (holdTimer <= 0) //can still rotate while shooting
            {
                lFootAnim.SetBool("Walk", true);
                rFootAnim.SetBool("Walk", true);
                CC.Move(moveDirection * moveSpeed * Time.deltaTime);
            }
            else
            {
                lFootAnim.SetBool("Walk", false);
                rFootAnim.SetBool("Walk", false);
            }

            targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;

            if (animState == AS.idle || animState == AS.Punch1Recovery || animState == AS.Punch2Recovery || animState == AS.Punch3Recovery) //can't rotate whiling using fists
            {
                transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            }
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

        if (animState != 0) //movement from attacking
        {
            if (attackMoveSpeed>=0)
                CC.Move(attackDirection * attackMoveSpeed * Time.deltaTime);

            attackMoveSpeed -= Time.deltaTime * 50;
        }

        holdTimer -= Time.deltaTime;


        if (useWeapon)
        {
            rArm.localPosition = new Vector3(0.75f, 0, 0); //resets strong punch
            powerPunchWindup = 0;

            if (equippedWeapon == Weapons.Fist) //no punch if other weapon equipped //Attack(equippedWeapon, previousWeapon);
            {
                if (animState == AS.idle || animState == AS.Punch3Recovery)
                {
                    holdTimer = 0.383f; //can't move during attack windup & active, full animation is 0.75
                    attackDirection = moveDirection;
                    attackMoveSpeed = 20;

                    animTimer = 0;
                    animState = AS.Punch1Windup;
                    lArmAnim.Play("Punch1");
                }
                else if (animState == AS.Punch1Active || animState == AS.Punch1Recovery)
                {
                    holdTimer = 0.4166f; //can't move during attack windup & active, full animation is 0.75
                    attackDirection = moveDirection;
                    attackMoveSpeed = 20;

                    animTimer = 0;
                    animState = AS.Punch2Windup;
                    lArmAnim.Play("Punch2");
                }
                else if (animState == AS.Punch2Active || animState == AS.Punch2Recovery)
                {
                    holdTimer = 0.5166f; //can't move during attack windup & active, full animation is 0.75
                    attackDirection = moveDirection;
                    attackMoveSpeed = 20;

                    animTimer = 0;
                    animState = AS.Punch3Windup;
                    lArmAnim.Play("Punch3");
                }
            }
            else //using other weapons
            {
                holdTimer = 0.15f;  //consider using a per-weapon case stun where we check the stun duration and if there is one needed by the weapon's script
            }
        }
        if (animState != AS.idle)
        {
            animTimer += Time.deltaTime;

            SphereCollider fist = weaponList[0].GetComponent<SphereCollider>();

            if (animState == AS.Punch1Windup || animState == AS.Punch1Active || animState == AS.Punch1Recovery)
            {
                if (animTimer >= 0.383f) // 23/60
                {
                    animState = AS.Punch1Recovery;
                    fist.enabled = false;
                }
                else if (animTimer >= 0.3f) // 18/60 active
                {
                    animState = AS.Punch1Active;
                    fist.enabled = true;
                }
            }

            if (animState == AS.Punch2Windup || animState == AS.Punch2Active || animState == AS.Punch2Recovery)
            {
                if (animTimer >= 0.4166f) // 25/60
                {
                    animState = AS.Punch2Recovery;
                    fist.enabled = false;
                }
                else if (animTimer >= 0.333f) // 20/60 active
                {
                    animState = AS.Punch2Active;
                    fist.enabled = true;
                }
            }


            if (animState == AS.Punch3Windup || animState == AS.Punch3Active || animState == AS.Punch3Recovery)
            {
                if (animTimer >= 0.5166f) // 31/60
                {
                    animState = AS.Punch3Recovery;
                    fist.enabled = false;
                }
                else if (animTimer >= 0.233f) // 14/60 active
                {
                    animState = AS.Punch3Active;
                    fist.enabled = true;
                }
            }

            if (animTimer >= 0.75f) // 45/60
            {
                animState = 0; //idle
                animTimer = 0;
                lArmAnim.Play("Idle");
            }
        }

        if (Input.GetMouseButton(1) && holdTimer<=0)
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
                animState = AS.StrongPunch;
                holdTimer = 0.5f; //can't move during attack windup & active, full animation is 0.5
                attackDirection = moveDirection;
                attackMoveSpeed = 35;

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
                //pickupHit.transform.gameObject.SetActive(false); //consider destroying instead


                weaponList[(int)previousWeapon].SetActive(false);
                weaponList[(int)equippedWeapon].SetActive(true);
                weaponID = (int)equippedWeapon;

                Destroy(pickupHit.transform.gameObject);
                //Destroy(pickupHit.transform);
            }
            else if (pickupHit.transform.name == "Coin" || pickupHit.transform.name == "Speed" || pickupHit.transform.name == "Health" || pickupHit.transform.name == "Shield")
            {
                if (pickupHit.transform.name == "Coin")
                {
                    coins++;
                    moneyText.text = "$: " + coins;
                    Destroy(pickupHit.transform.gameObject);
                    //Debug.Log("coin");
                }
                if (pickupHit.transform.name == "Health")
                {
                    hp += 3;
                    hpText.text = "HP: " + hp;
                    Destroy(pickupHit.transform.gameObject);
                    //Debug.Log("health");
                }
                if (pickupHit.transform.name == "Speed")
                {
                    currentMaxSpeed = startingSpeed *1.5f;
                    speedBuffTimer = 5;
                    Destroy(pickupHit.transform.gameObject);
                    //Debug.Log("speed");
                }
                if (pickupHit.transform.name == "Shield")
                {
                    shieldBuffTimer = 3.5f;
                    Destroy(pickupHit.transform.gameObject);
                    //Debug.Log("shield");
                }
            }
            else
            {
                Debug.Log("Change the object's name to the correct weapon or pickup name");
            }
        } //pickup raycast

        if (weaponDiscarded && discardingPlayerID == PlayerID)
        {
            weaponDiscarded = false;
            previousWeapon = equippedWeapon;
            equippedWeapon = Weapons.Fist;

            weaponList[(int)previousWeapon].SetActive(false);
            weaponList[(int)equippedWeapon].SetActive(true);
            weaponID = (int)equippedWeapon;
            //Debug.Log("actually discarded yo");
        }


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
            if (!(attackingPlayer == lastPlayerID && 
                (damageType == WeaponBase.damageTypes.IndestructableProjectile || damageType == WeaponBase.damageTypes.melee) &&
                identicalDamageCD>=0)) //making sure player is not taking multiple instances of damage from the same attack
            {
                if (shieldBuffTimer>=0)
                {
                    hp = hp - damage;
                    hpText.text = ("HP: " + hp);
                }
                //Debug.Log("Ouch!, Player " + attackingPlayer.ToString() + " hurt me! I have +" + hp + " Hp!");
            }


            lastPlayerID = attackingPlayer;
            identicalDamageCD = 0.1f;
        }
    }

    /**
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
        
    }
    */
    public static void DiscardWeapon(PlayerTypes weaponPlayerID)
    {
        discardingPlayerID = weaponPlayerID;
        weaponDiscarded = true;
        //Debug.Log("weapon discarded");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, hitBoxSize);
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
