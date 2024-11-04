using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
    public Rigidbody2D rb;
    private void Start()
    {
        rb.velocity = new Vector2(Random.Range(1, 10), Random.Range(1, 10));
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Wall"))
    //    {
    //        rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
    //    }
    //    else if (collision.gameObject.CompareTag("Ground")) 
    //    {
    //        rb.velocity = new Vector2(rb.velocity.x , rb.velocity.y * -1);
    //    }
    //}
}
