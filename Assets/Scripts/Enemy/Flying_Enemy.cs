using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flying_Enemy : MonoBehaviour
{
    GameObject player;
    [SerializeField] Rigidbody2D body;
    public GameObject shootPoint;
    public GameObject bullet;

    [Header("Attack Parameters")]
    public float lineOfSite;
    public float shootingRange;
    bool canAttack;
    bool isAttacking;
    public float duration = 1f;
    private float cooldown;

    [Header("Movement Parameters")]
    public float patrolDistance = 5f; // Distancia de patrullaje desde la posición inicial
    public float speed;
    private Vector2 startPoint;      // Posición inicial
    private Vector2 patrolPointRight; // Punto derecho de patrulla
    private Vector2 patrolPointLeft;  // Punto izquierdo de patrulla
    private Vector2 currentPatrolPoint; // Punto actual de patrulla

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        canAttack = true;

        // Guardamos la posición inicial del enemigo
        startPoint = transform.position;

        // Calculamos los puntos de patrullaje a partir de la posición inicial y la distancia
        patrolPointRight = startPoint + Vector2.right * patrolDistance;
        patrolPointLeft = startPoint - Vector2.right * patrolDistance;

        // Iniciamos el patrullaje hacia el primer punto (derecha)
        currentPatrolPoint = patrolPointRight;
    }

    private void Update()
    {
        float dist = Vector2.Distance(player.transform.position, this.transform.position);

        if (dist > lineOfSite || dist > shootingRange)
        {
            Patrol();
        }
        else if (dist <= shootingRange && cooldown < Time.time)
        {
            Instantiate(bullet, shootPoint.transform.position, Quaternion.identity);
            cooldown = Time.time + duration;
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

    void Flip()
    {
        // Determinar la dirección en función de la posición del enemigo y del objetivo
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon"))
        {
            // Función de quitar vida aquí
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lineOfSite);
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
