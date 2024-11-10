using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Rendering;

public class Weapon : MonoBehaviour
{
    [SerializeField] Movement move;
    public GameObject weapon;
    public float duration;
    public float cooldown;
    bool canAttack;
    bool isAttacking;

    private void Start()
    {
        canAttack = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && move.grounded &&  canAttack) // Sera el mateix boto que el del parry pero nomes ho podra fer desde el terra. 
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack() 
    {
        weapon.SetActive(true);
        canAttack = false;
        isAttacking = true;
        Debug.Log("Attacking");
        yield return new WaitForSeconds(duration);
        weapon.SetActive(false);
        isAttacking = false;
        Debug.Log("Not Attacking");
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}
