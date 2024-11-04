using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;
    bool canParry = true;
    public bool isParrying;
    public float parryTime = 0.2f;
    public float parryCooldown = 0.3f;
    public float parryPower;


    void Update()
    {
        if (Input.GetKey(KeyCode.Z) && canParry)
        {
            StartCoroutine(ParryCoroutine());
        }
    }
    private IEnumerator ParryCoroutine()
    {
        canParry = false;
        isParrying = true;
        Debug.Log("Parring");
        yield return new WaitForSeconds(parryTime);
        isParrying = false;
        yield return new WaitForSeconds(parryCooldown);
        canParry = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isParrying && collision.CompareTag("Enemy"))
        {
            body.velocity = new Vector2(transform.localScale.x * parryPower, transform.localScale.y * parryPower);
        }
    }
}
