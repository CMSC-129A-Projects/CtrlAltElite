using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] private float bounce;
    private GameObject player;
    private Rigidbody2D rb;



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
            //rb = collision.gameObject.GetComponent<Rigidbody2D>();
            //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * bounce);
            //collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(collision.gameObject.GetComponent<Rigidbody2D>().velocity.x, collision.gameObject.GetComponent<Rigidbody2D>().velocity.y * bounce);
            /* player = collision.gameObject;
            rb = player.GetComponent<Rigidbody2D>();
            if (rb.velocity.y > 0.1f)
            {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Abs(rb.velocity.y) + bounce/2);
            } 
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Abs(rb.velocity.y) + bounce);
            }
            
            Debug.Log(rb.velocity);*/
        }
    }
}
