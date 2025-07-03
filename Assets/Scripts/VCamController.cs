using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Rendering;

public class VCamController : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public float initialZoom = 5f;
    [SerializeField] float duration = 1f;
    [SerializeField] float shakeMagnitude = .2f;
    [SerializeField] float shootMagnitude = 2f;
    [SerializeField] bool shake = false;
    private Vector2 direction = new Vector2(0,0);
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.zKey.isPressed)
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(vcam.m_Lens.OrthographicSize, 30f, Time.deltaTime*4f);
        else
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(vcam.m_Lens.OrthographicSize, initialZoom, Time.deltaTime*4f);

        if (shake)
        {
            StartCoroutine(shakeFunc());
            shake = false;
        }

        if(direction != Vector2.zero)
        {
            StartCoroutine(gunRecoil(direction));
            direction = Vector2.zero;
        }
    }

    private IEnumerator shakeFunc()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsedTime = 0f;
        while(elapsedTime < duration)
        {
            float xOffset = Random.Range(-.5f, .5f) * shakeMagnitude;
            float yOffset = Random.Range(-.5f, .5f) * shakeMagnitude;

            transform.localPosition = new Vector3(xOffset, yOffset+20, originalPos.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;
    }

    private IEnumerator gunRecoil(Vector2 direction)
    {
        direction = HelperFunctions.rotateVector(direction, -transform.eulerAngles.z); //this is to account for the rotation of the player
        Vector3 originalPos = transform.localPosition;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localPosition = new Vector3(transform.localPosition.x+(direction.x*shootMagnitude), transform.localPosition.y + (direction.y * shootMagnitude), originalPos.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localPosition = new Vector3(transform.localPosition.x + (+direction.x * shootMagnitude), transform.localPosition.y + (+direction.y * shootMagnitude), originalPos.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;
    }

    public void setShake(bool shake)
    {
        this.shake = shake;
    }

    public void setGunRecoil(Vector2 direction)
    {
        this.direction = direction;
    }

}
