using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Pirate : MonoBehaviour
{
    [SerializeField] GameObject cannonV2GameObject;
    [SerializeField] CannonV2 cannonV2Script;
    [SerializeField] PlayerInput PI;
    [SerializeField] int price;

    private float payCD;
    private float AttackCD;

    private CharacterControl payingPlayerRef;

    [SerializeField] LayerMask collisionMask;
    Collider[] playerSearch;
    //Vector3 hitBox = new Vector3(10f, 10f, 10f); //new Vector3(1f, 0.75f, 0.75f)

    [SerializeField] GameObject[] shipPartsGFX;
    private bool outlineActive;

    [SerializeField] GameObject GFX;
    [SerializeField] GameObject target;
    private Quaternion prevRotation;

    [SerializeField] GameObject completePiratePackage;

    [SerializeField] GameObject payingPirate;

    //[SerializeField] GameObject payUI;
    [SerializeField] Animator payUI;
    void Start()
    {
        PI.enabled = false;
    }


    void Update()
    {
        if (PI.enabled)
        {
            //GFX.transform.LookAt(targetTransform.transform);

            prevRotation = GFX.transform.rotation;
            GFX.transform.LookAt(target.transform);

            GFX.transform.rotation = Quaternion.Lerp(prevRotation, GFX.transform.rotation, Time.deltaTime * 0.25f);

        }

        //playerSearch = Physics.OverlapBox(transform.position, hitBox, Quaternion.identity, collisionMask); //half extents
        playerSearch = Physics.OverlapSphere(transform.position, 15, collisionMask);

        if (playerSearch.Length > 0)
        {
            if (!outlineActive)
            {
                //payUI.SetActive(true);
                payUI.SetBool("On", true);

                outlineActive = true;
                for (int i = 0; i < shipPartsGFX.Length; i++)
                {
                    shipPartsGFX[i].GetComponent<Outline>().OutlineWidth = 1;
                }
            }


            for (int i = 0; i < playerSearch.Length; i++)
            {
                if (Leaderboard.singleton.GetMoney(playerSearch[i].GetComponent<CharacterControl>().PlayerID)>=price && Time.time>=payCD)
                {
                    cannonV2GameObject.transform.position = transform.position;

                    SoundManager.singleton.PlayClip($"Pay", transform.position, 0.4f, false, false); //return with sound

                    payingPlayerRef = playerSearch[i].GetComponent<CharacterControl>();
                    //payingPlayerRef.StopAnimator();
                    payingPlayerRef.enabled = false;
                    payingPlayerRef.gameObject.SetActive(false);


                    Leaderboard.singleton.ModifyMoney((payingPlayerRef.PlayerID), -price);
                    cannonV2GameObject.SetActive(true);
                    cannonV2Script.UpdateShooter(payingPlayerRef.PlayerID);

                    PI.enabled = true;

                    if (SceneManager.GetActiveScene().name != "OsherScene")
                        CameraManager.instance.RemoveFromCameraGroup(payingPirate);


                    //PI.user = 0;
                    //PI.SwitchCurrentControlScheme("Mouse & Keyboard",InputDevice[0].device);



                    payCD = Time.time + 18;
                    AttackCD = Time.time + 10;
                }
            }
        }
        else if (outlineActive)
        {
            //payUI.SetActive(false);
            payUI.SetBool("On", false);
            outlineActive = false;
            for (int i = 0; i < shipPartsGFX.Length; i++)
            {
                shipPartsGFX[i].GetComponent<Outline>().OutlineWidth = 0.2f;
            }
        }

        if (Time.time>=AttackCD && payingPlayerRef!=null)
        {
            PI.enabled = false;

            cannonV2GameObject.SetActive(false);
            payingPlayerRef.gameObject.SetActive(true); //consider making Character Control "Dead Stop" public and use it to fix animation bug
            payingPlayerRef.enabled = true;
            payingPlayerRef = null;
            //payingPlayerRef.EnableAnimator();
            //payingPlayerRef.DeadStop();


            //considering making the pay available once per round
            //gameObject.SetActive(false);
            completePiratePackage.SetActive(false);
        }
    }
}
