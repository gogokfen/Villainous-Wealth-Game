using System.Collections;
using System.Collections.Generic;
using MelenitasDev.SoundsGood;
using Unity.VisualScripting;
using UnityEngine;

public class RoundStart : MonoBehaviour
{
    private int spawnAnimIndex;
    private Animator[] playerAnimators;
    private CharacterControl[] characterReferences;
    public bool playing;
    Sound announcerRound;
    float duration;
    public IEnumerator Intro()
    {
        spawnAnimIndex = 0;
        playing = true;
        characterReferences = Leaderboard.singleton.GetPlayerReferences();
        playerAnimators = Leaderboard.singleton.GetPlayerAnimators();
        Leaderboard.singleton.DisableCharacterController();
        Leaderboard.singleton.DisableCharacterControl();
        //SoundManager.singleton.PlayClipSFX(SFX.AnnouncerRoundStart, transform.position, 1, false, false);
        StartCoroutine(AnnounceInSequence());

        for (int i = 0; i < playerAnimators.Length; i++)
        {
            characterReferences[spawnAnimIndex].characterGFX.SetActive(true);
            StartCoroutine(PlaySpawnAnimation(spawnAnimIndex));
            yield return new WaitForSeconds(0.75f);
            spawnAnimIndex++;
        }
        //playing = false;

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

        //Leaderboard.singleton.AnnounceTextRoundStart($"Round {RoundManager.instance.currentRound + 1} / {RoundManager.instance.totalRounds}");
        WhichRoundSound();
        yield return new WaitForSeconds(duration);
        SoundManager.singleton.PlayClipSFX(SFX.AnnouncerRoundStart, transform.position, 1, false, false);
        Leaderboard.singleton.AnnounceTextRoundStart("3");
        yield return new WaitForSeconds(1f);
        Leaderboard.singleton.AnnounceTextRoundStart("2");
        yield return new WaitForSeconds(1f);
        Leaderboard.singleton.AnnounceTextRoundStart("1");
        yield return new WaitForSeconds(1f);
        Leaderboard.singleton.AnnounceTextRoundStart("Get Rich!");
        Leaderboard.singleton.EnableCharacterController();
        Leaderboard.singleton.EnableCharacterControl();
        playing = false;
    }

    private void WhichRoundSound()
    {
        if (RoundManager.instance.currentRound + 1 == 1)
        {
            announcerRound = new(SFX.AnnouncerRound1);
            Leaderboard.singleton.AnnounceTextRoundStart($"Round {RoundManager.instance.currentRound + 1} / {RoundManager.instance.totalRounds}");
        }
        else if (RoundManager.instance.currentRound + 1 == 2)
        {
            announcerRound = new(SFX.AnnouncerRound2);
            Leaderboard.singleton.AnnounceTextRoundStart($"Round {RoundManager.instance.currentRound + 1} / {RoundManager.instance.totalRounds}");
        }
        else if (RoundManager.instance.currentRound + 1 == 3)
        {
            announcerRound = new(SFX.AnnouncerRound3);
            Leaderboard.singleton.AnnounceTextRoundStart($"Round {RoundManager.instance.currentRound + 1} / {RoundManager.instance.totalRounds}");
        }
        else if (RoundManager.instance.currentRound + 1 == 4)
        {
            announcerRound = new(SFX.AnnouncerRound4);
            Leaderboard.singleton.AnnounceTextRoundStart("Final Round");
        }
        announcerRound.Play();
        duration = announcerRound.ClipDuration;
        //announcerRound.OnComplete(FinishedAnnouncing);
    }
}
