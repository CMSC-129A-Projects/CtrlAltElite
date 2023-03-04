using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{

    [Header("Layers")]
    public LayerMask groundLayer;
    

    [Space]
    public PlayerMovement player;

    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public bool canCornerCorrect;
    public int wallSide;

    [Space]

    [Header("Collision")]

    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    private Color debugCollisionColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {  
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer) 
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        wallSide = onRightWall ? -1 : 1;

        canCornerCorrect = Physics2D.Raycast(transform.position + player.data.edgeRayCastOffset, Vector2.up, player.data.topRayCastLength, groundLayer)
            && !Physics2D.Raycast(transform.position + player.data.innerRayCastOffset, Vector2.up, player.data.topRayCastLength, groundLayer)
            || Physics2D.Raycast(transform.position - player.data.edgeRayCastOffset, Vector2.up, player.data.topRayCastLength, groundLayer)
            && !Physics2D.Raycast(transform.position - player.data.innerRayCastOffset, Vector2.up, player.data.topRayCastLength, groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position  + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);

        Gizmos.color = Color.blue;
        /*// Corner Check
        Gizmos.DrawLine(transform.position + player.data.edgeRayCastOffset, transform.position + player.data.edgeRayCastOffset + Vector3.up * player.data.topRayCastLength);
        Gizmos.DrawLine(transform.position - player.data.edgeRayCastOffset, transform.position - player.data.edgeRayCastOffset + Vector3.up * player.data.topRayCastLength);
        Gizmos.DrawLine(transform.position + player.data.innerRayCastOffset, transform.position + player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength);
        Gizmos.DrawLine(transform.position - player.data.innerRayCastOffset, transform.position - player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength);

        // Corner Distance Check
        Gizmos.DrawLine(transform.position - player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength,
                        transform.position - player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength + Vector3.left * player.data.topRayCastLength);
        Gizmos.DrawLine(transform.position + player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength,
                        transform.position + player.data.innerRayCastOffset + Vector3.up * player.data.topRayCastLength + Vector3.right * player.data.topRayCastLength);*/

    }
}
