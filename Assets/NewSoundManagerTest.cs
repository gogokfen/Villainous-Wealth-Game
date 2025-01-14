using UnityEngine;
using MelenitasDev.SoundsGood;

public class NewSoundManagerTest : MonoBehaviour
{
    //Sound sound = new ($"{GameObject.FindAnyObjectByType<CharacterControl>().HeadGFX.name}");
    Sound sound;
    private void Update()
    {
        //sound.Play();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SoundManager.singleton.PlayClip("Melee1", transform.position, 1f, true, true);
        }
    }
}
