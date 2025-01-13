using UnityEngine;
using MelenitasDev.SoundsGood;

public class NewSoundManagerTest : MonoBehaviour
{
    //Sound sound = new ($"{GameObject.FindAnyObjectByType<CharacterControl>().HeadGFX.name}");
    Sound sound;
    private void Start() 
    {
     sound = new ("MafiaBoss");   
    }

    private void Update() 
    {
        //sound.Play();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SoundManager.singleton.PlayClip("MafiaBoss", transform.position, 1f, true);
        }
    }
}
