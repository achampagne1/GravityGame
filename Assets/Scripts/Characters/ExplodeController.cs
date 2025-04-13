using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeController : MonoBehaviour
{
    public void trigger()
    {
        foreach(Transform fragment in transform)
        {
            try
            {
                SpriteRenderer sr = fragment.gameObject.GetComponent<SpriteRenderer>();
                Color c = sr.color;
                c.a = 1.0f; // Opacity from 0 (transparent) to 1 (fully opaque)
                sr.color = c;
                fragment.gameObject.GetComponent<FragmentController>().setExplode(true);
            }
            catch
            {
                int ham = 1;
            }
        }
    }


}
