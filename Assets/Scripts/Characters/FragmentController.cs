using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentController : ObjectController
{
    // Start is called before the first frame update
    [SerializeField] bool explode = false;
    [SerializeField] GameObject center;
    [SerializeField] float explosionStrength = 1f;
    Rigidbody2D rb;
    void Start()
    {
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
        }

        calculateUpdate();
    }

    public void setExplode(bool explode)
    {
        this.explode = explode;
    }
}
