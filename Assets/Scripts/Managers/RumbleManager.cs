using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleManager : MonoBehaviour
{
    public static RumbleManager instance;
    private Gamepad pad;
    private void Awake()
    {
        instance = this;
    }
 public void RumblePulse(float lowFrequency, float highFrequency, float duration, PlayerInput input)
    {
        if (input.currentControlScheme == "Keyboard & Mouse") return;
        pad = input.devices[0] as Gamepad;
        pad.SetMotorSpeeds(lowFrequency, highFrequency);
        StartCoroutine(StopRumble(duration, pad));
    }

    private IEnumerator StopRumble(float duration, Gamepad pad)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        pad.SetMotorSpeeds(0f, 0f);
    }
}
