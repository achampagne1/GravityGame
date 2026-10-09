using UnityEngine;

public class VisorController : CharacterProp
{
    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
        base.Start();
    }

    public override void LateUpdate()
    {
        if (characterState == CHARACTERSTATE.AIMING)
            spriteRenderer.sprite = defaultSprite;
        base.LateUpdate();
    }
}
