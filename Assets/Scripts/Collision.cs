using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{

    [Header("Layers")]
    public LayerMask groundLayer;

    [Header("Checks")]
    [SerializeField] public Transform groundCheck;
    [SerializeField] public Transform wallCheck;
    [SerializeField] public Transform ledgeCheck;

    [Space]
    public PlayerMovement player;

    public bool onGround;
    public bool onWall;
    public bool canCornerCorrect;
    public bool canLedge;
    public int wallSide;

    [Space]

    [Header("Collision")]
    public float collisionRadius = 0.25f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);

        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        //onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);

        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        canLedge = Physics2D.OverlapCircle((Vector2)transform.position + ledgeOffsetRight, collisionRadius, groundLayer) ||
            Physics2D.OverlapCircle((Vector2)transform.position + ledgeOffsetLeft, collisionRadius, groundLayer);

        wallSide = onRightWall ? -1 : 1;*/

        onGround = Physics2D.OverlapCircle(groundCheck.position, collisionRadius, groundLayer);
        onWall = Physics2D.OverlapCircle(wallCheck.position, collisionRadius, groundLayer);
        canLedge = Physics2D.OverlapCircle(ledgeCheck.position, collisionRadius, groundLayer);


        canCornerCorrect = Physics2D.Raycast(transform.position + player.data.edgeRayCastOffset, Vector2.up, player.data.topRayCastLength, groundLayer)
            && !Physics2D.Raycast(transform.position + player.data.innerRayCastOffset, Vector2.up, player.data.topRayCastLength, groundLayer)
            || Physics2D.Raycast(transform.position - player.data.edgeRayCastOffset, Vector2.up, player.data.topRayCastLength, groundLayer)
            && !Physics2D.Raycast(transform.position - player.data.innerRayCastOffset, Vector2.up, player.data.topRayCastLength, groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, collisionRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(wallCheck.position, collisionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(ledgeCheck.position, collisionRadius);

        Gizmos.color = Color.blue;
        // Corner Check
        Gizmos.DrawLine(transform.position + player.data.edgeRayCastOffset, transform.position + player.data.edgeRayCastOffset + Vector3.up * player.data.topRayCastLength);
        Gizmos.DrawLine(transform.position - player.data.edgeRayCastOffset, transform.position - player.data.edgeRayCastOffset + Vector3.up * player.data.topRayCastLength);
        Gizmos.DrawLine(transform.position + player.data.innerRayCastOffset, transform.position + player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength);
        Gizmos.DrawLine(transform.position - player.data.innerRayCastOffset, transform.position - player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength);

        // Corner Distance Check
        Gizmos.DrawLine(transform.position - player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength,
                        transform.position - player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength + Vector3.left * player.data.topRayCastLength);
        Gizmos.DrawLine(transform.position + player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength,
                        transform.position + player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength + Vector3.right * player.data.topRayCastLength);

    }
}
