using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using VInspector;

public class Mine : WeaponBase
{
    [Foldout("Upgrades")]
    public bool GasCloudUpgrade = false;
    [EndFoldout]

    [SerializeField] GameObject mineGFX;
    [SerializeField] GameObject minePrefab;
    [SerializeField] int startingAmmo;
    int uses;
    [SerializeField] float mineCD = 2f;
    private float mineTimer;
    //bool placed;

    [HideInInspector] public bool placing = false;

    bool weaponActive = false;

    [SerializeField] Slider windUpSlider;


    private void Start()
    {
        damageType = damageTypes.destructableProjectile;
    }

    private void OnEnable()
    {
        uses = startingAmmo;
        weaponActive = true;
        mineGFX.SetActive(true);
    }

    private void OnDisable()
    {
        weaponActive = false;
        mineGFX.SetActive(false);
    }

    public void PlacingMine(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && weaponActive)
        {

            //placed = context.action.triggered;
            PlaceMine();
        }
    }

    private void PlaceMine()
    {
        if (uses > 0 && mineTimer<=0)
        {
            mineTimer = mineCD;
            uses--;
            GameObject tempMine = Instantiate(minePrefab, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity, null);
            tempMine.GetComponent<WeaponBase>().playerID = playerID;
            tempMine.GetComponent<WeaponBase>().damage = damage;
            tempMine.GetComponent<WeaponBase>().damageType = damageType;

            if (GasCloudUpgrade)
                tempMine.GetComponent<MineShot>().GasCloudUpgrade = true;

            mineGFX.SetActive(false);

            /*
            if (uses <= 0) //out of mines, refill and discard weapon
            {
                CharacterControl.DiscardWeapon(playerID);
                //uses = startingAmmo;
            }
            */

            //SoundManager.singleton.MinePlace(transform.position);

            placing = true;
        }
    }

    void Update()
    {
        if (mineTimer>0)
        {
            windUpSlider.gameObject.SetActive(true);
            windUpSlider.value = Mathf.InverseLerp(mineCD, 0, mineTimer);

            mineTimer -= Time.deltaTime;
            if (mineTimer<=0)
            {
                mineGFX.SetActive(true);
                windUpSlider.gameObject.SetActive(false);
            }
                
        }


        //if (placed)
        //{
        //placed = false;
        // if (uses>0)
        // {
        //     uses--;
        //     GameObject tempMine = Instantiate(minePrefab,new Vector3(transform.position.x,0,transform.position.z),Quaternion.identity,null);
        //     tempMine.GetComponent<WeaponBase>().playerID = playerID;
        //     tempMine.GetComponent<WeaponBase>().damage = damage;
        //     tempMine.GetComponent<WeaponBase>().damageType = damageType;
        //     if (uses<=0) //out of mines, refill and discard weapon
        //     {
        //         uses = startingAmmo;
        //         CharacterControl.DiscardWeapon();
        //     }
        // }

        //}
    }
}
