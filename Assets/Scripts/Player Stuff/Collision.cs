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

    [Space]
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerData data;

    public bool onGround;
    public bool onPlatform;
    public bool onWall;
    public bool inWater;
    public bool canCornerCorrect;
    public bool canLedge;


    [Space]

    [Header("Collision")]
    public float collisionRadius;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        onGround = Physics2D.OverlapCircle(groundCheck.position, collisionRadius, groundLayer);
        onPlatform = Physics2D.OverlapCircle(groundCheck.position, collisionRadius, platformLayer);

        onWall = Physics2D.OverlapCircle(wallCheck.position, collisionRadius, wallLayer)
            || Physics2D.OverlapCircle(wallCheck2.position, collisionRadius, wallLayer)
            || Physics2D.OverlapCircle(wallCheck3.position, collisionRadius, wallLayer);

        canLedge = Physics2D.OverlapCircle(ledgeCheck.position, collisionRadius, groundLayer);

        /*canCornerCorrect = Physics2D.Raycast(transform.position + player.data.edgeRayCastOffset, Vector2.up, player.data.topRayCastLength, groundLayer)
            && !Physics2D.Raycast(transform.position + player.data.innerRayCastOffset, Vector2.up, player.data.topRayCastLength, groundLayer)
            || Physics2D.Raycast(transform.position - player.data.edgeRayCastOffset, Vector2.up, player.data.topRayCastLength, groundLayer)
            && !Physics2D.Raycast(transform.position - player.data.innerRayCastOffset, Vector2.up, player.data.topRayCastLength, groundLayer);*/
        canCornerCorrect = Physics2D.Raycast(transform.position + data.edgeRayCastOffset, Vector2.up, data.topRayCastLength, groundLayer)
            && !Physics2D.Raycast(transform.position + data.innerRayCastOffset, Vector2.up, data.topRayCastLength, groundLayer)
            || Physics2D.Raycast(transform.position - data.edgeRayCastOffset, Vector2.up, data.topRayCastLength, groundLayer)
            && !Physics2D.Raycast(transform.position - data.innerRayCastOffset, Vector2.up, data.topRayCastLength, groundLayer);

        inWater = Physics2D.OverlapCircle(groundCheck.position, collisionRadius, waterLayer);
    }

    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, collisionRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(wallCheck.position, collisionRadius);
        Gizmos.DrawWireSphere(wallCheck2.position, collisionRadius);
        Gizmos.DrawWireSphere(wallCheck3.position, collisionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(ledgeCheck.position, collisionRadius);

        Gizmos.color = Color.blue;
        // Corner Check
        *//*Gizmos.DrawLine(transform.position + player.data.edgeRayCastOffset, transform.position + player.data.edgeRayCastOffset + Vector3.up * player.data.topRayCastLength);
        Gizmos.DrawLine(transform.position - player.data.edgeRayCastOffset, transform.position - player.data.edgeRayCastOffset + Vector3.up * player.data.topRayCastLength);
        Gizmos.DrawLine(transform.position + player.data.innerRayCastOffset, transform.position + player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength);
        Gizmos.DrawLine(transform.position - player.data.innerRayCastOffset, transform.position - player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength);

        // Corner Distance Check
        Gizmos.DrawLine(transform.position - player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength,
                        transform.position - player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength + Vector3.left * player.data.topRayCastLength);
        Gizmos.DrawLine(transform.position + player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength,
                        transform.position + player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength + Vector3.right * player.data.topRayCastLength);*//*

        Gizmos.DrawLine(transform.position + data.edgeRayCastOffset, transform.position + data.edgeRayCastOffset + Vector3.up * data.topRayCastLength);
        Gizmos.DrawLine(transform.position - data.edgeRayCastOffset, transform.position - data.edgeRayCastOffset + Vector3.up * data.topRayCastLength);
        Gizmos.DrawLine(transform.position + data.innerRayCastOffset, transform.position + data.innerRayCastOffset + Vector3.up * data.topRayCastLength);
        Gizmos.DrawLine(transform.position - data.innerRayCastOffset, transform.position - data.innerRayCastOffset + Vector3.up * data.topRayCastLength);

        // Corner Distance Check
        Gizmos.DrawLine(transform.position - data.innerRayCastOffset + Vector3.up * data.topRayCastLength,
                        transform.position - data.innerRayCastOffset + Vector3.up * data.topRayCastLength + Vector3.left * data.topRayCastLength);
        Gizmos.DrawLine(transform.position + data.innerRayCastOffset + Vector3.up * data.topRayCastLength,
                        transform.position + data.innerRayCastOffset + Vector3.up * data.topRayCastLength + Vector3.right * data.topRayCastLength);

        

    }*/ 
}
