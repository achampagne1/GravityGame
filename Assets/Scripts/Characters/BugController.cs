using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugController : CharacterController
{
    [SerializeField] float persistanceAfterDeath = 5f;

    // Start is called before the first frame update
    void Start()
    {
        calculateCharacterStart();
    }

    // Update is called once per frame
    void Update()
    {
        setMovement(1);
        setOrientation(-1);
        calculateCharacterUpdate();
    }

    protected override IEnumerator die()
    {
        setMovement(0);
        yield return base.die();
        yield return new WaitForSeconds(persistanceAfterDeath);
        Destroy(gameObject);
    }
}
