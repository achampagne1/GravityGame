using UnityEngine;
using System;

public struct Timer
{
    private float totalTime;         // Total duration of the timer in seconds
    private DateTime targetTime;     // When the timer should end
    private bool isRunning;          // Is the timer currently running

    // Constructor to initialize the timer
    public Timer(float totalTime)
    {
        this.totalTime = totalTime;
        this.targetTime = DateTime.MinValue;
        this.isRunning = false;
    }

    // Start the timer by setting the target end time
    public void startTimer()
    {
        targetTime = DateTime.Now.AddSeconds(totalTime);
        isRunning = true;
    }

    // Check if the timer has expired
    public bool checkTimer()
    {
        if (isRunning && DateTime.Now >= targetTime)
        {
            isRunning = false;
            return true;
        }
        return false;
    }

    // Reset the timer without starting it
    public void resetTimer()
    {
        isRunning = false;
        targetTime = DateTime.MinValue;
    }

    // Time left in seconds
    public float getTimeRemaining()
    {
        if (!isRunning) return 0f;

        TimeSpan remaining = targetTime - DateTime.Now;
        return Mathf.Max(0f, (float)remaining.TotalSeconds);
    }

    public bool getIsRunning()
    {
        return isRunning;
    }
}
