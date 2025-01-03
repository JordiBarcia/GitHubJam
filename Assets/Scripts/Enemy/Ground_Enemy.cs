using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class Ground_Enemy : MonoBehaviour
{
    GameObject player;
    [SerializeField] Rigidbody2D body;

    [Header("Attack Parameters")]
    public GameObject weapon;
    bool canAttack;
    bool isAttacking;
    public float duration;
    public float cooldown;

    [Header("Movement Parameters")]
    public float patrolDistance = 5f; // Distancia de patrullaje desde la posici�n inicial
    public float speed;
    private Vector2 startPoint;      // Posici�n inicial
    private Vector2 patrolPointRight; // Punto derecho de patrulla
    private Vector2 patrolPointLeft;  // Punto izquierdo de patrulla
    private Vector2 currentPatrolPoint; // Punto actual de patrulla

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        canAttack = true;

        // Guardamos la posici�n inicial del enemigo
        startPoint = transform.position;

        // Calculamos los puntos de patrullaje a partir de la posici�n inicial y la distancia
        patrolPointRight = startPoint + Vector2.right * patrolDistance;
        patrolPointLeft = startPoint - Vector2.right * patrolDistance;

        // Iniciamos el patrullaje hacia el primer punto (derecha)
        currentPatrolPoint = patrolPointRight;
    }

    private void Update()
    {
        float dist = Vector2.Distance(player.transform.position, this.transform.position);

        if (dist <= 2 && canAttack)
        {
            // Si el jugador est� dentro del rango de ataque, atacar
            StartCoroutine(Attack());
        }
        else if (dist > 10 && !isAttacking)
        {
            // Si el jugador est� lejos, patrullar
            Patrol();
        }
        else if (!isAttacking)
        {
            // Si el jugador est� en rango medio, moverse hacia �l
            MoveToPlayer();
        }
    }

    void Patrol()
    {
        // Moverse hacia el punto de patrulla actual
        transform.position = Vector2.MoveTowards(transform.position, currentPatrolPoint, speed * Time.deltaTime);

        // Cambiar de punto cuando llega a uno de ellos
        if (Vector2.Distance(transform.position, currentPatrolPoint) < 0.1f)
        {
            // Alternar entre los puntos derecho e izquierdo
            currentPatrolPoint = (currentPatrolPoint == patrolPointRight) ? patrolPointLeft : patrolPointRight;
            Flip();
        }
    }

    void MoveToPlayer()
    {
        Vector2 targetPosition = new Vector2(player.transform.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Girar hacia la direcci�n del jugador
        if (transform.position.x < player.transform.position.x)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (transform.position.x > player.transform.position.x)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    void Flip()
    {
        // Determinar la direcci�n en funci�n de la posici�n del enemigo y del objetivo
        if (transform.position.x < currentPatrolPoint.x)
        {
            // Mover a la derecha
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (transform.position.x > currentPatrolPoint.x)
        {
            // Mover a la izquierda
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }



    IEnumerator Attack()
    {
        yield return new WaitForSeconds(cooldown);
        
        weapon.SetActive(true);
        canAttack = false;
        isAttacking = true;
        Debug.Log("Attacking");

        yield return new WaitForSeconds(duration);

        weapon.SetActive(false);
        isAttacking = false;
        Debug.Log("Not Attacking");

        canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon"))
        {
            // Funci�n de quitar vida aqu�
            Destroy(gameObject);
        }
    }
}
