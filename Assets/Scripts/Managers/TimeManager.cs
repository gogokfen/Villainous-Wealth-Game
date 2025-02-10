using UnityEngine;
public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    float timer;
    bool start;
    private void Awake()
    {
        instance = this;
    }
    void Update()
    {
        if (start)
        {
            timer -= Time.unscaledDeltaTime;
            if (timer <= 0)
            {
                start = false;
            }
        }
        else 
        {
            if (Time.timeScale < 1f  && !PauseMenu.instance.paused)
            {
                Time.timeScale += Time.unscaledDeltaTime * 1.5f;
                if (Time.timeScale > 1f)
                {
                    Time.timeScale = 1;
                }
            }
        }
    }
    public void SlowTime(float timeScale, float duration)
    {
        Time.timeScale = timeScale;
        timer = duration;
        start = true;
    }

    public void StopTime()
    {
        Time.timeScale = 0;
        timer = float.MaxValue;
        start = true;
    }
    public void ResumeTime()
    {
        Time.timeScale = 1;
        timer = 0;
        start = false;
    }
}
