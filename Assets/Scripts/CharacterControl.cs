using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;

using UnityEngine.UI;
using TMPro;
using MelenitasDev.SoundsGood;


public class CharacterControl : MonoBehaviour
{
    [Foldout("Upgrades")]
    public bool Ghost = false; //movespeed + go through obstacles
    [SerializeField] ParticleSystem ghostVFX;
    //private float ghostMoveSpeed;
    //private float ghostBuffDuration;
    private float ghostCD;
    public bool Teleport = false; //move instantly in the direction character is facing
    [SerializeField] ParticleSystem teleportVFXStart;
    [SerializeField] ParticleSystem teleportVFXEnd;
    private float teleportDistance = 6;
    private float teleportCD;
    public bool Invisibility = false; //self explanatory
    public GameObject[] bodyPartsGFX;
    [SerializeField] GameObject propHideout;
    private float invisibilityduration;
    private float invisibilityCD;

    [EndFoldout]

    public bool isTargetDummy = false;
    public bool cameraManagerIsOn = false;
    private bool mouseMovement = false;

    [HideInInspector] public bool zoneImmunity = false;
    [HideInInspector] public int zoneTicksGraceAmount = 2;

    public PlayerTypes PlayerID;
    //public static PlayerTypes discardingPlayerID;
    //public static PlayerTypes holdingPlayerID;

    //bool onOff = false;

    public int hp = 10;

    private int coinsAtRoundStart;

    Collider[] pickupSearch;
    [SerializeField] LayerMask pickupMask;
    private float speedBuffTimer;
    private float shieldBuffTimer;

    Collider[] characterSearch;
    [SerializeField] LayerMask characterMask;

    [SerializeField] LayerMask collisionMask;
    Collider[] projSearch;
    float identicalDamageCD;
    private PlayerTypes lastPlayerID;
    private float timeSinceLastPlayerHit; //checking if players intentionally dying to zone

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
    //[SerializeField] WeaponConfig[] configs;
    private bool weaponSwap;
    private bool strongPunchSwap;
    private Weapons originalWeapon;

    SphereCollider rFist;
    [SerializeField] SphereCollider lFist;
    [SerializeField] SphereCollider strongFist;

    public Animator charAnim;

    [Foldout("Limb Animators")]
    /* //new animator & animations
    [SerializeField] Animator lArmAnim;
    [SerializeField] Animator rArmAnim;
    [SerializeField] Animator lFootAnim;
    [SerializeField] Animator rFootAnim;
    [SerializeField] Transform rArm;
    */


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
    public enum PlayerTypes //add neutral?
    {
        Red,
        Green,
        Blue,
        Yellow
    }

    private Weapons equippedWeapon;
    private Weapons previousWeapon;

    //public static int weaponID;

    //private static bool weaponDiscarded = false;


    CharacterController CC;
    bool useWeapon;
    bool rollInput;
    bool shieldInput;
    bool consumableInput;
    int rightPunchAttackState;
    [HideInInspector] public bool dead;
    private Vector2 moveInput;

    [EndFoldout]

