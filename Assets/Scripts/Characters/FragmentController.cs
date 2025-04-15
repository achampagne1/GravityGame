using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentController : ObjectController
{
    // Start is called before the first frame update
    [SerializeField] bool explode = false;
    [SerializeField] GameObject center;
    [SerializeField] float explosionStrength = 1f;
    [SerializeField] float fadeTime = 1f;
    private float fadeCounter = 360f;
    private Timer fadeClock;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool fadeLatch = false;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        fadeClock = new Timer(fadeTime);
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(11, 11, true);
        rb.simulated = false;
        calculateStart();
        if (center == null)
            Debug.LogError("no center point found");
    }

    // Update is called once per frame
    void Update()
    {
       if (explode)
        {
            Vector2 fromCenter = (Vector2)(transform.position - center.transform.position);
            Vector2 explodeDirection = fromCenter.normalized * explosionStrength;
            rb.AddForce(explodeDirection, ForceMode2D.Impulse);
            gravityAffected = true;
            rb.simulated = true;
            explode = false;
            fadeClock.startTimer();
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }

        if (fadeClock.checkTimer())
            fadeLatch = true;

        if (fadeLatch)
            fade();

        calculateUpdate();
    }

    private void fade() //fading may be tweaked in the future
        {
        Color c = sr.color;
        c.a = Mathf.Sin(fadeCounter);
        sr.color = c;
        fadeCounter -= .05f;
        if (fadeCounter <= 0)
            fadeCounter = 360;
        }
    public void setExplode(bool explode)
    {
        this.explode = explode;
    }
}
