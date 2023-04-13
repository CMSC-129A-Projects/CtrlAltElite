using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class BreakableTile : MonoBehaviour
{
    public Tilemap tilemap;
    [SerializeField] private float replaceDelay;
    [SerializeField] private float replaceDelayWithPlayer;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the collision point in local coordinates
            Vector3 hitPosition = transform.InverseTransformPoint(collision.transform.position);
            // Get the tile at the collision point
            Vector3Int cellPosition = tilemap.WorldToCell(collision.transform.position);

            // check where the player made contact of the tile
            foreach (ContactPoint2D hitPos in collision.contacts)
            {
                // do some math to adjust the coordinates of the cell
                // this is because when the player first collides with the tile,
                // the coordinates will be set to the player's position instead of the tile's cell in front/back/top/bottom of the player
                if (hitPos.normal.y < 0)
                {
                    cellPosition.y += (int)(Mathf.Floor(hitPos.normal.y));
                }
                if (hitPos.normal.y > 0)
                {
                    cellPosition.y += (int)(Mathf.Ceil(hitPos.normal.y));
                }
                if (hitPos.normal.x < 0)
                {
                    cellPosition.x += (int)(Mathf.Floor(hitPos.normal.x));
                }
                if (hitPos.normal.x > 0)
                {
                    cellPosition.x += (int)(Mathf.Ceil(hitPos.normal.x));
                }

                TileBase tile = tilemap.GetTile(cellPosition); // get the tile of the recently broken tile

                if (tile != null)
                {
                    tilemap.SetTileFlags(cellPosition, TileFlags.None); // set its flags to none so no interactions to the tile
                    tilemap.SetColor(cellPosition, Color.clear); // set its color to clear (dont need this but just to be safe)
                    tilemap.SetTile(cellPosition, null); // remove the tile entirely
                    StartCoroutine(ReplaceTile(cellPosition, tile, replaceDelay, collision.gameObject)); // start replacing the tile after replaceDelay seconds
                }
                break;
            }
        }
    }

    IEnumerator ReplaceTile(Vector3Int cellPosition, TileBase tile, float delay, GameObject player)
    {
        yield return new WaitForSeconds(delay);
        Vector3 hitPosition = transform.InverseTransformPoint(player.transform.position); // get the player's position

        if (cellPosition == tilemap.WorldToCell(player.transform.position)) // if there is a player in the cell where a tile is to be placed, run the coroutine again
        {
            Debug.Log("Cant replace tile because player is here");
            StartCoroutine(ReplaceTile(cellPosition, tile,replaceDelayWithPlayer, player));
        }
        else
        {
            tilemap.SetTile(cellPosition, tile); // replace tile with tile
        }     
    }

    // some test stuff

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // function to get the distance between the trigger collider and the other collider, and then compare the relative positions of the two colliders to determine the direction of the collision.
            ColliderDistance2D distance = collision.Distance(GetComponent<Collider2D>());
            // Get the collision point in local coordinates
            Vector3 hitPosition = transform.InverseTransformPoint(collision.transform.position);
            // Get the tile at the collision point
            Vector3Int cellPosition = tilemap.WorldToCell(collision.transform.position);

            Debug.Log(hitPosition);
            Debug.Log("Initial CellPos: " + cellPosition);

            // adjust the collision detection because the player's coordinates get calculated, not the tile's
            if (distance.isOverlapped)
            {
                Vector2 direction = distance.normal * -1;

                if (direction == Vector2.up)
                {
                    Debug.Log("Colliding from above " + direction);
                    cellPosition.y -= (int)direction.y;
                }
                else if (direction == Vector2.down)
                {
                    Debug.Log("Colliding from below " + direction);
                    cellPosition.y -= (int)direction.y;
                }
                else if (direction == Vector2.left)
                {
                    Debug.Log("Colliding from the left " + direction);
                    cellPosition.x -= (int)direction.x;
                }
                else if (direction == Vector2.right)
                {
                    Debug.Log("Colliding from the right " + direction);
                    cellPosition.x -= (int)direction.x;
                }
            }

            TileBase tile = tilemap.GetTile(cellPosition);

            if (tile != null)
            {
                tilemap.SetTileFlags(cellPosition, TileFlags.None);
                tilemap.SetColor(cellPosition, Color.clear);
                tilemap.SetTile(cellPosition, null);
                previousCellPosition = cellPosition;
            }



            Debug.Log("final CellPos: " + cellPosition);
            Debug.Log(tile);
        }
    }*/

    /*private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // function to get the distance between the trigger collider and the other collider, and then compare the relative positions of the two colliders to determine the direction of the collision.
            ColliderDistance2D distance = collision.Distance(GetComponent<Collider2D>());

            // Debug.Log(collision.gameObject.transform.localScale.x);
            Vector3 hitPosition = collision.ClosestPoint(tilemap.transform.position);
            Vector3Int cellPosition = tilemap.WorldToCell(hitPosition);
            
            // Debug.Log(hitPosition + " " + cellPosition);
            // adjust the coordinates based on where the player is facing
            if (collision.gameObject.transform.localScale.x < 0)
                cellPosition.x += (int)collision.gameObject.transform.localScale.x;

            if (distance.isOverlapped)
            {
                Vector2 direction = distance.normal * -1;

                if (direction == Vector2.up)
                {
                    Debug.Log("Colliding from above " + direction);
                    cellPosition.y -= (int)direction.y;
                }
                else if (direction == Vector2.down)
                {
                    Debug.Log("Colliding from below " + direction);
                    cellPosition.y -= (int)direction.y;
                }
            }

            TileBase tile = tilemap.GetTile(cellPosition);

            if (tile != null)
            {
                tilemap.SetTileFlags(cellPosition, TileFlags.None);
                tilemap.SetColor(cellPosition, Color.clear);
                tilemap.SetTile(cellPosition, null);
                previousCellPosition = cellPosition;
            }
        }
    }*/
}