    //[SerializeField] TextMeshProUGUI hpText;
    [SerializeField] Sprite[] onCharIcons;
    [SerializeField] Image weaponSprite;
    [SerializeField] Image powerupSprite;
    [SerializeField] Image StaminaL;
    [SerializeField] Image StaminaR;
    [SerializeField] Image hpBar;
    [SerializeField] Image emptyHpBar;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] Animator moneyAnim;
    [SerializeField] TextMeshProUGUI onCharMoneyText;
    [SerializeField] GameObject shieldGFX;
    [SerializeField] GameObject blockBubble;
    private float blockDuration;
    private float blockCD;

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

    public GameObject HeadGFX;
    Color32 headColor = Color.grey;
    Color startingColor;

    private bool paintHead = false;
    private float paintAmount;
    private Vector3 desiredPos;
    //private Vector3 knockbackDirection;
    private Vector2 knockbackDirection;

    [SerializeField] GameObject playerIndicator;

    //[HideInInspector] public int cannonBallAmount = 0;
    //private int cannonBallAmount = 0;
    //[SerializeField] GameObject cannonBall;

    float rumbleTimer;

    [HideInInspector] public bool winner;

    private PlayerInput PI;

    bool keyboardMouse = false;
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
        if (PI.currentControlScheme == "Keyboard & Mouse") //you really tell me it contains a string
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


        //rightArmGFX.GetComponent<SphereCollider>().enabled = false; //reminder

        //if (isTargetDummy)
        //return;

        startingSpeed = moveSpeed;
        currentMaxSpeed = startingSpeed;
        //weaponID = 0;

        //REMINDER//rightArmGFX.GetComponent<WeaponBase>().playerID = PlayerID; //special treatment as he does not act like the other hand

        animState = AS.idle;

        useWeapon = false;

        //hpText.text = ("HP: " + hp);
        hpBar.fillAmount = 1f;

        //originalHpBarPos = hpBar.transform.position;
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
    public void Consumable(InputAction.CallbackContext context)
    {
        consumableInput = context.action.triggered;
    }

    public void OutTheRound()
    {
        //int randomMoneyDrop = UnityEngine.Random.Range(2, 7);
        //int moneylost = (int)(0.25f * Leaderboard.singleton.GetMoney(PlayerID));
        int moneylost = Leaderboard.singleton.DeathMoney(PlayerID);
        moneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString() + "-" + moneylost;
        onCharMoneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString();
        //Leaderboard.singleton.ModifyMoney(PlayerID, -moneylost); // I update it from the function itself
        moneyAnim.Play("Player Coin Pickup", -1, 0);

        //or do they? //players no longer drop physical money
        /*
        if (moneylost == 0)
            moneylost = 1;
        */
        /*
        for (int i =0;i<moneylost;i++)
        {
            PickupManager.singleton.SpawnTreasureChestCoin(new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z));
        }
        */
        PickupManager.singleton.SpawnDeadCharacterCoin(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), moneylost);

        PickupManager.singleton.SpawnPowerUp(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z));

        //Debug.Log(HeadGFX.name);
        //Debug.Log("Dead Player"+new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z));

        CC.enabled = false;

        //characterGFX.SetActive(false);

        weaponList[(int)equippedWeapon].SetActive(false); //making sure he can't attack while dead kek

        charAnim.Play("Death");

        DeadStop(); //making sure character is dead

        dead = true;

        bodyPartsGFX[6].SetActive(false);
        bodyPartsGFX[7].SetActive(false);
        //SoundManager.singleton.Death(transform.position);
        SoundManager.singleton.PlayClip($"{HeadGFX.name}Death", transform.position, 1f, true, true);
        //SoundManager.singleton.PlayClip("Death", transform.position, 1f, false, true);

        PlayerManager.instance.PlayerCheck();
        if (cameraManagerIsOn)
            CameraManager.instance.RemoveFromCameraGroup(gameObject);
        TimeManager.instance.SlowTime(0.4f, 1f);
    }
    public void NextRound()
    {
        hp = 10;
        //hpText.text = "HP: " + hp;
        hpBar.fillAmount = 1f;
        CC.enabled = true;
        characterGFX.SetActive(true);
        dead = false;
        moneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString();
        onCharMoneyText.text = moneyText.text;
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
        //hpText.text = "HP: " + hp;
        hpBar.fillAmount = 1f;
        CC.enabled = true;
        characterGFX.SetActive(true);
        dead = false;
        //moneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString();
        //coinsAtRoundStart = Leaderboard.singleton.GetMoney(PlayerID);
        //Leaderboard.singleton.UpdateRoundMoney(PlayerID, coinsAtRoundStart);

        weaponList[(int)equippedWeapon].SetActive(true); //making sure he can't attack while dead kek

        //DeadStop(); //making sure character was dead
        bodyPartsGFX[6].SetActive(true);
        bodyPartsGFX[7].SetActive(false); //removing the hp bar from the warmup round, characters can't die

        charAnim.Play("Idle");
        winner = false;
    }

    void Update()
    {
        //Debug.Log("Mouse"+Camera.main.WorldToScreenPoint(transform.position));
        //Debug.Log("Character" + transform.position);
        //Debug.Log(Mouse.current.position.value);

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Ghost = true;
            Invisibility = false;
            Teleport = false;
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            Ghost = false;
            Invisibility = true;
            Teleport = false;
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            Ghost = false;
            Invisibility = false;
            Teleport = true;
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            Leaderboard.singleton.ModifyMoney(PlayerID, 5);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            OutTheRound();
            DeadStop();
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            if (mouseMovement)
                mouseIndicator.gameObject.SetActive(false);
            else
                mouseIndicator.gameObject.SetActive(true);

            mouseMovement = !mouseMovement;

        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            RumbleManager.instance.RumblePulse(1f, 1f, 0.5f, PI);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            RumbleManager.instance.RumblePulse(0f, 1f, 0.5f, PI);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            RumbleManager.instance.RumblePulse(1f, 0f, 0.5f, PI);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            RumbleManager.instance.RumblePulse(1f, 1f, 0f, PI);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            RumbleManager.instance.RumblePulse(0.5f, 0.5f, 0.05f, PI);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            RumbleManager.instance.RumblePulse(0.5f, 0.5f, 0.1f, PI);
        if (Input.GetKeyDown(KeyCode.Alpha7))
            RumbleManager.instance.RumblePulse(0.5f, 0.5f, 0.15f, PI); //feels best
        if (Input.GetKeyDown(KeyCode.Alpha8))
            RumbleManager.instance.RumblePulse(0.5f, 0.5f, 0.20f, PI);
        if (Input.GetKeyDown(KeyCode.Alpha9))
            RumbleManager.instance.RumblePulse(0.5f, 0.5f, 0.25f, PI);

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

        identicalDamageCD -= Time.deltaTime;
        timeSinceLastPlayerHit += Time.deltaTime;

        if (mouseMovement) //360 aiming with mouse && Input.GetMouseButton(1)  //equippedWeapon != Weapons.Fist && (Input.GetMouseButton(1) || Input.GetMouseButton(0))
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

        speedBuffTimer -= Time.deltaTime;
        
        if (shieldBuffTimer > 0)
        {
            shieldBuffTimer -= Time.deltaTime;

            if (shieldBuffTimer<=0)
            {
                shieldGFX.SetActive(false);
                if (speedBuffTimer <= 0)
                    powerupSprite.sprite = null;
                else
                    powerupSprite.sprite = onCharIcons[8];
            }
            /*
            if (speedBuffTimer>0) //in case both buffs are active
                powerupSprite.sprite = onCharIcons[8];
            else
                powerupSprite = null;
            */
        }

        if (equippedWeapon == Weapons.Blunderbuss)
        {
            charAnim.SetBool("ShotGun", true);
        }
        else
            charAnim.SetBool("ShotGun", false);

        Movement(); //character movement

        CharacterCollision();

        if (Input.GetKeyDown(KeyCode.Insert)) //unused right now, might change later
            SwapWeapon();

        if (shieldInput && blockCD <= 0) //disable for now
        {
            blockBubble.SetActive(true);
            blockDuration = 0.75f;
            blockCD = 3.5f;
            holdTimer = 0.75f;

            //SoundManager.singleton.Shield(transform.position);
        }

        
        if (rollCD > 0)
        {
            rollCD -= Time.deltaTime;

            //playerIndicator.GetComponent<Image>().color = new Color(playerIndicator.GetComponent<Image>().color.r, playerIndicator.GetComponent<Image>().color.g, playerIndicator.GetComponent<Image>().color.b, Mathf.InverseLerp(2.5f, 0, rollCD));
           
            StaminaL.fillAmount = Mathf.InverseLerp(2.5f, 0, rollCD);
            StaminaR.fillAmount = Mathf.InverseLerp(2.5f, 0, rollCD);
            if (StaminaL.fillAmount>=1)
            {
                StaminaL.gameObject.SetActive(false);
                StaminaR.gameObject.SetActive(false);
            }
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
            //charAnim.Play("SpinDash");
            charAnim.SetTrigger("Roll");

            //playerIndicator.GetComponent<Image>().color = new Color(playerIndicator.GetComponent<Image>().color.r, playerIndicator.GetComponent<Image>().color.g, playerIndicator.GetComponent<Image>().color.b, 0);

            StaminaL.gameObject.SetActive(true);
            StaminaR.gameObject.SetActive(true);

            //SoundManager.singleton.Roll(transform.position);
            SoundManager.singleton.PlayClip($"{HeadGFX.name}Dash", transform.position, 1f, true, true);


            charAnim.SetBool("StrongPunch", false); //making sure you can't strong punch after rolling
            powerPunchWindup = 0;
            rightPunchAttackState = 0;
            strongPunchCharged.Stop();
        }


        if (rolling)
        {
            rollTimer -= Time.deltaTime;

            //transform.Rotate(Vector3.right, Time.deltaTime * 1030); //replaced with animation

            float rollingSpeed = (startingSpeed * 4 - (startingSpeed * ((1 - (rollTimer * 2.75f)) * 4)));
            if (CC.enabled)
                CC.Move(rollDirection * rollingSpeed * Time.deltaTime);
            if (rollTimer <= 0)
            {
                rolling = false;
                //charAnim.Play("Idle");
                //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        }


        if (consumableInput)
        {

            if (Teleport)
            {
                //teleportCD -= Time.deltaTime;
                if (Time.time > teleportCD)
                {
                    Instantiate(teleportVFXStart, transform.position, transform.rotation);
                    //transform.Translate(Vector3.forward * teleportDistance);

                    CC.excludeLayers = 1;
                    CC.Move(transform.forward * teleportDistance);
                    CC.excludeLayers = 0;
                    CC.Move(transform.forward * 0.1f);


                    Instantiate(teleportVFXEnd, transform.position, transform.rotation);
                    teleportCD = Time.time + 4;

                    Teleport = false; //for consumeable behaviour
                }
            }

            if (Ghost)
            {
                if (Time.time > ghostCD)
                {
                    Instantiate(ghostVFX, transform.position, transform.rotation);
                    CC.excludeLayers = 1;
                    currentMaxSpeed = startingSpeed * 1.25f;
                    speedBuffTimer = 5;
                    ghostCD = Time.time + 12;

                    Ghost = false; //for consumeable behaviour
                }
            }

            if (Invisibility)
            {
                //invisibilityduration -= Time.deltaTime;
                if (Time.time > invisibilityCD)
                {
                    for (int i = 0; i < bodyPartsGFX.Length; i++)
                    {
                        bodyPartsGFX[i].SetActive(false);
                    }
                    leaderCrown.SetActive(false);
                    propHideout.SetActive(true);
                    invisibilityduration = Time.time + 5;
                    invisibilityCD = Time.time + 11;
                }
            }
        }
        if (Invisibility)
        {

            if (Time.time < invisibilityduration && Time.time + Time.deltaTime >= invisibilityduration)
            {
                for (int i = 0; i < bodyPartsGFX.Length; i++)
                {
                    bodyPartsGFX[i].SetActive(true);
                }
                propHideout.SetActive(false);
                Leaderboard.singleton.FindLeader();

                Invisibility = false; //for consumeable behaviour
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
                if (attackMoveSpeed >= 0 && CC.enabled)
                    CC.Move(attackDirection * attackMoveSpeed * Time.deltaTime);

                attackMoveSpeed -= Time.deltaTime * 50;
            }

        }

        holdTimer -= Time.deltaTime;
        slowdownTimer -= Time.deltaTime;

        if (holdTimer <= 0)
        {
            strongFist.enabled = false;
        }

        /*
        windUpBar.value = Mathf.InverseLerp(reloadTime, 0, holdTimer);
        if (holdTimer <= 0)
            windUpBar.gameObject.SetActive(false);

        windUpBar.value = Mathf.InverseLerp(reloadTime, 0, slowdownTimer);
        if (slowdownTimer <= 0)
            windUpBar.gameObject.SetActive(false);
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

                if (projSearch[i].GetComponent<WeaponBase>().damageType == WeaponBase.damageTypes.grenade)
                    TakeDamage(attackWB.playerID, attackWB.damage, projSearch[i].transform.position);
                else if (projSearch[i].GetComponent<WeaponBase>().damageType == WeaponBase.damageTypes.zone)
                    TakeDamage(attackWB.damage);
                else
                    TakeDamage(attackWB.playerID, attackWB.damage, attackWB.damageType, projSearch[i].transform.position);

                if (projSearch[i].GetComponent<WeaponBase>().damageType == WeaponBase.damageTypes.destructableProjectile && attackWB.playerID != PlayerID)
                {
                    Destroy(projSearch[i].gameObject);
                }

            }
        }

    }

    private void DamageVisual()
    {
        if (paintHead)
        {
            paintAmount += Time.deltaTime * 500;
            //headColor = new Color32(headColor.r, (byte)(headColor.g+1000 * Time.deltaTime), (byte)(headColor.b+1000 *Time.deltaTime), headColor.a);
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
                //headColor = Color.grey;
                headColor = startingColor;
                HeadGFX.GetComponent<Renderer>().material.SetColor("_BaseColor", headColor);
                paintHead = false;

                hpBar.transform.localPosition = Vector3.zero;
                emptyHpBar.transform.localPosition = Vector3.zero;
                hpBar.transform.localScale = Vector3.one;
                emptyHpBar.transform.localScale = Vector3.one;

            }
        }

        //characterGFX.transform.localPosition /= (1 + Time.deltaTime * 10);

        //transform.position += knockbackDirection;
        if (CC.enabled)
            CC.Move(new Vector3(knockbackDirection.x, 0, knockbackDirection.y));
        //CC.SimpleMove(knockbackDirection);
        knockbackDirection /= (1 + Time.deltaTime * 10);


    }

    private void Movement()
    {
        if (moveInput != Vector2.zero)
        {
            if (speedBuffTimer <= 0) //&& speedBuffTimer>-1
            {
                currentMaxSpeed = startingSpeed;
                if (moveSpeed > currentMaxSpeed)
                    moveSpeed = currentMaxSpeed;
                CC.excludeLayers = 0;

                if (shieldBuffTimer > 0)
                    powerupSprite.sprite = onCharIcons[9];
                else
                    powerupSprite.sprite = null;
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
                //moveDirection = transform.forward;

                //moveDirection = transform.eulerAngles.y + (Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg);
                //moveDirection = new Vector3(Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad), 0 , Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad)).normalized;
                float angleCalc = transform.eulerAngles.y + (Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg);
                angleCalc *= Mathf.Deg2Rad;
                Vector3 vectorCalc = new Vector3(Mathf.Sin(angleCalc) , 0, Mathf.Cos(angleCalc) ).normalized;
                moveDirection = vectorCalc;
            }
            if (holdTimer <= 0) //can still rotate while shooting
            {
                //charAnim.Play("Runnig"); //CHANEL I'LL FUCKING MMURDER YOU reminder
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
                {
                    rFist.enabled = false;

                    //strongFist.enabled = false;
                }
                if (CC.enabled)
                    CC.Move(moveDirection * moveSpeed * Time.deltaTime);
            }
            else
            {
                charAnim.SetBool("Run", false);
                //charAnim.SetBool("RunGun", false);
                charAnim.SetBool("RunShotGun", false);
                //lFootAnim.SetBool("Walk", false);
                //rFootAnim.SetBool("Walk", false);
            }

            if (!mouseMovement)
                targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
            else
            {
                //targetAngle = transform.eulerAngles.y;

                targetAngle = transform.eulerAngles.y + (Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg);
            }

            if ((animState == AS.idle || animState == AS.Punch1Recovery || animState == AS.Punch2Recovery || animState == AS.Punch3Recovery || animState == AS.Sword1Recovery || animState == AS.Sword2Recovery) && !rolling) //can't rotate whiling using fists
            {
                transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            }
        }
        else
        {
            //lFootAnim.SetBool("Walk", false);
            //rFootAnim.SetBool("Walk", false);
            //charAnim.Play("Idle"); reminder


            if (moveSpeed > 1)
            {
                accelSpeed = 0;
                deAccelSpeed += Time.deltaTime * 5;
                moveSpeed -= deAccelSpeed;

                if (moveSpeed < 5)
                {
                    charAnim.SetBool("Run", false);
                    //charAnim.SetBool("RunGun", false);
                    //charAnim.SetBool("RunShotGun", false);
                }

                if (moveSpeed < 1)
                {
                    moveSpeed = 1;
                }

            }
        }

    }

    private void CharacterCollision()
    {
        //characterSearch = (Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y + 1.25f, transform.position.z), hitBoxSize / 1, transform.rotation, characterMask));
        characterSearch = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y + 1.25f, transform.position.z), 1.25f, characterMask);
        if (characterSearch.Length > 0)
        {
            for (int i = 0; i < characterSearch.Length; i++)
            {
                /*
                if (animState != 0 && characterSearch[i].GetComponent<CharacterControl>().PlayerID != PlayerID) // trying out collision with players while meleeing
                    attackMoveSpeed = 0;
                */

                if (moveInput != Vector2.zero)
                {
                    //characterSearch[i].GetComponent<CharacterController>().Move(moveDirection * moveSpeed * Time.deltaTime);
                    Vector3 pushDirection = characterSearch[i].transform.position - transform.position;
                    pushDirection.Normalize();
                    pushDirection /= 15;
                    pushDirection.y = 0;

                    characterSearch[i].GetComponent<CharacterController>().Move(pushDirection);
                    //characterSearch[i].transform.position += pushDirection;
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
            //rArm.localPosition = new Vector3(0.75f, 0, 0); //resets strong punch
            powerPunchWindup = 0;

            if (equippedWeapon == Weapons.Fist)  //no punch if other weapon equipped //Attack(equippedWeapon, previousWeapon);
            {
                if (animState == AS.idle || animState == AS.Punch3Recovery)
                {
                    //holdTimer = 0.383f; //can't move during attack windup & active, full animation is 0.75
                    holdTimer = 0.4166f;  // 25/60 chanel
                    attackDirection = transform.forward; //moveDirection
                    attackMoveSpeed = speedBuffTimer > 0 ? 16 : 16; //16 orignally //0 ? 30 : 24;
                    //forwardMomentumDelay = 0.133f; // 8/60 osher
                    forwardMomentumDelay = 0.166f; // 10/60 chanel


                    animTimer = 0;
                    animState = AS.Punch1Windup;
                    //lArmAnim.Play("Punch1");
                    //charAnim.Play("Punching");
                    charAnim.SetTrigger("Punch1");
                    SoundManager.singleton.PlayClip($"{HeadGFX.name}Melee1", transform.position, 1f, true, true);


                    /*
                    if (!(mouseMovement || isTargetDummy))
                        RumbleManager.instance.RumblePulse(0.2f, 0.2f, 0.5f, PI);
                    */
                }
                else if (animState == AS.Punch1Recovery) //animState == AS.Punch1Active ||
                {
                    holdTimer = 0.4166f; //can't move during attack windup & active, full animation is 0.75
                    attackDirection = transform.forward; //moveDirection
                    attackMoveSpeed = speedBuffTimer > 0 ? 17 : 17; //0 ? 30 : 24;
                    //forwardMomentumDelay = 0.233f; // 14/60 osher
                    forwardMomentumDelay = 0.166f; // 10/60 chanel

                    animTimer = 0;
                    animState = AS.Punch2Windup;
                    //lArmAnim.Play("Punch2");
                    //charAnim.Play("PunchingTwo");
                    charAnim.SetTrigger("Punch2");
                    //SoundManager.singleton.Melee2(transform.position);
                    SoundManager.singleton.PlayClip($"{HeadGFX.name}Melee2", transform.position, 1f, true, true);

                }
                else if (animState == AS.Punch2Recovery) //animState == AS.Punch2Active || 
                {
                    //holdTimer = 0.5166f; //can't move during attack windup & active, full animation is 0.75
                    holdTimer = 0.4166f; // 25/60
                    attackDirection = transform.forward; //moveDirection
                    attackMoveSpeed = speedBuffTimer > 0 ? 17 : 17; //0 ? 30 : 24;
                    //forwardMomentumDelay = 0.233f; // 14/60 osher
                    forwardMomentumDelay = 0.166f; // 10/60 chanel

                    animTimer = 0;
                    animState = AS.Punch3Windup;
                    //lArmAnim.Play("Punch3");
                    //charAnim.Play("PunchingThree");
                    charAnim.SetTrigger("Punch3");
                    //SoundManager.singleton.Melee3(transform.position);
                    SoundManager.singleton.PlayClip($"{HeadGFX.name}Melee3", transform.position, 1f, true, true);

                }
            }
            else if (equippedWeapon == Weapons.Sword)
            {
                weaponList[(int)Weapons.Sword].GetComponent<Sword>().desiredRotation = transform.rotation;
                if (animState == AS.idle || animState == AS.Sword2Recovery)
                {
                    holdTimer = 0.4166f;  // 25/60 chanel
                    attackDirection = transform.forward; //moveDirection
                    attackMoveSpeed = speedBuffTimer > 0 ? 30f : 30; //20 originally 0 ? 37.5f : 30;
                    forwardMomentumDelay = 0.166f; // 10/60 chanel

                    animTimer = 0;
                    animState = AS.Sword1Windup;
                    charAnim.SetTrigger("Sword1");
                    //SoundManager.singleton.Melee1(transform.position);
                    SoundManager.singleton.PlayClip($"{HeadGFX.name}Sword1", transform.position, 1f, true, true);

                }
                else if (animState == AS.Sword1Recovery) //animState == AS.Punch1Active ||
                {
                    holdTimer = 0.4166f;
                    attackDirection = transform.forward; //moveDirection
                    attackMoveSpeed = speedBuffTimer > 0 ? 30f : 30; //20 originally 0 ? 37.5f : 30;
                    forwardMomentumDelay = 0.166f; // 10/60 chanel

                    animTimer = 0;
                    animState = AS.Sword2Windup;
                    charAnim.SetTrigger("Sword2");
                    //SoundManager.singleton.Melee2(transform.position);
                    SoundManager.singleton.PlayClip($"{HeadGFX.name}Sword2", transform.position, 1f, true, true);
                }

            }
            else //using other weapons
            {
                /*
                if (holdTimer <= 0 && (equippedWeapon == Weapons.Gun || equippedWeapon == Weapons.Lazer )) //|| equippedWeapon == Weapons.Blunderbuss
                    charAnim.SetTrigger("Shoot");
                */

                Shoot(equippedWeapon); //                if (rightPunchAttackState ==0)

                //charAnim.Play("Shooting");

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
        if (animState != AS.idle) //&& rightPunchAttackState == 0
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
                    //rFist.enabled = false;
                    lFist.enabled = false;
                }
                else if (animTimer >= 0.35f) // 20/60 osher active  21/60 chanel 
                {
                    animState = AS.Punch2Active;
                    //rFist.enabled = true;
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
                {
                    strongFist.enabled = true;
                }
            }


            if (animTimer >= 0.5833f) // 45/60  35/60 chanel
            {
                animState = 0; //idle
                animTimer = 0;
                //lArmAnim.Play("Idle");
                //charAnim.Play("Idle"); reminder
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

        if (rightPunchAttackState == 1 && holdTimer <= 0) //&& equippedWeapon == Weapons.Fist
        {
            if (!chargeSFX)
            {
                SoundManager.singleton.PlayClip($"{HeadGFX.name}Charge", transform.position, 1f, true, true);
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
                    //charAnim.Play("StrongPunch");
                    charAnim.SetBool("StrongPunch", true);
                    //rArm.localPosition = new Vector3(0.75f, 0, -powerPunchWindup);
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
                attackDirection = transform.forward; //moveDirection
                attackMoveSpeed = speedBuffTimer > 0 ? 37f : 37; //46.25f
                strongPunchCharged.Stop();
                strongPunchPulse.Play("Waiting");
                //rFist.enabled = true;
                //strongFist.enabled = true;
                //strongTimer = 0.5f;


                //rArm.localPosition = new Vector3(0.75f, 0, 2 * powerPunchWindup);
                //rArm.localPosition = new Vector3(0.75f, 0, 0);
                //rArmAnim.Play("StrongPunch");
                //charAnim.Play("StrongPunchRelease");

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

                //charAnim.Play("StrongPunchRelease");
                //rArm.localPosition = new Vector3(0.75f, 0, 0);
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
                    //weaponList[(int)equippedWeapon].SetConfig(WeaponConfigList[(int)equippedWeapon]);
                    //weaponID = (int)equippedWeapon;

                    weaponSprite.sprite = onCharIcons[(int)equippedWeapon];

                    Destroy(pickupSearch[i].transform.gameObject);
                    return;
                }
                else if (pickupSearch[i].transform.name == "Coin" || pickupSearch[i].transform.name == "Coin2" || pickupSearch[i].transform.name == "Speed" || pickupSearch[i].transform.name == "Health" || pickupSearch[i].transform.name == "Shield" || pickupSearch[i].transform.name == "CannonBall" || pickupSearch[i].transform.name == "CoinSack")
                {
                    //Debug.Log(HeadGFX.name +" "+ pickupSearch[i].transform.name);

                    //SoundManager.singleton.Pickup(transform.position);
                    SoundManager.singleton.PlayClip("Pickup", transform.position, 0.15f, true, true);
                    if (pickupSearch[i].transform.name == "Coin")
                    {

                        moneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString() + "+" + "1";
                        Leaderboard.singleton.ModifyMoney(PlayerID, 1);
                        onCharMoneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString();
                        moneyAnim.Play("Player Coin Pickup", -1, 0);
                        pickupSearch[i].transform.gameObject.SetActive(false);
                        PickupManager.singleton.CoinPickupVFX(pickupSearch[i].transform.position);
                    }
                    else if (pickupSearch[i].transform.name == "Coin2")
                    {

                        moneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString() + "+" + "1";
                        Leaderboard.singleton.ModifyMoney(PlayerID, 1);
                        onCharMoneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString();
                        moneyAnim.Play("Player Coin Pickup", -1, 0);
                        pickupSearch[i].transform.gameObject.SetActive(false);
                        PickupManager.singleton.CoinPickupVFX(pickupSearch[i].transform.position);
                    }
                    else if (pickupSearch[i].transform.name == "CoinSack")
                    {

                        moneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString() + "+" + "3";
                        Leaderboard.singleton.ModifyMoney(PlayerID, 3);
                        onCharMoneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString();
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

                        powerupSprite.sprite = onCharIcons[8];

                        Destroy(pickupSearch[i].transform.gameObject);
                    }
                    else if (pickupSearch[i].transform.name == "Shield")
                    {
                        shieldGFX.SetActive(true);
                        shieldBuffTimer = 3.5f;

                        moneyText.text = "Shield";
                        moneyAnim.Play("Player Powerup Pickup", -1, 0);

                        powerupSprite.sprite = onCharIcons[9];

                        Destroy(pickupSearch[i].transform.gameObject);
                    }
                    /*
                    else if (pickupSearch[i].transform.name == "CannonBall")
                    {
                        cannonBallAmount = Int32.Parse(cannonBall.name);

                        moneyText.text = "Cannonball";
                        moneyAnim.Play("Player Powerup Pickup", -1, 0);


                        cannonBallAmount++;
                        cannonBall.name = "" + cannonBallAmount;
                        Destroy(pickupSearch[i].transform.gameObject);
                    }
                    */
                    return;
                }
                else
                {
                    Debug.Log("Change the object's name to the correct weapon or pickup name");
                }
            }
        }
        /*
        if (weaponDiscarded && discardingPlayerID == PlayerID)
        {
            weaponDiscarded = false;
            previousWeapon = equippedWeapon;
            equippedWeapon = Weapons.Fist;

            weaponList[(int)previousWeapon].SetActive(false);
            weaponList[(int)equippedWeapon].SetActive(true);
            //weaponID = (int)equippedWeapon;
            Debug.Log("actually discarded yo");
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
                    if (hp > 0 && hp - damage <= 0)
                    {
                        Leaderboard.singleton.AnnounceKill(attackingPlayer, PlayerID);
                        if (RoundManager.instance.areWeWarming == false)
                        {
                            //Leaderboard.singleton.ModifyMoney(attackingPlayer, 5); // giving money to the killer
                        }
                        OutTheRound();
                        DeadStop();
                    }

                    Leaderboard.singleton.StopForwardMomentum(attackingPlayer);

                    hp = hp - damage;
                    //hpText.text = ("HP: " + hp);
                    hpBar.fillAmount = hp / 10f;
                    //originalHpBarPos = hpBar.transform.position;
                    charAnim.SetTrigger("DMG");
                    //charAnim.SetTrigger("Roll");
                    //charAnim.SetTrigger("Punch1");
                    //charAnim.Play("Punching");
                    //charAnim.Play("Death");

                    if (!dead)
                    {
                        headColor = Color.red;
                        paintHead = true;
                    }


                    meleeParticleEffect.transform.position = hitPos;
                    meleeParticleEffect.Play();

                    //originalPos = characterGFX.transform.position;


                    knockbackDirection += new Vector2(transform.position.x - hitPos.x, transform.position.z - hitPos.z);
                    knockbackDirection.Normalize();
                    knockbackDirection *= 0.15f;
                    knockbackDirection *= (damage / 2f);
                    //knockbackDirection.y = 0;

                    /*
                    knockbackDirection = transform.position - hitPos;
                    Debug.Log("Original:" + knockbackDirection);
                    knockbackDirection.Normalize();
                    Debug.Log("Normalized:" + knockbackDirection);
                    knockbackDirection *= 0.2f;
                    knockbackDirection *= (damage / 2f);
                    //knockbackDirection.y = 0;
                    Debug.Log("Final:" + knockbackDirection);
                    */


                    //characterGFX.transform.position += knockbackDirection;

                    //transform.position += knockbackDirection;

                    //--------------------test-----------------------
                    /*
                    if (damageType == WeaponBase.damageTypes.melee)
                    {

                        //meleeParticleEffect.gameObject.SetActive(true);
                        meleeParticleEffect.Play();
                        meleeParticleEffect.transform.position = hitPos;
                    }
                    */

                    //SoundManager.singleton.Damage(transform.position);

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
                    if (RoundManager.instance.areWeWarming == false)
                    {
                        //Leaderboard.singleton.ModifyMoney(attackingPlayer, 5);
                    }
                    OutTheRound();
                    DeadStop();
                }


                hp = hp - damageBasedOnDistance;
                //hpText.text = ("HP: " + hp);
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

                /*
                knockbackDirection = transform.position - grenadePos;
                knockbackDirection.Normalize();
                knockbackDirection *= 0.2f;
                knockbackDirection *= (damage / 2f);
                knockbackDirection.y = 0;
                */
                //SoundManager.singleton.Damage(transform.position);

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

            //characterGFX.transform.position += knockbackDirection;
            //transform.position += knockbackDirection;

            lastPlayerID = attackingPlayer;
            identicalDamageCD = 0.1f;
            timeSinceLastPlayerHit = 0;
        }
        else if (attackingPlayer == PlayerID)
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
                    /* no money granted for killing self
                    if (RoundManager.instance.areWeWarming == false)
                    {
                        Leaderboard.singleton.ModifyMoney(attackingPlayer, 5);
                    }
                    */
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
                        if (RoundManager.instance.areWeWarming == false)
                        {
                            //Leaderboard.singleton.ModifyMoney(lastPlayerID, 5); // giving money to the killer
                        }
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

                //SoundManager.singleton.Damage(transform.position);
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
        //holdTimer = 0.15f; //default case if no change
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

                /*
                if (weaponList[(int)Weapons.Boomerang].GetComponent<Boomerang>().releasing)
                {
                    charAnim.SetTrigger("ThrowRelease");
                    weaponList[(int)Weapons.Boomerang].GetComponent<Boomerang>().releasing = false;
                }
                */
                //holdTimer = 0.2f;
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
                    //holdTimer = weaponList[(int)Weapons.Blunderbuss].GetComponent<Blunderbuss>().holdTime * 2;
                    slowdownTimer = weaponList[(int)Weapons.Blunderbuss].GetComponent<Blunderbuss>().holdTime * 2;

                    //windUpBar.gameObject.SetActive(true);
                    reloadBar.gameObject.SetActive(true);
                    //reloadTime = holdTimer;
                    reloadTime = slowdownTimer;
                }
                break;
            case Weapons.Grenade:  // 7
                if (weaponList[(int)Weapons.Grenade].GetComponent<Grenade>().charging)
                {
                    charAnim.SetBool("ThrowCharge", true);
                    holdTimer = 0.25f;
                }
                //holdTimer = 0.25f;
                break;
            default:
                Debug.Log("no");
                break;
        }

    }

    private void DeadStop()
    {
        paintAmount = 0; //making sure head isn't red
        //headColor = startingColor;
        HeadGFX.GetComponent<Renderer>().material.SetColor("_BaseColor", startingColor);
        paintHead = false;

        //cannonBallAmount = 0; //cannon balls don't transfer between rounds
        //cannonBall.name = "" + cannonBallAmount;
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
            //weaponID = (int)equippedWeapon;

            weaponSprite.sprite = onCharIcons[(int)equippedWeapon];

        }
        else
        {
            Debug.Log("Change the object's name to the correct weapon");
        }
    }

    public void BuyWeapon(WeaponConfig config)
    {
        switch (equippedWeapon)
        {

            case Weapons.Lazer:
                break;
            case Weapons.Mine:
                break;
            case Weapons.Blunderbuss:
                break;
            case Weapons.Grenade:
                break;
            case Weapons.Fist:
                break;
            case Weapons.Gun:
                break;
            case Weapons.Sword:
                break;
            case Weapons.Boomerang:
                break;
            default:
                break;
        }
    }

    public void BuyConsumeable(string shopConsumeableName)
    {
        if (shopConsumeableName == "Ghost")
        {
            Ghost = true;
            Invisibility = false;
            Teleport = false;
        }
        else if (shopConsumeableName == "Invisibility")
        {
            Ghost = false;
            Invisibility = true;
            Teleport = false;
        }
        else if (shopConsumeableName == "Teleport")
        {
            Ghost = false;
            Invisibility = false;
            Teleport = true;
        }
        else
        {
            Debug.Log("please enter the correct consumeable name in the shop");
        }
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
        onCharMoneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString();
        moneyAnim.Play("Player Coin Pickup", -1, 0);
        PickupManager.singleton.CoinPickupVFX(transform.position);
    }

    public void SetWinner()
    {
        winner = true;
        SoundManager.singleton.PlayClip($"{HeadGFX.name}WinRound", transform.position, 1f, true, false);
    }

    /*
    public static void DiscardWeapon(PlayerTypes weaponPlayerID)
    {
        discardingPlayerID = weaponPlayerID;
        weaponDiscarded = true;
        //Debug.Log("weapon discarded");
    }
    */

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

    public void DisplaySack()
    {
        moneyText.text = Leaderboard.singleton.GetMoney(PlayerID).ToString();
        //moneyAnim.Play("Player Coin Pickup", -1, 0);
        moneyAnim.enabled = false;
        moneyText.color = Color.white;
        moneyText.alpha = 1;

        //DON"T RECOMMEND DOING THIS
        moneyAnim.gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
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


/**
if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1.25f, transform.position.z), transform.forward, out pickupHit, 1f, pickupMask))
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
            moneyText.text = coins.ToString();
            //Destroy(pickupHit.transform.gameObject); //need to think this through with the pickup manager
            pickupHit.transform.gameObject.SetActive(false);
            //Debug.Log("coin");
        }
        if (pickupHit.transform.name == "Health")
        {
            hp += 3;
            //hpText.text = "HP: " + hp;
            hpBar.fillAmount += 3f / 10f;
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
*/

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


/* I hate myself
 *             speed = Vector2.SqrMagnitude(Mouse.current.position.value - prevMousePos);
            if (speed > prevSpeed)
                accel++;
            else
                accel--;

            //Debug.Log(accel);
            float distance = Mathf.InverseLerp(1, 10, accel);


            if (accel>0)
                mouseIndicator.transform.localPosition = Vector3.right * distance;
            else
            {
                mouseIndicator.transform.localPosition = Vector3.zero;
                accel = 0;
            }
                

            prevSpeed = speed;
*/