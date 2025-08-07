using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamager
{
    public float getDamage();
    public bool damage(GameObject gameObject);
}