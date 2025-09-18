using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEditor.FilePathAttribute;

public class GunController : ItemController
{
    //public variables
    [SerializeField] float bulletForce = 50.0f;

    //vectors
    private Vector3 shootDirection = Vector3.zero;

    //object creation
    private GameObject bulletObject;
    private Timer throwTimer;
    private Timer throwTimer2;
    private Animator animator;
    [SerializeField] AudioClip gunshotClip;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject muzzleFlash;

    public override void Start()
    {
        Physics2D.IgnoreLayerCollision(9, 13, true);
        Physics2D.IgnoreLayerCollision(11, 13, true);
        Physics2D.IgnoreLayerCollision(13, 13, true);
        throwTimer = new Timer(.25f); //this is to make sure the player doesnt immidietly grab the item when it is thrown

        base.Start();

        try
        {
            animator = GetComponent<Animator>();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void useItem()
    {
        setShootDirection(new Vector3(1f,0f,0f)); //swap this to all items
        shootWrapper(); //currently jsut guns
    }

    private void shootWrapper()
    {
        Vector3 offset = new Vector3(.5f, .25f, 0);
        offset.y = offset.y * (facingLeft ? -1 : 1);
        GameObject bulletClone= Instantiate(bullet, transform.position + transform.rotation * offset, transform.rotation);
        GameObject muzzleFlashClone = Instantiate(muzzleFlash,transform);
        muzzleFlashClone.transform.parent = transform;
        muzzleFlashClone.transform.localPosition = new Vector3(3.04f, 1.05f, 0f);
        Destroy(muzzleFlashClone, .05f);
        animator.SetTrigger("Shoot");
        bulletClone.GetComponent<BulletController>().init(transform.parent.gameObject.layer);
        bulletClone.GetComponent<Rigidbody2D>().AddForce(shootDirection * bulletForce, ForceMode2D.Impulse);
        SoundManager.instance.playSound(gunshotClip, transform, 1f);

    }

    private void setShootDirection(Vector3 input)
    {
        shootDirection = input;
    }


}