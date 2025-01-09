using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonV2 : MonoBehaviour
{
    [SerializeField] Transform shootingSpot;
    [SerializeField] GameObject cannonShot;

    [SerializeField] float shootingOffset = 2;
    [SerializeField] float shootingFrequency = 0.5f;

    private Vector3 originalLocation;

    private float shotTimer;

    CharacterControl.PlayerTypes shooterColor;

    void Start()
    {
        originalLocation = transform.localPosition;
    }

    
    void Update()
    {
        transform.LookAt(shootingSpot);

        if (Time.time>=shotTimer)
        {
            transform.localPosition = originalLocation + Vector3.forward * Random.Range(-shootingOffset, shootingOffset) + Vector3.right * Random.Range(-shootingOffset, shootingOffset); 

            GameObject tempCannonShot = Instantiate(cannonShot, transform.position, transform.rotation);
            tempCannonShot.GetComponent<WeaponBase>().playerID = shooterColor;
            

            shotTimer = Time.time + Random.Range(shootingFrequency/2f, shootingFrequency);
        }
    }

    public void UpdateShooter(CharacterControl.PlayerTypes shooterColor)
    {
        this.shooterColor = shooterColor;
    }

}
