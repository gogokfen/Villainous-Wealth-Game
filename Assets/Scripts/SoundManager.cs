using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class SoundManager : MonoBehaviour
{
    public static SoundManager singleton { get; private set; }

    AudioSource AS;

    [Tab("Melee")]
    [SerializeField] AudioClip melee1;
    [SerializeField] AudioClip melee2;
    [SerializeField] AudioClip melee3;

    [Tab("Gunshots")]
    [SerializeField] AudioClip gunShot;
    [SerializeField] AudioClip lazerGunShot;
    [SerializeField] AudioClip blunderbussShot;
    [SerializeField] AudioClip blunderbussReload;

    [Tab("Boomerang")]
    [SerializeField] AudioClip boomerangThrow;
    [SerializeField] AudioClip boomerangCatch;

    [Tab("Mine")]
    [SerializeField] AudioClip minePlace;
    [SerializeField] AudioClip mineExplode;

    [Tab("Bomb")]
    [SerializeField] AudioClip bombThrow;
    [SerializeField] AudioClip bombExplode;

    [Tab("Taking Damage")]
    [SerializeField] AudioClip takeDamage1;
    [SerializeField] AudioClip takeDamage2;
    [SerializeField] AudioClip takeDamage3;

    [Tab("Death")]
    [SerializeField] AudioClip death;

    [Tab("Coins")]
    [SerializeField] AudioClip coinPickup1;
    [SerializeField] AudioClip coinPickup2;
    [SerializeField] AudioClip coinPickup3;

    [Tab("Menu")]
    [SerializeField] AudioClip announcerBoxhead;
    [SerializeField] AudioClip announcerDragon;
    [SerializeField] AudioClip announcerMonopolyDude;
    [SerializeField] AudioClip announcerTestDummy;


    [Tab("Misc")]
    [SerializeField] AudioClip roll;
    [SerializeField] AudioClip shield;
    [SerializeField] AudioClip chestOpen;
    [SerializeField] AudioClip cannonShot;

    void Start()
    {
        AS = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        singleton = this;
    }
    public void AnnounceBoxhead()
    {
        AS.PlayOneShot(announcerBoxhead);
    }
    public void AnnounceDragon()
    {
        AS.PlayOneShot(announcerDragon);
    }
    public void AnnounceTestDummy()
    {
        AS.PlayOneShot(announcerTestDummy);
    }
    public void AnnounceMonopolyDude()
    {
        AS.PlayOneShot(announcerMonopolyDude);
    }

    public void Melee1()
    {
        AS.PlayOneShot(melee1);
    }

    public void Melee2()
    {
         AS.PlayOneShot(melee2);
    }

    public void Melee3()
    {
        AS.PlayOneShot(melee3);
    }

    public void GunShot()
    {
        AS.PlayOneShot(gunShot);
    }
    public void LazerGunShot()
    {
        AS.PlayOneShot(lazerGunShot);
    }
    public void BlunderbussShot()
    {
        AS.PlayOneShot(blunderbussShot);
    }
    public void BlunderbussReload()
    {
        AS.PlayOneShot(blunderbussReload);
    }
    public void BoomerangThrow()
    {
        AS.PlayOneShot(boomerangThrow);
    }
    public void BoomerangCatch()
    {
        AS.PlayOneShot(boomerangCatch);
    }
    public void MinePlace()
    {
        AS.PlayOneShot(minePlace);
    }
    public void MineExplode()
    {
        AS.PlayOneShot(mineExplode);
    }
    public void BombThrow()
    {
        AS.PlayOneShot(bombThrow);
    }
    public void BombExplode()
    {
        AS.PlayOneShot(bombExplode);
    }

    public void Damage()
    {
        int random = Random.Range(0, 3);
        if (random == 0)
            AS.PlayOneShot(takeDamage1);
        if (random == 1)
            AS.PlayOneShot(takeDamage2);
        if (random == 2)
            AS.PlayOneShot(takeDamage3);
    }
    public void Death()
    {
        AS.PlayOneShot(death);
    }
    public void Pickup()
    {
        int random = Random.Range(0, 3);
        if (random == 0)
            AS.PlayOneShot(coinPickup1);
        if (random == 1)
            AS.PlayOneShot(coinPickup2);
        if (random == 2)
            AS.PlayOneShot(coinPickup3);
    }
    public void CannonShot()
    {
        AS.PlayOneShot(cannonShot);
    }
    public void ChestOpen()
    {
        AS.PlayOneShot(chestOpen);
    }
    public void Roll()
    {
        AS.PlayOneShot(roll);
    }
    public void Shield()
    {
        AS.PlayOneShot(shield);
    }
}
