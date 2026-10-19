using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ElectricFenceController : HazardController
{
    [SerializeField] private List<Sprite> frames;
    private SpriteRenderer spriteRenderer;
    private Coroutine newFrame;
    private int currentFrameIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        newFrame = StartCoroutine(chooseNewFrame());
        base.Start();
    }

    private IEnumerator chooseNewFrame()
    {
        while (true)
        {
            int frameIndex = Random.Range(0, frames.Count);
            if(frameIndex == currentFrameIndex)
            {
                frameIndex = (frameIndex + 1) % frames.Count;
            }
            spriteRenderer.sprite = frames[frameIndex];
            currentFrameIndex = frameIndex;
            yield return new WaitForSeconds(.05f);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
