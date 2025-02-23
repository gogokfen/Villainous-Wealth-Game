using System.Collections;
using System.Collections.Generic;
using MelenitasDev.SoundsGood;
using UnityEngine;

public class RoundStart : MonoBehaviour
{
    private int spawnAnimIndex;
    private Animator[] playerAnimators;
    private CharacterControl[] characterReferences;
    public bool playing;
    public IEnumerator Intro()
    {
        spawnAnimIndex = 0;
        playing = true;
        characterReferences = Leaderboard.singleton.GetPlayerReferences();
        playerAnimators = Leaderboard.singleton.GetPlayerAnimators();
        Leaderboard.singleton.DisableCharacterController();
        Leaderboard.singleton.DisableCharacterControl();
        SoundManager.singleton.PlayClipSFX(SFX.AnnouncerRoundStart, transform.position, 1, false, false);
        StartCoroutine(AnnounceInSequence());

        for (int i = 0; i < playerAnimators.Length; i++)
        {
            characterReferences[spawnAnimIndex].characterGFX.SetActive(true);
            StartCoroutine(PlaySpawnAnimation(spawnAnimIndex));
            yield return new WaitForSeconds(0.75f);
            spawnAnimIndex++;
        }
        playing = false;
        
    }

    private IEnumerator PlaySpawnAnimation(int index)
    {
        Animator anim = playerAnimators[index];
        anim.Play($"Spawning{index}");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName($"Spawning{index}"));
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !anim.IsInTransition(0));

        anim.Play("Idle");
    }

    private IEnumerator AnnounceInSequence()
    {
        Leaderboard.singleton.AnnounceTextRoundStart("3");
        yield return new WaitForSeconds(1f);
        Leaderboard.singleton.AnnounceTextRoundStart("2");
        yield return new WaitForSeconds(1f);
        Leaderboard.singleton.AnnounceTextRoundStart("1");
        yield return new WaitForSeconds(1f);
        Leaderboard.singleton.AnnounceTextRoundStart("Go!");
        Leaderboard.singleton.EnableCharacterController();
        Leaderboard.singleton.EnableCharacterControl();
    }
}
