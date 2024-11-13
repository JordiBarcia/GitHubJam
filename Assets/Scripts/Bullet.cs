using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    GameObject player;
    [SerializeField] Rigidbody2D body;
    [Header("Parameters")]
    public float speed;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Vector2 moveDir = (player.transform.position - transform.position).normalized * speed;
        body.velocity = new Vector2(moveDir.x, moveDir.y);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }
}
