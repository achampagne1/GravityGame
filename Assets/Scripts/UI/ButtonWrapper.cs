using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonWrapper
{
    private Button button1;
    private Button button2;
    private System.Action func;
    private AudioSource buttonClickAudioSource;
    private AudioSource buttonHoverAudioSource;
    private float originalTop;
    private float originalLeft;
    private float originalWidth;
    private float originalHeight;


    [SerializeField] float hoverSize = 1.1f;

    public ButtonWrapper(Button button1, Button button2, System.Action func, AudioSource buttonClickAudioSource, AudioSource buttonHoverAudioSource)
    {
        originalTop = button2.resolvedStyle.top;
        originalLeft = button2.resolvedStyle.left;
        originalWidth = button2.resolvedStyle.width;
        originalHeight = button2.resolvedStyle.height;

        this.buttonClickAudioSource = buttonClickAudioSource;
        this.buttonHoverAudioSource = buttonHoverAudioSource;
        setButtons(button1, button2);
        setClickEvent(func);
        registerCallBacks();
    }

    public void setButtons(Button button1, Button button2)
    {
        this.button1 = button1;
        this.button2 = button2;
    }

    public void setClickEvent(System.Action func)
    {
        this.func = func;
    }

    public void registerCallBacks()
    {
        button1.clicked += () =>
        {
            buttonClickAudioSource?.Play();
            func?.Invoke();
        };

        button1.RegisterCallback<PointerEnterEvent>(evt => { buttonHoverEnter(); });

        button1.RegisterCallback<PointerLeaveEvent>(evt => { buttonHoverExit(); });
    }

    private void buttonHoverEnter()
    {
        buttonHoverAudioSource?.Play();
        originalTop = button2.resolvedStyle.top;
        originalLeft = button2.resolvedStyle.left;
        originalWidth = button2.resolvedStyle.width;
        originalHeight = button2.resolvedStyle.height;

        float newHeight = originalHeight * hoverSize;
        float newWidth = originalWidth * hoverSize;
        float heightDelta = newHeight - originalHeight;
        float widthDelta = newWidth - originalWidth;

        button2.style.top = new Length(button2.resolvedStyle.top - (heightDelta / 2f), LengthUnit.Pixel);
        button2.style.left = new Length(button2.resolvedStyle.left - (widthDelta / 2f), LengthUnit.Pixel);
        button2.style.width = new Length(button2.resolvedStyle.width * hoverSize, LengthUnit.Pixel);
        button2.style.height = new Length(newHeight, LengthUnit.Pixel);
    }

    private void buttonHoverExit()
    {
        button2.style.top = new Length(originalTop, LengthUnit.Pixel);
        button2.style.left = new Length(originalLeft, LengthUnit.Pixel);
        button2.style.width = new Length(originalWidth, LengthUnit.Pixel);
        button2.style.height = new Length(originalHeight, LengthUnit.Pixel);
    }

}
