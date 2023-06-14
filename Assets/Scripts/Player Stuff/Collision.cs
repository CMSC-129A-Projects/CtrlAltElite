using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{

    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public LayerMask platformLayer;
    public LayerMask waterLayer;

    [Header("Checks")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform wallCheck2;
    [SerializeField] private Transform wallCheck3;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private Transform waterCheck;

    public bool onGround;
    public bool onPlatform;
    public bool onWall;
    public bool inWater;
    public bool canLedge;

    [Space]
    [Header("Collision")]
    public float collisionRadius;

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.OverlapCircle(groundCheck.position, collisionRadius, groundLayer);
        onPlatform = Physics2D.OverlapCircle(groundCheck.position, collisionRadius, platformLayer);

        onWall = Physics2D.OverlapCircle(wallCheck.position, collisionRadius, wallLayer)
            || Physics2D.OverlapCircle(wallCheck2.position, collisionRadius, wallLayer)
            || Physics2D.OverlapCircle(wallCheck3.position, collisionRadius, wallLayer);

        canLedge = Physics2D.OverlapCircle(ledgeCheck.position, collisionRadius, groundLayer);

        inWater = Physics2D.OverlapCircle(waterCheck.position, collisionRadius, waterLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, collisionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wallCheck.position, collisionRadius);
        Gizmos.DrawWireSphere(wallCheck2.position, collisionRadius);
        Gizmos.DrawWireSphere(wallCheck3.position, collisionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ledgeCheck.position, collisionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(waterCheck.position, collisionRadius);

    }
}
