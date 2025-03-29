using UnityEngine;

public struct Timer
{
    private float totalTime;   // Total time the timer will run
    private float currentTime; // Time left in the timer
    private bool isRunning;    // Flag to check if the timer is running

    // Constructor to initialize the total time
    public Timer(float totalTime)
    {
        this.totalTime = totalTime;
        currentTime = 0f;
        isRunning = false;
    }

    // Start the timer
    public void startTimer()
    {
        isRunning = true;
        currentTime = totalTime; // Reset the current time to the total time
    }

    // Check if the timer has ended
    public bool checkTimer()
    {
        if (isRunning)
        {
            currentTime -= Time.deltaTime; // Decrease the timer based on time passed
            if (currentTime <= 0)
            {
                isRunning = false;
                currentTime = 0f;
                return true; // Timer has ended
            }
        }
        return false; // Timer is still running
    }

    // Reset the timer to its initial state
    public void resetTimer()
    {
        isRunning = false;
        currentTime = totalTime;
    }

    // Optional: You can add a function to check how much time is left
    public float getTimeRemaining()
    {
        return currentTime;
    }

    public bool getIsRunning()
    {
        return isRunning;
    }
}