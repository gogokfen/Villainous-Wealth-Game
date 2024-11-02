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
    //public static PlayerTypes holdingPlayerID;

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
    private PlayerTypes lastPlayerID;

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
    private float forwardMomentumDelay;

    //private bool holdPos;
    private float holdTimer;
    //public static float staticHoldTimer;
    private bool rolling;
    private Vector3 rollDirection;
    private float rollCD;
    private float rollTimer;

    //private int animState = 0;
    private AS animState;
    private float animTimer = 0;

    private enum AS //animestate
    {
        idle = 0,
        Punch1Windup = 1,
        Punch1Active = 2,
        Punch1Recovery = 3,
        Punch2Windup = 4,
        Punch2Active = 5,
        Punch2Recovery = 6,
        Punch3Windup = 7,
        Punch3Active = 8,
        Punch3Recovery = 9,
        StrongPunch = 10
    }

    [SerializeField] GameObject[] weaponList;
    [SerializeField] GameObject rightArmGFX;

    [Foldout("Limb Animators")]
    [SerializeField] Animator lArmAnim;
    [SerializeField] Animator rArmAnim;
    [SerializeField] Animator lFootAnim;
    [SerializeField] Animator rFootAnim;
    [SerializeField] Transform rArm;



    float powerPunchWindup = 0;
    public enum Weapons
    {
        Fist,
        Gun,
        Sword,
        Boomerang,
        Lazer,
        Mine,
        Blunderbuss,
        Grenade
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
    bool rollInput;
    bool shieldInput;
    int rightPunchAttackState;
    public bool dead;
    private Vector2 moveInput;

    [EndFoldout]

    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] GameObject shieldGFX;
    [SerializeField] GameObject blockBubble;
    private float blockDuration;
    private float blockCD;

    [SerializeField] Slider windUpBar;
    float reloadTime;

    [SerializeField] ParticleSystem meleeParticleEffect;
    [SerializeField] GameObject characterGFX;

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

        useWeapon = false;

        hpText.text = ("HP: " + hp);
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

    public void Roll(InputAction.CallbackContext context)
    {
        rollInput = context.action.triggered;
    }
    public void Shield(InputAction.CallbackContext context)
    {
        shieldInput = context.action.triggered;
    }

    public void RightPunch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            rightPunchAttackState = 1;
        }
        else if (context.canceled)
        {
            if (rightPunchAttackState == 1)
                rightPunchAttackState = -1;
            else
                rightPunchAttackState = 0;
        }
    }

    public void OutTheRound()
    {
        PickupManager.singleton.SpawnTreasureChestCoin(transform);
        CC.enabled = false;
        characterGFX.SetActive(false);
        dead = true;

        PlayerManager.PlayerCheck();
    }
    public void NextRound()
    {
        hp = 10;
        hpText.text = "HP: " + hp;
        CC.enabled = true;
        characterGFX.SetActive(true);
        dead = false;
    }

    void Update()
    {
        moneyText.text = "$: " + coins; //delete

        if (hp <= 0 & !dead)
        {
            //PickupManager.singleton.SpawnTreasureChestCoin(transform);
            //Destroy(gameObject);
            OutTheRound();
        }

        identicalDamageCD -= Time.deltaTime;

        projSearch = Physics.OverlapBox(transform.position, hitBoxSize, Quaternion.identity, collisionMask);
        if (projSearch.Length > 0)
        {
            for (int i = 0; i < projSearch.Length; i++)
            {
                WeaponBase attackWB = projSearch[i].GetComponent<WeaponBase>();

                if (projSearch[i].GetComponent<WeaponBase>().damageType == WeaponBase.damageTypes.grenade)
                    TakeDamage(attackWB.playerID, attackWB.damage, projSearch[i].transform.position);
                else
                    TakeDamage(attackWB.playerID, attackWB.damage, attackWB.damageType, projSearch[i].transform.position);



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
        if (shieldBuffTimer <= 0)
            shieldGFX.SetActive(false);


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

            if ((animState == AS.idle || animState == AS.Punch1Recovery || animState == AS.Punch2Recovery || animState == AS.Punch3Recovery) && !rolling) //can't rotate whiling using fists
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

        if (shieldInput && blockCD <= 0)
        {
            blockBubble.SetActive(true);
            blockDuration = 0.75f;
            blockCD = 3.5f;
            holdTimer = 0.75f;
        }

        rollCD -= Time.deltaTime;

        if (rollInput && rollCD <= 0 && (animState == AS.idle || animState == AS.Punch1Recovery || animState == AS.Punch2Recovery || animState == AS.Punch3Recovery))
        {
            rollDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            if (rollDirection == Vector3.zero)
            {
                rollDirection = transform.forward;
            }
            rollTimer = 0.35f;
            rolling = true;
            rollCD = 2.5f;
        }
       

        if (rolling)
        {
            rollTimer -= Time.deltaTime;

            transform.Rotate(Vector3.right, Time.deltaTime * 1030);

            float rollingSpeed = (startingSpeed * 4 - (startingSpeed * ((1 - (rollTimer * 2.75f)) * 4)));
            CC.Move(rollDirection * rollingSpeed * Time.deltaTime);
            if (rollTimer <= 0)
            {
                rolling = false;
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        }


        blockCD -= Time.deltaTime;
        if (blockDuration > 0)
        {
            blockDuration -= Time.deltaTime;
            if (blockDuration <= 0)
                blockBubble.SetActive(false);
        }

        if (animState != 0) //movement from attacking (forward momentum)
        {
            forwardMomentumDelay -= Time.deltaTime;
            if (forwardMomentumDelay <= 0)
            {
                if (attackMoveSpeed >= 0)
                    CC.Move(attackDirection * attackMoveSpeed * Time.deltaTime);

                attackMoveSpeed -= Time.deltaTime * 50;
            }

        }

        holdTimer -= Time.deltaTime;
        windUpBar.value = Mathf.InverseLerp(reloadTime, 0, holdTimer);
        if (holdTimer <= 0)
            windUpBar.gameObject.SetActive(false);


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
                    attackMoveSpeed = 16;
                    forwardMomentumDelay = 0.133f; // 8/60

                    animTimer = 0;
                    animState = AS.Punch1Windup;
                    lArmAnim.Play("Punch1");
                }
                else if (animState == AS.Punch1Active || animState == AS.Punch1Recovery)
                {
                    holdTimer = 0.4166f; //can't move during attack windup & active, full animation is 0.75
                    attackDirection = moveDirection;
                    attackMoveSpeed = 16;
                    forwardMomentumDelay = 0.233f; // 14/60

                    animTimer = 0;
                    animState = AS.Punch2Windup;
                    lArmAnim.Play("Punch2");
                }
                else if (animState == AS.Punch2Active || animState == AS.Punch2Recovery)
                {
                    holdTimer = 0.5166f; //can't move during attack windup & active, full animation is 0.75
                    attackDirection = moveDirection;
                    attackMoveSpeed = 16;
                    forwardMomentumDelay = 0.233f; // 14/60

                    animTimer = 0;
                    animState = AS.Punch3Windup;
                    lArmAnim.Play("Punch3");
                }
            }
            else //using other weapons
            {
                Attack(equippedWeapon);
                //holdTimer = 0.15f;  //consider using a per-weapon case stun where we check the stun duration and if there is one needed by the weapon's script
                /*
                if (holdingPlayerID == PlayerID)
                {
                    holdTimer = staticHoldTimer;
                }
                else
                    holdTimer = 0.15f;
                */
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

        if (rightPunchAttackState == 1 && equippedWeapon == Weapons.Fist && holdTimer <= 0)
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

        if (rightPunchAttackState == -1 && equippedWeapon == Weapons.Fist)
        {
            rightPunchAttackState = 0;

            if (powerPunchWindup >= 0.75)
            {
                animState = AS.StrongPunch;
                holdTimer = 0.5f; //can't move during attack windup & active, full animation is 0.5
                attackDirection = moveDirection;
                attackMoveSpeed = 22;

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
                    //Destroy(pickupHit.transform.gameObject); //need to think this through with the pickup manager
                    pickupHit.transform.gameObject.SetActive(false);
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
                    currentMaxSpeed = startingSpeed * 1.5f;
                    speedBuffTimer = 5;
                    Destroy(pickupHit.transform.gameObject);
                    //Debug.Log("speed");
                }
                if (pickupHit.transform.name == "Shield")
                {
                    shieldGFX.SetActive(true);
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
            Debug.Log("actually discarded yo");
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

    private void TakeDamage(PlayerTypes attackingPlayer, int damage, WeaponBase.damageTypes damageType, Vector3 hitPos)
    {
        if (attackingPlayer != PlayerID)
        {

            if (!(attackingPlayer == lastPlayerID &&
                (damageType == WeaponBase.damageTypes.indestructableProjectile || damageType == WeaponBase.damageTypes.melee || damageType == WeaponBase.damageTypes.bounceOffProjectile) &&
                identicalDamageCD >= 0)) //making sure player is not taking multiple instances of damage from the same attack
            {
                if (shieldBuffTimer <= 0)
                {
                    hp = hp - damage;
                    hpText.text = ("HP: " + hp);



                    //--------------------test-----------------------

                    if (damageType == WeaponBase.damageTypes.melee)
                    {

                        //meleeParticleEffect.gameObject.SetActive(true);
                        meleeParticleEffect.Play();
                        meleeParticleEffect.transform.position = hitPos;
                    }

                }
                //Debug.Log("Ouch!, Player " + attackingPlayer.ToString() + " hurt me! I have +" + hp + " Hp!");
            }


            lastPlayerID = attackingPlayer;
            identicalDamageCD = 0.1f;
        }
    }

    private void TakeDamage(PlayerTypes attackingPlayer, int damage, Vector3 grenadePos) // in case of grenade
    {
        if (attackingPlayer != PlayerID)
        {
            if (shieldBuffTimer <= 0) //ranges from about 1 to 6
            {
                int damageBasedOnDistance;
                if (Vector3.Distance(transform.position, grenadePos) > 4.5f)
                {
                    damageBasedOnDistance = (int)(0.35 * damage);
                }
                else if ((Vector3.Distance(transform.position, grenadePos) > 2.0f))
                {
                    damageBasedOnDistance = (int)(0.7 * damage);
                }
                else
                    damageBasedOnDistance = damage;

                hp = hp - damageBasedOnDistance;
                hpText.text = ("HP: " + hp);
            }

            lastPlayerID = attackingPlayer;
            identicalDamageCD = 0.1f;
        }
    }

    private void Attack(Weapons WeaponUsed)
    {
        //holdTimer = 0.15f; //default case if no change
        switch (WeaponUsed)
        {
            case Weapons.Fist:
                //Debug.Log("0");
                break;
            case Weapons.Gun:
                //Debug.Log("1");
                holdTimer = 0.15f;
                break;
            case Weapons.Sword:
                //Debug.Log("2");
                break;
            case Weapons.Boomerang:
                //Debug.Log("3");
                holdTimer = 0.2f;
                break;
            case Weapons.Lazer:
                //Debug.Log("4");
                holdTimer = 0.15f;
                break;
            case Weapons.Mine:
                //Debug.Log("5");
                holdTimer = 0.25f;
                break;
            case Weapons.Blunderbuss:
                //Debug.Log("6");
                if (weaponList[(int)Weapons.Blunderbuss].GetComponent<Blunderbuss>().shoot)
                {
                    weaponList[(int)Weapons.Blunderbuss].GetComponent<Blunderbuss>().shoot = false;
                    animState = AS.StrongPunch;
                    holdTimer = weaponList[(int)Weapons.Blunderbuss].GetComponent<Blunderbuss>().holdTime;
                    attackDirection = -moveDirection;
                    attackMoveSpeed = 10;
                }
                else if (weaponList[(int)Weapons.Blunderbuss].GetComponent<Blunderbuss>().reloading)
                {
                    weaponList[(int)Weapons.Blunderbuss].GetComponent<Blunderbuss>().reloading = false;
                    holdTimer = weaponList[(int)Weapons.Blunderbuss].GetComponent<Blunderbuss>().holdTime * 2;

                    windUpBar.gameObject.SetActive(true);
                    reloadTime = holdTimer;
                }
                break;
            case Weapons.Grenade:
                //Debug.Log("7");
                holdTimer = 0.25f;
                break;
            default:
                Debug.Log("no");
                break;
        }

    }

    public void BuyWeapon(string shopWeaponName)
    {
        if (Enum.TryParse<Weapons>(shopWeaponName, out Weapons weapon))
        {
            previousWeapon = equippedWeapon;
            equippedWeapon = Enum.Parse<Weapons>(shopWeaponName);


            weaponList[(int)previousWeapon].SetActive(false);
            weaponList[(int)equippedWeapon].SetActive(true);
            weaponID = (int)equippedWeapon;
        }
        else
        {
            Debug.Log("Change the object's name to the correct weapon");
        }
    }

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
public static void Hold(float holdTime, PlayerTypes holdingPlayer)
{
    holdingPlayerID = holdingPlayer;
    staticHoldTimer = holdTime;
}
*/

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

/**
if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
{
    if (moveSpeed < startingSpeed)
    {
        accelSpeed += Time.deltaTime * 5;
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
