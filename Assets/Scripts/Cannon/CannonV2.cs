using System.Collections;
using System.Collections.Generic;
using MelenitasDev.SoundsGood;
using UnityEngine;

public class CannonV2 : MonoBehaviour
{
    [SerializeField] Transform shootingSpot;
    [SerializeField] GameObject cannonShot;
    [SerializeField] GameObject bombIndicatorVFX;

    [SerializeField] float shootingOffset = 2;
    [SerializeField] float shootingFrequency = 0.5f;

    Vector3 shootLocationModifier;

    private Vector3 originalLocation;

    private float shotTimer;

    CharacterControl.PlayerTypes shooterColor;

    void Start()
    {
        originalLocation = transform.localPosition;
    }

    
    void Update()
    {
        //transform.LookAt(shootingSpot);

        if (Time.time>=shotTimer)
        {
            shootLocationModifier = Vector3.forward * Random.Range(-shootingOffset, shootingOffset) + Vector3.right * Random.Range(-shootingOffset, shootingOffset);

            //transform.localPosition = originalLocation + shootLocationModifier;

            //Vector3 originalPos
            shootingSpot.localPosition += shootLocationModifier;

            transform.LookAt(shootingSpot);

            GameObject tempCannonShot = Instantiate(cannonShot, transform.position, transform.rotation);
            tempCannonShot.GetComponent<WeaponBase>().playerID = shooterColor;

            Instantiate(bombIndicatorVFX,shootingSpot.position, Quaternion.identity); //shootLocationModifier + shootingSpot.position
            SoundManager.singleton.PlayClip("CannonShot", transform.position, 0.5f, true, true);

            shootingSpot.localPosition = Vector3.zero;

            shotTimer = Time.time + Random.Range(shootingFrequency/2f, shootingFrequency);
        }
    }

    public void UpdateShooter(CharacterControl.PlayerTypes shooterColor)
    {
        this.shooterColor = shooterColor;
    }

}
