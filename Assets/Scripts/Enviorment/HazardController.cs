using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardController : MonoBehaviour, IDamager
{
    [SerializeField] float damageVariable = 100f;
    private Collider2D collider;
    protected CharacterController characterController;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        collider = GetComponent<Collider2D>();
        if (collider == null)
            Debug.LogError("no attached collider for " + gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool damage(GameObject damagedObject)
    {
        characterController = damagedObject.GetComponent<CharacterController>();
        StartCoroutine(damageOverTime());
        return true;
    }

    protected virtual IEnumerator damageOverTime()
    {
        if (!collider.isTrigger)
            yield return new WaitUntil(() => collider.IsTouching(characterController.getCollider()));
        characterController.setHealth(characterController.getHealth()-damageVariable);
        yield return new WaitForSeconds(1f);
        if(collider.IsTouching(characterController.getCollider()))
            characterController.hit(gameObject);
        yield return null;
    }

    public float getDamage()
    {
        return damageVariable;
    }
}
