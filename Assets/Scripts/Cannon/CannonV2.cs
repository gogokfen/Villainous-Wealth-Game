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

    private Vector3 shootLocationModifier;

    private Vector3 originalLocation;

    private float shotTimer;

    private CharacterControl.PlayerTypes shooterColor;

    void Start()
    {
        originalLocation = transform.localPosition;
    }

    void Update()
    {
        if (Time.time>=shotTimer)
        {
            shootLocationModifier = Vector3.forward * Random.Range(-shootingOffset, shootingOffset) + Vector3.right * Random.Range(-shootingOffset, shootingOffset);

            shootingSpot.localPosition += shootLocationModifier;

            transform.LookAt(shootingSpot);

            GameObject tempCannonShot = Instantiate(cannonShot, transform.position, transform.rotation);
            tempCannonShot.GetComponent<WeaponBase>().playerID = shooterColor;

            Instantiate(bombIndicatorVFX,shootingSpot.position, Quaternion.identity); //shootLocationModifier + shootingSpot.position
            SoundManager.singleton.PlayClip("CannonShot", transform.position, 0.5f, true, true);

            shootingSpot.localPosition = new Vector3(0,0.05f,0);

            shotTimer = Time.time + Random.Range(shootingFrequency/2f, shootingFrequency);
        }
    }

    public void UpdateShooter(CharacterControl.PlayerTypes shooterColor)
    {
        this.shooterColor = shooterColor;

        if (this.shooterColor == CharacterControl.PlayerTypes.Red)
            shootingSpot.gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", new Color32(204, 67, 152, 255));

        else if (this.shooterColor == CharacterControl.PlayerTypes.Green)
            shootingSpot.gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", new Color32(96, 196, 71, 255));

        else if (this.shooterColor == CharacterControl.PlayerTypes.Blue)
            shootingSpot.gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", new Color32(54, 111, 218, 255));

        else if (this.shooterColor == CharacterControl.PlayerTypes.Yellow)
            shootingSpot.gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", new Color32(51, 189, 190, 255));

    }
}
