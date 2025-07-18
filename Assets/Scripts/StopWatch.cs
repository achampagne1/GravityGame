using UnityEngine;

public class StopWatch
{
    private float startTime;
    private float elapsedTime;
    private bool isRunning;

    public void start()
    {
        if (!isRunning)
        {
            startTime = Time.time;
            isRunning = true;
        }
    }

    public void stop()
    {
        if (isRunning)
        {
            elapsedTime += Time.time - startTime;
            isRunning = false;
        }
    }

    public void reset()
    {
        startTime = 0f;
        elapsedTime = 0f;
        isRunning = false;
    }

    public float getElapsedTime()
    {
        if (isRunning)
        {
            return elapsedTime + (Time.time - startTime);
        }
        return elapsedTime;
    }

    public bool getIsRunning()
    {
        return isRunning;
    }
}