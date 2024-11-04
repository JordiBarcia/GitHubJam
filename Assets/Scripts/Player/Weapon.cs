using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Weapon : MonoBehaviour
{
    public Transform player; // Asigna el transform del jugador en el inspector
    public float distanceFromPlayer = 1f; // Distancia a la que el arma se mantendrá del jugador

    bool canParry = true;
    public bool isParrying;
    float parryTime = 0.5f;
    float parryCooldown = 0.7f;

    public GameObject parryColor;

    void Update()
    {
        Positioning();
        if (Input.GetKey(KeyCode.Mouse0) && canParry) 
        {
            StartCoroutine(Parry());
        }
    }

    void Positioning()
    {
        // Obtenemos la posición del mouse en el mundo
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Ajustamos la posición en Z si el juego es 2D

        // Calculamos la dirección del jugador al mouse
        Vector3 direction = (mousePosition - player.position).normalized;

        // Posicionamos el arma a la distancia deseada desde el jugador en la dirección del mouse
        transform.position = player.position + direction * distanceFromPlayer;

        // Rotamos el arma para que apunte hacia el mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }


    private IEnumerator Parry()
    {
        canParry = false;
        isParrying = true;
        parryColor.SetActive(true);
        yield return new WaitForSeconds(parryTime);
        isParrying = false;
        parryColor.SetActive(false);
        yield return new WaitForSeconds(parryCooldown);
        canParry = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isParrying && collision.CompareTag("Enemy"))
        {
            Debug.Log("PUM");
        }
    }
}
