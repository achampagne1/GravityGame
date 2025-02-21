using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTimer
{
    private float timer = 0f;
    private float timerDuration = 1000f;
    public void create(float min, float max)
    {
        timerDuration = UnityEngine.Random.Range(min, max);
        timer = timerDuration;
    }
    public bool checkTimer()
    {
        timer -= Time.deltaTime;        
        // Check if the timer has reached zero
        if (timer <= 0f)
        {
            // Timer has reached zero, do something
            timer = timerDuration;
            return true;

            // Optionally reset the timer for repeat functionality
        }
        else
            return false;

    }
    public void resetTimer()
    {
        timer = timerDuration;
    }
}
