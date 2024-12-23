using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class SoundManager : MonoBehaviour
{
    public static SoundManager singleton { get; private set; }

    AudioSource sfxAS;
    public AudioSource musicAS;

    public AudioClip[] musicClips;
    public AudioClip maloMart;
    private int currentClipIndex = 0;

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
        sfxAS = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        singleton = this;
    }
    public void AnnounceBoxhead()
    {
        sfxAS.PlayOneShot(announcerBoxhead);
    }
    public void AnnounceDragon()
    {
        sfxAS.PlayOneShot(announcerDragon);
    }
    public void AnnounceTestDummy()
    {
        sfxAS.PlayOneShot(announcerTestDummy);
    }
    public void AnnounceMonopolyDude()
    {
        sfxAS.PlayOneShot(announcerMonopolyDude);
    }

    public void Melee1(Vector3 soundLocation)
    {
        sfxAS.PlayOneShot(melee1);
        transform.position = soundLocation;
    }

    public void Melee2(Vector3 soundLocation)
    {
         sfxAS.PlayOneShot(melee2);
        transform.position = soundLocation;
    }

    public void Melee3(Vector3 soundLocation)
    {
        sfxAS.PlayOneShot(melee3);
        transform.position = soundLocation;
    }

    public void GunShot(Vector3 soundLocation)
    {
        sfxAS.PlayOneShot(gunShot);
        transform.position = soundLocation;
    }
    public void LazerGunShot(Vector3 soundLocation)
    {
        sfxAS.PlayOneShot(lazerGunShot);
        transform.position = soundLocation;
    }
    public void BlunderbussShot(Vector3 soundLocation)
    {
        sfxAS.PlayOneShot(blunderbussShot);
        transform.position = soundLocation;
    }
    public void BlunderbussReload(Vector3 soundLocation)
    {
        sfxAS.PlayOneShot(blunderbussReload);
        transform.position = soundLocation;
    }
    public void BoomerangThrow(Vector3 soundLocation)
    {
        sfxAS.PlayOneShot(boomerangThrow);
        transform.position = soundLocation;
    }
    public void BoomerangCatch(Vector3 soundLocation)
    {
        sfxAS.PlayOneShot(boomerangCatch);
        transform.position = soundLocation;
    }
    public void MinePlace(Vector3 soundLocation)
    {
        sfxAS.PlayOneShot(minePlace);
        transform.position = soundLocation;
    }
    public void MineExplode(Vector3 soundLocation)
    {
        sfxAS.PlayOneShot(mineExplode);
        transform.position = soundLocation;
    }
    public void BombThrow(Vector3 soundLocation)
    {
        sfxAS.PlayOneShot(bombThrow);
        transform.position = soundLocation;
    }
    public void BombExplode(Vector3 soundLocation)
    {
        sfxAS.PlayOneShot(bombExplode);
        transform.position = soundLocation;
    }

    public void Damage(Vector3 soundLocation)
    {
        transform.position = soundLocation;
        int random = Random.Range(0, 3);
        if (random == 0)
            sfxAS.PlayOneShot(takeDamage1);
        if (random == 1)
            sfxAS.PlayOneShot(takeDamage2);
        if (random == 2)
            sfxAS.PlayOneShot(takeDamage3);
    }
    public void Death(Vector3 soundLocation)
    {
        sfxAS.PlayOneShot(death);
        transform.position = soundLocation;
    }
    public void Pickup(Vector3 soundLocation)
    {
        transform.position = soundLocation;
        int random = Random.Range(0, 3);
        if (random == 0)
            sfxAS.PlayOneShot(coinPickup1);
        if (random == 1)
            sfxAS.PlayOneShot(coinPickup2);
        if (random == 2)
            sfxAS.PlayOneShot(coinPickup3);
    }
    public void CannonShot(Vector3 soundLocation)
    {
        transform.position = soundLocation;
        sfxAS.PlayOneShot(cannonShot);
    }
    public void ChestOpen(Vector3 soundLocation)
    {
        transform.position = soundLocation;
        sfxAS.PlayOneShot(chestOpen);
    }
    public void Roll(Vector3 soundLocation)
    {
        transform.position = soundLocation;
        sfxAS.PlayOneShot(roll);
    }
    public void Shield(Vector3 soundLocation)
    {
        transform.position = soundLocation;
        sfxAS.PlayOneShot(shield);
    }

    public void PlayNextClip()
    {
        currentClipIndex = (currentClipIndex + 1) % musicClips.Length;
        musicAS.clip = musicClips[currentClipIndex];
        musicAS.Play();
    }

    void Update()
    {
        if (!musicAS.isPlaying)
        {
            PlayNextClip();
        }
    }

    public void MaloMart()
    {
        musicAS.clip = maloMart;
        musicAS.Play();
    }
}
