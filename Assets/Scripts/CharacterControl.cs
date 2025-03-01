using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;

using UnityEngine.UI;
using TMPro;
using MelenitasDev.SoundsGood;


public class CharacterControl : MonoBehaviour
{
    public bool isTargetDummy = false;
    public bool cameraManagerIsOn = false; //delete this on final build
    private bool mouseMovement = false;

    [HideInInspector] public bool zoneImmunity = false;
    [HideInInspector] public int zoneTicksGraceAmount = 2;

    public PlayerTypes PlayerID;

    public int hp = 10;

    private int coinsAtRoundStart;

    private Collider[] pickupSearch;
    private LayerMask pickupMask;
    private float speedBuffTimer;
    private float shieldBuffTimer;

    private Collider[] characterSearch;
    private LayerMask characterMask;

    private LayerMask collisionMask;
    private Collider[] projSearch;
    private float identicalDamageCD;
    private PlayerTypes lastPlayerID;
    private float timeSinceLastPlayerHit; //checking if players intentionally dying to zone

    private Vector3 hitBoxSize = new Vector3(1, 2f, 1);

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

    private float holdTimer;
    private float slowdownTimer;
    private bool rolling;
    private Vector3 rollDirection;
    private float rollCD;
    private float rollTimer;

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
        StrongPunch = 10,
        Sword1Windup = 11,
        Sword1Active = 12,
        Sword1Recovery = 13,
        Sword2Windup = 14,
        Sword2Active = 15,
        Sword2Recovery = 16
    }

    [SerializeField] GameObject[] weaponList;
    [SerializeField] GameObject rightArmGFX;
    private bool weaponSwap;
    private bool strongPunchSwap;
    private Weapons originalWeapon;

    private SphereCollider rFist;
    [SerializeField] SphereCollider lFist;
    [SerializeField] SphereCollider strongFist;

    public Animator charAnim;

    private float powerPunchWindup = 0;
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

    private CharacterController CC;
    private bool useWeapon;
    private bool rollInput;
    private int rightPunchAttackState;
    [HideInInspector] public bool dead;
    private Vector2 moveInput;

    //maybe add one stamina bar
    [SerializeField] Image hpBar;
    [SerializeField] Image emptyHpBar;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] Animator moneyAnim;
    [SerializeField] GameObject shieldGFX;
    [SerializeField] GameObject blockBubble;

    [SerializeField] Slider windUpBar;
    [SerializeField] Slider reloadBar;
    float reloadTime;

    [Foldout("VFX")]
    [SerializeField] ParticleSystem meleeParticleEffect;
    [SerializeField] ParticleSystem speedBuffEffect;
    [SerializeField] ParticleSystem healthBuffEffect;
    [SerializeField] ParticleSystem leaderGlow;
    [SerializeField] ParticleSystem strongPunchCharged;
    [SerializeField] Animator strongPunchPulse;
    [SerializeField] GameObject leaderCrown;
    [EndFoldout]

    public GameObject characterGFX;

    public GameObject[] bodyPartsGFX;

    public GameObject HeadGFX;
    private Color32 headColor = Color.grey;
    private Color startingColor;

    private bool paintHead = false;
    private float paintAmount;
    private Vector2 knockbackDirection;

    [SerializeField] GameObject playerIndicator;

    float rumbleTimer;

    [HideInInspector] public bool winner;

    private PlayerInput PI;

    private bool keyboardMouse = false;
    [SerializeField] GameObject mouseIndicator;
    [SerializeField] GameObject weaponScripts;
    private bool chargeSFX;

    private void Awake()
    {
        PI = GetComponent<PlayerInput>();
        CC = GetComponent<CharacterController>();

    }

    void Start()
    {
        pickupMask = 64;     //pickup layer
        collisionMask = 128; //projectile layer
        characterMask = 256; //character layer

        if (PI.currentControlScheme == "Keyboard & Mouse")
        {
            keyboardMouse = true;
            mouseMovement = true;

            Mouse.current.WarpCursorPosition(Camera.main.WorldToScreenPoint(transform.position) + (transform.forward * 0.25f));
            mouseIndicator.gameObject.SetActive(true);
        }
        rFist = weaponList[0].GetComponent<SphereCollider>();
        lFist.GetComponent<WeaponBase>().playerID = PlayerID;
        strongFist.GetComponent<WeaponBase>().playerID = PlayerID;
        startingColor = HeadGFX.GetComponent<Renderer>().material.color;
        for (int i = 0; i < weaponList.Length; i++)
        {
            weaponList[i].GetComponent<WeaponBase>().playerID = PlayerID;
        }

        if (PlayerID == PlayerTypes.Red)
        {
            playerIndicator.GetComponent<Image>().color = new Color32(204, 67, 152, 255);
            mouseIndicator.GetComponent<Image>().color = new Color32(204, 67, 152, 255);
        }
        else if (PlayerID == PlayerTypes.Green)
        {
            playerIndicator.GetComponent<Image>().color = new Color32(96, 196, 71, 255);
            mouseIndicator.GetComponent<Image>().color = new Color32(96, 196, 71, 255);
        }
        else if (PlayerID == PlayerTypes.Blue)
        {
            playerIndicator.GetComponent<Image>().color = new Color32(54, 111, 218, 255);
            mouseIndicator.GetComponent<Image>().color = new Color32(54, 111, 218, 255);
        }
        else if (PlayerID == PlayerTypes.Yellow)
        {
            playerIndicator.GetComponent<Image>().color = new Color32(51, 189, 190, 255);
            mouseIndicator.GetComponent<Image>().color = new Color32(51, 189, 190, 255);
        }

        startingSpeed = moveSpeed;
        currentMaxSpeed = startingSpeed;

        animState = AS.idle;

        useWeapon = false;

        hpBar.fillAmount = 1f;

        charAnim.writeDefaultValuesOnDisable = true;
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PauseMenu.instance.OnPause(PI);
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

    public void Roll(InputAction.CallbackContext context)
    {
        rollInput = context.action.triggered;
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
        int moneylost = Leaderboard.singleton.DeathMoney(PlayerID);
        moneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString() + "-" + moneylost;
        moneyAnim.Play("Player Coin Pickup", -1, 0);

        PickupManager.singleton.SpawnDeadCharacterCoin(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), moneylost);

        PickupManager.singleton.SpawnPowerUp(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z));

        CC.enabled = false;

        weaponList[(int)equippedWeapon].SetActive(false); //making sure he can't attack while dead kek

        charAnim.Play("Death");

        DeadStop(); //making sure character is dead

        dead = true;

        bodyPartsGFX[6].SetActive(false);
        bodyPartsGFX[7].SetActive(false);

        SoundManager.singleton.PlayClip($"{HeadGFX.name}Death", transform.position, 1f, true, true);

        PlayerManager.instance.PlayerCheck();
        if (cameraManagerIsOn)
            CameraManager.instance.RemoveFromCameraGroup(gameObject);
        TimeManager.instance.SlowTime(0.4f, 1f);
    }
    public void NextRound()
    {
        hp = 10;
        hpBar.fillAmount = 1f;
        CC.enabled = true;
        characterGFX.SetActive(true);
        dead = false;
        moneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString();
        coinsAtRoundStart = Leaderboard.singleton.GetMoney(PlayerID);
        Leaderboard.singleton.UpdateRoundMoney(PlayerID, coinsAtRoundStart);

        weaponList[(int)equippedWeapon].SetActive(true); //making sure he can't attack while dead kek

        DeadStop(); //making sure character was dead
        bodyPartsGFX[6].SetActive(true);
        bodyPartsGFX[7].SetActive(true);

        charAnim.Play("Idle");
        winner = false;
    }

    public void WarmupRound()
    {
        hp = 9999;
        hpBar.fillAmount = 1f;
        CC.enabled = true;
        characterGFX.SetActive(true);
        dead = false;

        weaponList[(int)equippedWeapon].SetActive(true); //making sure he can't attack while dead kek

        //DeadStop(); //making sure character was dead
        bodyPartsGFX[6].SetActive(true);
        bodyPartsGFX[7].SetActive(false); //removing the hp bar from the warmup round, characters can't die

        charAnim.Play("Idle");
        winner = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            if (mouseMovement)
                mouseIndicator.gameObject.SetActive(false);
            else
                mouseIndicator.gameObject.SetActive(true);

            mouseMovement = !mouseMovement;
        }

        if (winner)
        {
            charAnim.Play("Victory");
            winner = false;
        }

        if (transform.position.y != 0)
        {
            CC.enabled = false;
            transform.position = new Vector3(transform.position.x, 0, transform.position.z); //making sure not climbing anything
            CC.enabled = true;
        }

        if (dead)  //dying only turns out visuals, making sure no unwanted actions happen
            return;

        if (identicalDamageCD>=0)
            identicalDamageCD -= Time.deltaTime;
        if (timeSinceLastPlayerHit<=5)
            timeSinceLastPlayerHit += Time.deltaTime;

        if (mouseMovement) //360 aiming with mouse
        {
            //V1
            /*
            Vector3 mouseCharLock = Camera.main.WorldToScreenPoint(transform.position); //moving the mouse to character's position
            Mouse.current.WarpCursorPosition(new Vector2(mouseCharLock.x, mouseCharLock.y-1f)); //prevent character from always going up
            */

            LayerMask LM = 1024; //floor layer
            Ray raycastFromMouse = Camera.main.ScreenPointToRay(Input.mousePosition); //because it's a raycast, it hits invisble colliders
            if (Physics.Raycast(raycastFromMouse, out RaycastHit raycastHit, 500, LM))
            {
            }
            raycastHit.point = new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z); //preventing the char from unwanted rotation;

            Vector3 mouseCharLock = Camera.main.WorldToScreenPoint(transform.position);

            //V4 +V3
            float distance = Vector3.Distance(raycastHit.point, transform.position);

            float restrictedDis = Mathf.InverseLerp(0, 10, distance);

            mouseIndicator.transform.localPosition = Vector3.right * restrictedDis;

            /*
            Vector2 charMousePos = new Vector2(mouseCharLock.x, mouseCharLock.y);
            Vector2 dir = charMousePos - Mouse.current.position.value;
            dir.Normalize();
            dir *= 15;

            if (distance > 2.75)
            {
                Mouse.current.WarpCursorPosition(Mouse.current.position.value + dir);
            }
            else if (distance < 2.75)
            {
                Mouse.current.WarpCursorPosition(Mouse.current.position.value - dir);
            }
            
            //Mouse.current.WarpCursorPosition(Vector2.Lerp(Mouse.current.position.value, new Vector2(mouseCharLock.x, mouseCharLock.y), 0.01f));
            */

            if (!rolling)
            {
                if (distance > 0.2f) //0.2f
                    transform.LookAt(raycastHit.point);
                //V3 
                else
                {
                    //Vector3 mouseCharLock = Camera.main.WorldToScreenPoint(transform.position);
                    Mouse.current.WarpCursorPosition(mouseCharLock + (transform.forward * 0.25f));
                }

            }


            //V2
            /*
            Vector3 mouseCharLock = Camera.main.WorldToScreenPoint(transform.position);

            if (Vector2.Distance(Mouse.current.position.value, new Vector2(mouseCharLock.x, mouseCharLock.y)) > 500)
                Mouse.current.WarpCursorPosition(mouseCharLock);
            */





            //mouseMovement = true;
            /*
             *         else
            mouseMovement = false;
            */
        }



        AttackScan(); //scanning the area around the character for projectiles & melee attacks - damage intances

        DamageVisual(); //representing the damage recieved visually for the player to understand

        if (isTargetDummy)
            return;

        if (speedBuffTimer>=0)
            speedBuffTimer -= Time.deltaTime;
        
        if (shieldBuffTimer > 0)
        {
            shieldBuffTimer -= Time.deltaTime;

            if (shieldBuffTimer<=0)
                shieldGFX.SetActive(false);
        }

        if (equippedWeapon == Weapons.Blunderbuss)
        {
            charAnim.SetBool("ShotGun", true);
        }
        else
            charAnim.SetBool("ShotGun", false);

        Movement(); //character movement

        CharacterCollision();

        
        if (rollCD > 0)
        {
            rollCD -= Time.deltaTime;

            //playerIndicator.GetComponent<Image>().color = new Color(playerIndicator.GetComponent<Image>().color.r, playerIndicator.GetComponent<Image>().color.g, playerIndicator.GetComponent<Image>().color.b, Mathf.InverseLerp(2.5f, 0, rollCD));
           /*
            StaminaL.fillAmount = Mathf.InverseLerp(2.5f, 0, rollCD);
            StaminaR.fillAmount = Mathf.InverseLerp(2.5f, 0, rollCD);
            if (StaminaL.fillAmount>=1)
            {
                StaminaL.gameObject.SetActive(false);
                StaminaR.gameObject.SetActive(false);
            }
           */
        }

        if (rollInput && rollCD <= 0 && (animState == AS.idle || animState == AS.Punch1Recovery || animState == AS.Punch2Recovery || animState == AS.Punch3Recovery))
        {
            if (!mouseMovement)
                rollDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            else
                rollDirection = transform.forward;
            if (rollDirection == Vector3.zero)
            {
                rollDirection = transform.forward;
            }
            rollTimer = 0.35f;
            rolling = true;
            rollCD = 2.5f;
            charAnim.SetTrigger("Roll");

            //playerIndicator.GetComponent<Image>().color = new Color(playerIndicator.GetComponent<Image>().color.r, playerIndicator.GetComponent<Image>().color.g, playerIndicator.GetComponent<Image>().color.b, 0);

            //StaminaL.gameObject.SetActive(true);
            //StaminaR.gameObject.SetActive(true);

            SoundManager.singleton.PlayClip($"{HeadGFX.name}Dash", transform.position, 1f, true, true);

            charAnim.SetBool("StrongPunch", false); //making sure you can't strong punch after rolling
            powerPunchWindup = 0;
            rightPunchAttackState = 0;
            strongPunchCharged.Stop();
        }


        if (rolling)
        {
            rollTimer -= Time.deltaTime;

            float rollingSpeed = (startingSpeed * 4 - (startingSpeed * ((1 - (rollTimer * 2.75f)) * 4)));
            if (CC.enabled)
                CC.Move(rollDirection * rollingSpeed * Time.deltaTime);
            if (rollTimer <= 0)
                rolling = false;
        }

        if (animState != 0) //movement from attacking (forward momentum)
        {
            forwardMomentumDelay -= Time.deltaTime;
            if (forwardMomentumDelay <= 0)
            {
                if (attackMoveSpeed >= 0 && CC.enabled)
                    CC.Move(attackDirection * attackMoveSpeed * Time.deltaTime);

                attackMoveSpeed -= Time.deltaTime * 50;
            }

        }


        if (holdTimer>=0)
            holdTimer -= Time.deltaTime;
        if (slowdownTimer >=0)
            slowdownTimer -= Time.deltaTime;

        /*
        if (holdTimer <= 0)
            strongFist.enabled = false;
        */

        reloadBar.value = Mathf.InverseLerp(reloadTime, 0, slowdownTimer);
        if (slowdownTimer <= 0)
            reloadBar.gameObject.SetActive(false);

        Attack();

        if (!dead)
            PickupSearch(); //scan the area for pickups
    }

    private void AttackScan()
    {
        projSearch = Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y + 1.25f, transform.position.z), hitBoxSize / 2, Quaternion.identity, collisionMask);
        if (projSearch.Length > 0)
        {
            for (int i = 0; i < projSearch.Length; i++)
            {
                WeaponBase attackWB = projSearch[i].GetComponent<WeaponBase>();

                if (attackWB.damageType == WeaponBase.damageTypes.grenade)
                    TakeDamage(attackWB.playerID, attackWB.damage, projSearch[i].transform.position);
                else if (attackWB.damageType == WeaponBase.damageTypes.zone)
                    TakeDamage(attackWB.damage);
                else
                    TakeDamage(attackWB.playerID, attackWB.damage, attackWB.damageType, projSearch[i].transform.position);

                if (attackWB.damageType == WeaponBase.damageTypes.destructableProjectile && attackWB.playerID != PlayerID)
                    Destroy(projSearch[i].gameObject);
            }
        }
    }

    private void DamageVisual()
    {
        if (paintHead)
        {
            paintAmount += Time.deltaTime * 500;
            if (paintAmount < 127)
            {
                headColor = new Color32((byte)(255 - paintAmount), (byte)(paintAmount), (byte)(paintAmount), headColor.a);
                HeadGFX.GetComponent<Renderer>().material.SetColor("_BaseColor", headColor);

                Vector3 randomValue = new Vector3(UnityEngine.Random.Range(-3.75f, 3.75f), UnityEngine.Random.Range(-3.75f, 3.75f), UnityEngine.Random.Range(0, 0));
                Vector3 randomValueScale = Vector3.one * (UnityEngine.Random.Range(1f, 1.5f));
                hpBar.transform.localPosition = randomValue;
                emptyHpBar.transform.localPosition = randomValue;

                hpBar.transform.localScale = randomValueScale;
                emptyHpBar.transform.localScale = randomValueScale;
            }
            else
            {
                paintAmount = 0;
                headColor = startingColor;
                HeadGFX.GetComponent<Renderer>().material.SetColor("_BaseColor", headColor);
                paintHead = false;

                hpBar.transform.localPosition = Vector3.zero;
                emptyHpBar.transform.localPosition = Vector3.zero;
                hpBar.transform.localScale = Vector3.one;
                emptyHpBar.transform.localScale = Vector3.one;
            }
        }

        if (CC.enabled)
            CC.Move(new Vector3(knockbackDirection.x, 0, knockbackDirection.y));
        knockbackDirection /= (1 + Time.deltaTime * 10);
    }

    private void Movement()
    {
        if (moveInput != Vector2.zero)
        {
            if (speedBuffTimer <= 0)
            {
                currentMaxSpeed = startingSpeed;
                if (moveSpeed > currentMaxSpeed)
                    moveSpeed = currentMaxSpeed;
            }
            if (moveSpeed < currentMaxSpeed)
            {
                accelSpeed += Time.deltaTime * 5;
                deAccelSpeed = 0;
                moveSpeed += accelSpeed;
                if (moveSpeed > currentMaxSpeed)
                    moveSpeed = currentMaxSpeed;
            }
            if (slowdownTimer > 0)
            {
                moveSpeed = startingSpeed * 0.35f;
            }


            if (!mouseMovement)
                moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            else
            {
                float angleCalc = transform.eulerAngles.y + (Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg);
                angleCalc *= Mathf.Deg2Rad;
                Vector3 vectorCalc = new Vector3(Mathf.Sin(angleCalc) , 0, Mathf.Cos(angleCalc) ).normalized;
                moveDirection = vectorCalc;
            }
            if (holdTimer <= 0) //can still rotate while shooting
            {
                if (equippedWeapon == Weapons.Gun || equippedWeapon == Weapons.Lazer)
                {
                    charAnim.SetBool("RunGun", true);
                    charAnim.SetBool("Run", true);
                    charAnim.SetBool("RunShotGun", false);
                }
                else if (equippedWeapon == Weapons.Blunderbuss)
                {
                    charAnim.SetBool("RunShotGun", true);
                    charAnim.SetBool("Run", true);
                    charAnim.SetBool("RunGun", false);
                }
                else
                {
                    charAnim.SetBool("Run", true);
                    charAnim.SetBool("RunGun", false);
                    charAnim.SetBool("RunShotGun", false);
                }

                if (equippedWeapon == Weapons.Fist)
                    rFist.enabled = false;
                if (CC.enabled)
                    CC.Move(moveDirection * moveSpeed * Time.deltaTime);
            }
            else
            {
                charAnim.SetBool("Run", false);
                charAnim.SetBool("RunShotGun", false);
            }

            if (!mouseMovement)
                targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
            else
                targetAngle = transform.eulerAngles.y + (Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg);

            if ((animState == AS.idle || animState == AS.Punch1Recovery || animState == AS.Punch2Recovery || animState == AS.Punch3Recovery || animState == AS.Sword1Recovery || animState == AS.Sword2Recovery) && !rolling) //can't rotate whiling using fists
                transform.rotation = Quaternion.Euler(0, targetAngle, 0);
        }
        else
        {
            if (moveSpeed > 1)
            {
                accelSpeed = 0;
                deAccelSpeed += Time.deltaTime * 5;
                moveSpeed -= deAccelSpeed;

                if (moveSpeed < 5)
                    charAnim.SetBool("Run", false);

                if (moveSpeed < 1)
                    moveSpeed = 1;
            }
        }
    }

    private void CharacterCollision()
    {
        characterSearch = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y + 1.25f, transform.position.z), 1.25f, characterMask);
        if (characterSearch.Length > 0)
        {
            for (int i = 0; i < characterSearch.Length; i++)
            {
                if (moveInput != Vector2.zero)
                {
                    Vector3 pushDirection = characterSearch[i].transform.position - transform.position;
                    pushDirection.Normalize();
                    pushDirection /= 15;
                    pushDirection.y = 0;

                    characterSearch[i].GetComponent<CharacterController>().Move(pushDirection);
                }
            }
        }
    }

    private void SwapWeapon()
    {
        if (equippedWeapon != Weapons.Boomerang || (equippedWeapon == Weapons.Boomerang && weaponList[(int)equippedWeapon].GetComponent<Boomerang>().canThrow == true)) //checking that the boomerang is not mid air
        {
            weaponSwap = !weaponSwap;
            if (weaponSwap)
            {
                originalWeapon = equippedWeapon;
                weaponList[(int)equippedWeapon].SetActive(false);
                equippedWeapon = Weapons.Fist;
                weaponList[(int)equippedWeapon].SetActive(true);
            }
            else
            {
                weaponList[(int)equippedWeapon].SetActive(false);
                equippedWeapon = originalWeapon;
                weaponList[(int)equippedWeapon].SetActive(true);
            }
        }
    }

    private void Attack()
    {
        if (equippedWeapon == Weapons.Boomerang)
        {
            if (weaponList[(int)Weapons.Boomerang].GetComponent<Boomerang>().releasing)
            {
                charAnim.SetTrigger("ThrowRelease");
                weaponList[(int)Weapons.Boomerang].GetComponent<Boomerang>().releasing = false;
                charAnim.SetBool("ThrowCharge", false);
            }
        }
        else if (equippedWeapon == Weapons.Grenade)
        {
            if (weaponList[(int)Weapons.Grenade].GetComponent<Grenade>().releasing)
            {
                charAnim.SetTrigger("ThrowRelease");
                weaponList[(int)Weapons.Grenade].GetComponent<Grenade>().releasing = false;
                charAnim.SetBool("ThrowCharge", false);
            }
        }

        if (useWeapon && rightPunchAttackState == 0)
        {
            powerPunchWindup = 0;

            if (equippedWeapon == Weapons.Fist)  //no punch if other weapon equipped
            {
                if (animState == AS.idle || animState == AS.Punch3Recovery)
                {
                    holdTimer = 0.4166f;  // 25/60 chanel can't move during attack windup & active, full animation is 0.75
                    attackDirection = transform.forward;
                    attackMoveSpeed = 16;
                    forwardMomentumDelay = 0.166f; // 10/60 chanel

                    animTimer = 0;
                    animState = AS.Punch1Windup;
                    charAnim.SetTrigger("Punch1");
                    SoundManager.singleton.PlayClip($"{HeadGFX.name}Melee1", transform.position, 1f, true, true);
                }
                else if (animState == AS.Punch1Recovery)
                {
                    holdTimer = 0.4166f; //can't move during attack windup & active, full animation is 0.75
                    attackDirection = transform.forward;
                    attackMoveSpeed = 17;
                    forwardMomentumDelay = 0.166f; // 10/60 chanel

                    animTimer = 0;
                    animState = AS.Punch2Windup;
                    charAnim.SetTrigger("Punch2");
                    SoundManager.singleton.PlayClip($"{HeadGFX.name}Melee2", transform.position, 1f, true, true);
                }
                else if (animState == AS.Punch2Recovery)
                {
                    holdTimer = 0.4166f; // 25/60 can't move during attack windup & active, full animation is 0.75
                    attackDirection = transform.forward;
                    attackMoveSpeed = 17;
                    forwardMomentumDelay = 0.166f; // 10/60 chanel

                    animTimer = 0;
                    animState = AS.Punch3Windup;
                    charAnim.SetTrigger("Punch3");
                    SoundManager.singleton.PlayClip($"{HeadGFX.name}Melee3", transform.position, 1f, true, true);
                }
            }
            else if (equippedWeapon == Weapons.Sword)
            {
                if (animState == AS.idle || animState == AS.Sword2Recovery)
                {
                    holdTimer = 0.4166f;  // 25/60 chanel
                    attackDirection = transform.forward;
                    attackMoveSpeed = 30;
                    forwardMomentumDelay = 0.166f; // 10/60 chanel

                    animTimer = 0;
                    animState = AS.Sword1Windup;
                    charAnim.SetTrigger("Sword1");
                    SoundManager.singleton.PlayClip($"{HeadGFX.name}Sword1", transform.position, 1f, true, true);
                }
                else if (animState == AS.Sword1Recovery)
                {
                    holdTimer = 0.4166f;
                    attackDirection = transform.forward;
                    attackMoveSpeed = 30;
                    forwardMomentumDelay = 0.166f; // 10/60 chanel

                    animTimer = 0;
                    animState = AS.Sword2Windup;
                    charAnim.SetTrigger("Sword2");
                    SoundManager.singleton.PlayClip($"{HeadGFX.name}Sword2", transform.position, 1f, true, true);
                }

            }
            else //using other weapons
            {
                Shoot(equippedWeapon);
            }
        }
        if (animState != AS.idle)
        {
            animTimer += Time.deltaTime;

            if (animState == AS.Punch1Windup || animState == AS.Punch1Active || animState == AS.Punch1Recovery)
            {
                if (animTimer >= 0.4166f) // 23/60 osher 25/60 chanel
                {
                    animState = AS.Punch1Recovery;
                    rFist.enabled = false;
                }
                else if (animTimer >= 0.35f) // 18/60 osher active 21/60 chanel 
                {
                    animState = AS.Punch1Active;
                    rFist.enabled = true;
                }
            }

            if (animState == AS.Punch2Windup || animState == AS.Punch2Active || animState == AS.Punch2Recovery)
            {
                if (animTimer >= 0.4166f) // 25/60 osher 25/60 chanel
                {
                    animState = AS.Punch2Recovery;
                    lFist.enabled = false;
                }
                else if (animTimer >= 0.35f) // 20/60 osher active  21/60 chanel 
                {
                    animState = AS.Punch2Active;
                    lFist.enabled = true;
                }
            }


            if (animState == AS.Punch3Windup || animState == AS.Punch3Active || animState == AS.Punch3Recovery)
            {
                if (animTimer >= 0.4166f) // 31/60 25/60 chanel
                {
                    animState = AS.Punch3Recovery;
                    rFist.enabled = false;
                    lFist.enabled = false;
                }
                else if (animTimer >= 0.166f) // 14/60 active 10/60 chanel 
                {
                    animState = AS.Punch3Active;
                    rFist.enabled = true;
                    lFist.enabled = true;
                }
            }
            if (animState == AS.StrongPunch)
            {
                if (animTimer >= 0.22f)
                {
                    strongFist.enabled = false;
                    if (strongPunchSwap)
                    {
                        SwapWeapon();
                        strongPunchSwap = false;
                    }
                }
                else if (animTimer >= 0.05f)
                    strongFist.enabled = true;
            }

            if (animTimer >= 0.5833f) // 45/60  35/60 chanel
            {
                animState = 0; //idle
                animTimer = 0;
            }

            ////////////////////////////////////////// sword attack states

            if (animState == AS.Sword1Windup || animState == AS.Sword1Active || animState == AS.Sword1Recovery)
            {
                if (animTimer >= 0.45f) //25/60 chanel original || buffed version: 27/60
                {
                    animState = AS.Sword1Recovery;
                    weaponList[(int)Weapons.Sword].GetComponent<BoxCollider>().enabled = false;
                }
                else if (animTimer >= 0.3f) //21/60 chanel original || buffed version: 18/60
                {
                    animState = AS.Sword1Active;
                    weaponList[(int)Weapons.Sword].GetComponent<BoxCollider>().enabled = true;
                }
            }

            if (animState == AS.Sword2Windup || animState == AS.Sword2Active || animState == AS.Sword2Recovery)
            {
                if (animTimer >= 0.45f) //25/60 chanel original || buffed version: 27/60
                {
                    animState = AS.Sword2Recovery;
                    weaponList[(int)Weapons.Sword].GetComponent<BoxCollider>().enabled = false;
                }
                else if (animTimer >= 0.3f) //21/60 chanel  original || buffed version: 18/60
                {
                    animState = AS.Sword2Active;
                    weaponList[(int)Weapons.Sword].GetComponent<BoxCollider>().enabled = true;
                }
            }
        }

        if (rightPunchAttackState == 1 && holdTimer <= 0)
        {
            if (!chargeSFX && powerPunchWindup >0.25f) //to prevent sound spamming
            {
                SoundManager.singleton.PlayClip($"{HeadGFX.name}Charge", transform.position, 1f, true, true); //consider lowering volume to prevent spamming
                chargeSFX = true;
            }
            if (equippedWeapon != Weapons.Fist)
            {
                SwapWeapon();
                strongPunchSwap = true;
            }

            if (animState == 0)
            {
                if (powerPunchWindup == 0.75f)
                {
                    rumbleTimer -= Time.deltaTime;
                    if (rumbleTimer <= 0)
                    {
                        rumbleTimer = 0.25f;

                        if (!(keyboardMouse || isTargetDummy || mouseMovement))
                            RumbleManager.instance.RumblePulse(0.15f, 0.375f, 0.10f, PI);
                    }
                }

                slowdownTimer = 0.05f;
                if (powerPunchWindup < 0.75)
                {
                    powerPunchWindup += Time.deltaTime;
                    charAnim.SetBool("StrongPunch", true);
                    if (powerPunchWindup >= 0.75)
                    {
                        strongPunchCharged.Play();
                        strongPunchPulse.Play("Strong Punch Pulse");
                        powerPunchWindup = 0.75f;
                    }
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
                attackDirection = transform.forward;
                attackMoveSpeed = 37;
                strongPunchCharged.Stop();
                strongPunchPulse.Play("Waiting");

                charAnim.SetTrigger("StrongPunchRelease");
                SoundManager.singleton.PlayClip($"{HeadGFX.name}Throw", transform.position, 1f, true, true);
                charAnim.SetBool("StrongPunch", false);

                powerPunchWindup = 0;
                chargeSFX = false;
            }
            else
            {
                if (strongPunchSwap)
                {
                    SwapWeapon();
                    strongPunchSwap = false;
                }
                charAnim.SetBool("StrongPunch", false);
                powerPunchWindup = 0;
                chargeSFX = false;
            }
        }
    }

    private void PickupSearch()
    {
        pickupSearch = (Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y + 1.25f, transform.position.z), hitBoxSize / 2, Quaternion.identity, pickupMask));
        if (pickupSearch.Length > 0)
        {
            for (int i = 0; i <= pickupSearch.Length; i++)
            {
                if (Enum.TryParse<Weapons>(pickupSearch[i].transform.name, out Weapons weapon))
                {
                    previousWeapon = equippedWeapon;
                    equippedWeapon = Enum.Parse<Weapons>(pickupSearch[i].transform.name);
                    originalWeapon = equippedWeapon;

                    weaponList[(int)previousWeapon].SetActive(false);
                    weaponList[(int)equippedWeapon].SetActive(true);

                    Destroy(pickupSearch[i].transform.gameObject);
                    return;
                }
                else if (pickupSearch[i].transform.name == "Coin" || pickupSearch[i].transform.name == "Coin2" || pickupSearch[i].transform.name == "Speed" || pickupSearch[i].transform.name == "Health" || pickupSearch[i].transform.name == "Shield" || pickupSearch[i].transform.name == "CannonBall" || pickupSearch[i].transform.name == "CoinSack")
                {
                    SoundManager.singleton.PlayClip("Pickup", transform.position, 0.15f, true, true);
                    if (pickupSearch[i].transform.name == "Coin")
                    {
                        moneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString() + "+" + "1";
                        Leaderboard.singleton.ModifyMoney(PlayerID, 1);
                        moneyAnim.Play("Player Coin Pickup", -1, 0);
                        pickupSearch[i].transform.gameObject.SetActive(false);
                        PickupManager.singleton.CoinPickupVFX(pickupSearch[i].transform.position);
                    }
                    else if (pickupSearch[i].transform.name == "Coin2")
                    {
                        moneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString() + "+" + "1";
                        Leaderboard.singleton.ModifyMoney(PlayerID, 1);
                        moneyAnim.Play("Player Coin Pickup", -1, 0);
                        pickupSearch[i].transform.gameObject.SetActive(false);
                        PickupManager.singleton.CoinPickupVFX(pickupSearch[i].transform.position);
                    }
                    else if (pickupSearch[i].transform.name == "CoinSack")
                    {
                        moneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString() + "+" + "3";
                        Leaderboard.singleton.ModifyMoney(PlayerID, 3);
                        moneyAnim.Play("Player Coin Pickup", -1, 0);
                        pickupSearch[i].transform.gameObject.SetActive(false);
                        PickupManager.singleton.CoinPickupVFX(pickupSearch[i].transform.position);
                    }
                    else if (pickupSearch[i].transform.name == "Health")
                    {
                        if (hp + 3 >= 10)
                        {
                            if (RoundManager.instance.areWeWarming == false)
                            {
                                hp = 10;
                                hpBar.fillAmount = 10;
                            }               
                        }
                        else
                        {
                            hp += 3;
                            hpBar.fillAmount += 3f / 10f;
                        }
                        healthBuffEffect.Play();

                        moneyText.text = "Health";
                        moneyAnim.Play("Player Powerup Pickup", -1, 0);

                        Destroy(pickupSearch[i].transform.gameObject);
                    }
                    else if (pickupSearch[i].transform.name == "Speed")
                    {
                        currentMaxSpeed = startingSpeed * 1.5f;
                        speedBuffTimer = 5;
                        speedBuffEffect.Play();

                        moneyText.text = "Speed";
                        moneyAnim.Play("Player Powerup Pickup", -1, 0);

                        Destroy(pickupSearch[i].transform.gameObject);
                    }
                    else if (pickupSearch[i].transform.name == "Shield")
                    {
                        shieldGFX.SetActive(true);
                        shieldBuffTimer = 3.5f;

                        moneyText.text = "Shield";
                        moneyAnim.Play("Player Powerup Pickup", -1, 0);

                        Destroy(pickupSearch[i].transform.gameObject);
                    }
                    return;
                }
                else
                {
                    Debug.Log("Change the object's name to the correct weapon or pickup name");
                }
            }
        }
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
                    if (hp > 0 && hp - damage <= 0)
                    {
                        Leaderboard.singleton.AnnounceKill(attackingPlayer, PlayerID);
                        OutTheRound();
                        DeadStop();
                    }

                    Leaderboard.singleton.StopForwardMomentum(attackingPlayer);

                    hp = hp - damage;
                    hpBar.fillAmount = hp / 10f;
                    charAnim.SetTrigger("DMG");

                    if (!dead)
                    {
                        headColor = Color.red;
                        paintHead = true;
                    }

                    meleeParticleEffect.transform.position = hitPos;
                    meleeParticleEffect.Play();

                    knockbackDirection += new Vector2(transform.position.x - hitPos.x, transform.position.z - hitPos.z);
                    knockbackDirection.Normalize();
                    knockbackDirection *= 0.15f;
                    knockbackDirection *= (damage / 2f);

                    if (identicalDamageCD<0)
                    {
                        if (damage<=2)
                        {
                            SoundManager.singleton.PlayClip($"{HeadGFX.name}HitS", transform.position, 1f, true, true);

                        }
                        else if (damage<=4)
                        {
                            SoundManager.singleton.PlayClip($"{HeadGFX.name}HitM", transform.position, 1f, true, true);

                        }
                        else //6
                        {
                            SoundManager.singleton.PlayClip($"{HeadGFX.name}HitL", transform.position, 1f, true, true);

                        }

                    }

                    if (!(keyboardMouse || isTargetDummy || mouseMovement))
                        RumbleManager.instance.RumblePulse((0.25f + damage * 0.125f), 0.5f, 0.225f, PI);

                    if (cameraManagerIsOn)
                        CameraManager.instance.ShakeCamera(0.1f * damage, 0.1f);
                }
            }
            lastPlayerID = attackingPlayer;
            identicalDamageCD = 0.1f;
            timeSinceLastPlayerHit = 0;
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

                if (hp > 0 && hp - damageBasedOnDistance <= 0)
                {
                    Leaderboard.singleton.AnnounceKill(attackingPlayer, PlayerID);
                    OutTheRound();
                    DeadStop();
                }

                hp = hp - damageBasedOnDistance;
                hpBar.fillAmount = hp / 10f;
                charAnim.SetTrigger("DMG");

                if (!dead)
                {
                    headColor = Color.red;
                    paintHead = true;
                }

                knockbackDirection = new Vector2(transform.position.x - grenadePos.x, transform.position.z - grenadePos.z);
                knockbackDirection.Normalize();
                knockbackDirection *= 0.15f;
                knockbackDirection *= (damageBasedOnDistance / 2f);

                if (damageBasedOnDistance <= 2)
                {
                    SoundManager.singleton.PlayClip($"{HeadGFX.name}HitS", transform.position, 1f, true, true);

                }
                else if (damageBasedOnDistance <= 4)
                {
                    SoundManager.singleton.PlayClip($"{HeadGFX.name}HitM", transform.position, 1f, true, true);

                }
                else //6
                {
                    SoundManager.singleton.PlayClip($"{HeadGFX.name}HitL", transform.position, 1f, true, true);

                }

                if (!(keyboardMouse || isTargetDummy || mouseMovement))
                    RumbleManager.instance.RumblePulse((0.25f + damageBasedOnDistance * 0.125f), 0.5f, 0.225f, PI);

                if (cameraManagerIsOn)
                    CameraManager.instance.ShakeCamera(0.1f * damage, 0.1f);
            }
            lastPlayerID = attackingPlayer;
            identicalDamageCD = 0.1f;
            timeSinceLastPlayerHit = 0;
        }
        else if (attackingPlayer == PlayerID) //plays can get hurt from their own bombs, take reduced damage
        {
            if (shieldBuffTimer <= 0)
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

                if (hp > 0 && hp - damageBasedOnDistance <= 0)
                {
                    Leaderboard.singleton.AnnounceKill(attackingPlayer, PlayerID);

                    OutTheRound();
                    DeadStop();
                }

                damageBasedOnDistance = (int)(damageBasedOnDistance * 0.5f);

                hp = hp - damageBasedOnDistance;
                hpBar.fillAmount = hp / 10f;
                charAnim.SetTrigger("DMG");

                if (!dead)
                {
                    headColor = Color.red;
                    paintHead = true;
                }

                knockbackDirection = new Vector2(transform.position.x - grenadePos.x, transform.position.z - grenadePos.z);
                knockbackDirection.Normalize();
                knockbackDirection *= 0.15f;
                knockbackDirection *= (damageBasedOnDistance / 2f);

                if (damageBasedOnDistance <= 2)
                {
                    SoundManager.singleton.PlayClip($"{HeadGFX.name}HitS", transform.position, 1f, true, true);

                }
                else if (damageBasedOnDistance <= 4)
                {
                    SoundManager.singleton.PlayClip($"{HeadGFX.name}HitM", transform.position, 1f, true, true);

                }
                else //6
                {
                    SoundManager.singleton.PlayClip($"{HeadGFX.name}HitL", transform.position, 1f, true, true);

                }

                if (!(keyboardMouse || isTargetDummy || mouseMovement))
                    RumbleManager.instance.RumblePulse((0.25f + damageBasedOnDistance * 0.125f), 0.5f, 0.225f, PI);

                if (cameraManagerIsOn)
                    CameraManager.instance.ShakeCamera(0.1f * damage, 0.1f);
            }
        }
    }

    private void TakeDamage(int damage) // in case of zone/storm closing
    {
        if (shieldBuffTimer <= 0)
        {
            if (zoneImmunity)
            {
                return;
            }
            else if (zoneTicksGraceAmount > 0)
                zoneTicksGraceAmount--;
            else
            {
                if (hp > 0 && hp - damage <= 0)
                {
                    if (timeSinceLastPlayerHit <5)
                    {
                        Leaderboard.singleton.AnnounceKill(lastPlayerID, PlayerID);
                    }
                    else
                        Leaderboard.singleton.AnnounceKill(PlayerID); //overload function

                    OutTheRound();
                    DeadStop();
                }

                hp = hp - damage;
                hpBar.fillAmount = hp / 10f;
                charAnim.SetTrigger("DMG");

                if (!dead)
                {
                    headColor = Color.red;
                    paintHead = true;
                }

                SoundManager.singleton.PlayClip($"{HeadGFX.name}HitS", transform.position, 1f, true, true);

                if (!(keyboardMouse || isTargetDummy || mouseMovement))
                    RumbleManager.instance.RumblePulse(0.25f, 0.5f, 0.225f, PI);

                if (cameraManagerIsOn)
                    CameraManager.instance.ShakeCamera(0.1f * damage, 0.1f);
            }
        }
    }

    private void Shoot(Weapons WeaponUsed)
    {
        switch (WeaponUsed)
        {
            case Weapons.Fist:         // 0
                break;
            case Weapons.Gun:          // 1
                charAnim.SetTrigger("Shoot");
                //holdTimer = 0.15f;
                slowdownTimer = 0.15f;
                break;
            case Weapons.Sword:        // 2
                break;
            case Weapons.Boomerang:    // 3
                if (weaponList[(int)Weapons.Boomerang].GetComponent<Boomerang>().charging)
                {
                    charAnim.SetBool("ThrowCharge", true);
                    holdTimer = 0.2f;
                }
                break;
            case Weapons.Lazer:      // 4
                charAnim.SetTrigger("Shoot");
                holdTimer = 0.15f;
                break;
            case Weapons.Mine:       // 5
                if (weaponList[(int)Weapons.Mine].GetComponent<Mine>().placing)
                {
                    weaponList[(int)Weapons.Mine].GetComponent<Mine>().placing = false;
                    charAnim.SetTrigger("Mine");
                }
                holdTimer = 0.25f;
                break;
            case Weapons.Blunderbuss:// 6
                if (weaponList[(int)Weapons.Blunderbuss].GetComponent<Blunderbuss>().shoot)
                {
                    charAnim.SetTrigger("Shoot");
                    weaponList[(int)Weapons.Blunderbuss].GetComponent<Blunderbuss>().shoot = false;
                    animState = AS.StrongPunch;
                    holdTimer = weaponList[(int)Weapons.Blunderbuss].GetComponent<Blunderbuss>().holdTime;
                    attackDirection = -moveDirection;
                    attackMoveSpeed = 10;
                }
                else if (weaponList[(int)Weapons.Blunderbuss].GetComponent<Blunderbuss>().reloading)
                {
                    weaponList[(int)Weapons.Blunderbuss].GetComponent<Blunderbuss>().reloading = false;
                    slowdownTimer = weaponList[(int)Weapons.Blunderbuss].GetComponent<Blunderbuss>().holdTime * 2;
                    reloadBar.gameObject.SetActive(true);
                    reloadTime = slowdownTimer;
                }
                break;
            case Weapons.Grenade:  // 7
                if (weaponList[(int)Weapons.Grenade].GetComponent<Grenade>().charging)
                {
                    charAnim.SetBool("ThrowCharge", true);
                    holdTimer = 0.25f;
                }
                break;
            default:
                Debug.Log("incorrect weapon used enum");
                break;
        }
    }

    private void DeadStop()
    {
        paintAmount = 0; //making sure head isn't red
        HeadGFX.GetComponent<Renderer>().material.SetColor("_BaseColor", startingColor);
        paintHead = false;

        shieldBuffTimer = -1; //making sure not starting with a buff
        speedBuffTimer = -1;
        knockbackDirection = Vector2.zero;
        useWeapon = false; //making sure not attacking at the start of round

        strongPunchCharged.Stop();
        charAnim.ResetTrigger("Roll");
        charAnim.ResetTrigger("Punch1");
        charAnim.ResetTrigger("Punch2");
        charAnim.ResetTrigger("Punch3");
        charAnim.ResetTrigger("Shoot");
        charAnim.SetBool("StrongPunch", false);
        charAnim.ResetTrigger("StrongPunchRelease");
        charAnim.ResetTrigger("DMG");
        charAnim.ResetTrigger("Mine");
        charAnim.ResetTrigger("Sword1");
        charAnim.ResetTrigger("Sword2");
        charAnim.SetBool("ThrowCharge", false);
        charAnim.ResetTrigger("ThrowRelease");

        shieldGFX.SetActive(false);
        meleeParticleEffect.Stop();
        speedBuffEffect.Stop();
        healthBuffEffect.Stop();
        leaderGlow.Stop();
        strongPunchCharged.Stop();
    }

    public void BuyWeapon(string shopWeaponName)
    {
        if (Enum.TryParse<Weapons>(shopWeaponName, out Weapons weapon))
        {
            previousWeapon = equippedWeapon;
            equippedWeapon = Enum.Parse<Weapons>(shopWeaponName);

            originalWeapon = equippedWeapon; //interaction with weapon swap

            weaponList[(int)previousWeapon].SetActive(false);
            weaponList[(int)equippedWeapon].SetActive(true);
        }
        else
        {
            Debug.Log("Change the object's name to the correct weapon");
        }
    }

    public void BuyConsumeable(string shopConsumeableName)
    {
        //delete later when cleaning up Shop Manager script
    }

    public void EmptyHand()
    {
        weaponList[(int)equippedWeapon].SetActive(false);
        equippedWeapon = Weapons.Fist;
        weaponList[(int)equippedWeapon].SetActive(true);
    }

    public void SetLeader(PlayerTypes newLeader)
    {
        if (newLeader == PlayerID)
        {
            if (Leaderboard.singleton.GetMoney(PlayerID)!=0) //making sure a player with no money gets the crown
            {
                leaderGlow.Play();
                leaderCrown.SetActive(true);
            }
        }
        else
        {
            leaderGlow.Stop();
            leaderCrown.SetActive(false);
        }
    }

    public void GetKillingMoney (int killReward)
    {
        int money = Leaderboard.singleton.GetMoney(PlayerID);
        money -= killReward; //the killing money was already added before
        moneyText.text = money.ToString() + "+" +killReward;
        moneyAnim.Play("Player Coin Pickup", -1, 0);
        PickupManager.singleton.CoinPickupVFX(transform.position);
    }

    public void SetWinner()
    {
        winner = true;
        SoundManager.singleton.PlayClip($"{HeadGFX.name}WinRound", transform.position, 1f, true, false);
    }

    public void VictoryOrLose(int result)
    {
        if (result == 0) charAnim.Play("Victory");
        else if (result == 1) charAnim.Play("Loser");
        else if (result == 2) charAnim.Play("Loser2");
        else if (result == 3) charAnim.Play("Loser3");
    }

    public void StopForwardMomentum()
    {
        attackMoveSpeed = 0;
    }

    public void DisableWeaponScripts()
    {
        weaponScripts.SetActive(false);
    }

    public void EnableWeaponScripts()
    {
        weaponScripts.SetActive(true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + 1.25f, transform.position.z), hitBoxSize); // the original is half extents

        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + 1.25f, transform.position.z), hitBoxSize); // the original is half extents
    }
}