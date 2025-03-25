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

    [SerializeField] GameObject[] shipPartsGFX;
    private bool outlineActive;

    [SerializeField] GameObject GFX;
    [SerializeField] GameObject target;
    private Quaternion prevRotation;

    [SerializeField] GameObject completePiratePackage;

    [SerializeField] GameObject payingPirate;

    [SerializeField] Animator payUI;

    private Animator anchorAnim;

    [SerializeField] GameObject addToGroup;
    private float timer;

    private InputDevice payingDevice;
    private string payingScheme;
    void Start()
    {
        PI.enabled = false;
        anchorAnim = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        timer = Time.time + 8;
    }

    private void OnDisable()
    {
        addToGroup.SetActive(false);
    }
    void Update()
    {
        //Debug.Log(Time.time);
        //Debug.Log("Timer: " + timer);

        if (PI.enabled)
        {
            prevRotation = GFX.transform.rotation;
            GFX.transform.LookAt(target.transform);

            GFX.transform.rotation = Quaternion.Lerp(prevRotation, GFX.transform.rotation, Time.deltaTime * 0.25f);
        }

        if (Time.time>timer)
        {
            timer = float.MaxValue;
            addToGroup.SetActive(true);
            anchorAnim.Play("Anchor Animation");
        }

        playerSearch = Physics.OverlapSphere(transform.position, 15, collisionMask);

        if (playerSearch.Length > 0)
        {
            if (!outlineActive)
            {
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

                    SoundManager.singleton.PlayClip($"Pay", Vector3.zero, 0.4f, false, false); //return with sound

                    payingPlayerRef = playerSearch[i].GetComponent<CharacterControl>();
                    payingPlayerRef.enabled = false;
                    payingDevice = playerSearch[i].GetComponent<PlayerInput>().devices[0];
                    payingScheme = playerSearch[i].GetComponent<PlayerInput>().currentControlScheme;


                    payingPlayerRef.gameObject.SetActive(false);

                    Leaderboard.singleton.ModifyMoney((payingPlayerRef.PlayerID), -price);
                    cannonV2GameObject.SetActive(true);
                    cannonV2Script.UpdateShooter(payingPlayerRef.PlayerID);

                    PI.enabled = true;
                    PI.SwitchCurrentControlScheme(payingScheme, payingDevice);

                    if (SceneManager.GetActiveScene().name != "OsherScene")
                        CameraManager.instance.RemoveFromCameraGroup(payingPirate);

                    payCD = Time.time + 18;
                    AttackCD = Time.time + 10;
                }
            }
        }
        else if (outlineActive)
        {
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
            payingPlayerRef.gameObject.SetActive(true);
            payingPlayerRef.DeadStop();
            payingPlayerRef.enabled = true;
            payingPlayerRef.gameObject.GetComponent<PlayerInput>().SwitchCurrentControlScheme(payingScheme,payingDevice);
            payingPlayerRef = null;

            completePiratePackage.SetActive(false);
        }
    }
}
