using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ElectricFenceController : HazardController
{
    [SerializeField] private List<Sprite> frames;
    [SerializeField] private float stunTime = 1f;
    [SerializeField] private float knockBackForce = 30f;
    private SpriteRenderer spriteRenderer;
    private Coroutine newFrame;
    private Coroutine forceLocalAddedReset;
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
            yield return new WaitForSeconds(.025f);
        }
    }

    protected override IEnumerator damageOverTime()
    {
        characterController.setForceLocalAdded(true);
        Vector2 fenceNormal = transform.up;
        Vector2 hitObjectOffset = characterController.transform.position - transform.position;
        Vector2 forceDirection = Vector2.Dot(hitObjectOffset, fenceNormal) >= 0f ? fenceNormal : -fenceNormal;
        characterController.getRigidbody().AddForce(forceDirection * knockBackForce, ForceMode2D.Impulse);
        if (forceLocalAddedReset != null)
        {
            StopCoroutine(forceLocalAddedReset);
        }
        forceLocalAddedReset = StartCoroutine(resetForceLocalAdded());
        return base.damageOverTime();
    }

    private IEnumerator resetForceLocalAdded()
    {
        yield return new WaitForSeconds(stunTime);
        characterController.setForceLocalAdded(false);
        forceLocalAddedReset = null;
    }
}
