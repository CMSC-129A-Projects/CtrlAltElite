using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreakablePlatform : MonoBehaviour
{
    public Tilemap tilemap;
    [SerializeField] private float breakDelay;
    [SerializeField] private float replaceDelay;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);
            TileBase clickedTile = tilemap.GetTile(cellPosition);

            if (clickedTile != null)
            {
                Debug.Log("Clicked tile position: " + cellPosition);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var PlayerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (PlayerMovement.isOnPlatform && !PlayerMovement.inAir) 
            {
                Debug.Log(true);
            }
        }
        /*if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<PlayerMovement>().isOnPlatform)
        {
            // Get the collision point in local coordinates
            Vector3 hitPosition = transform.InverseTransformPoint(collision.transform.position);
            // Get the tile at the collision point
            Vector3Int cellPosition = tilemap.WorldToCell(collision.transform.position);

            //Debug.Log("hit pos: " + hitPosition + " cell pos: " + cellPosition);
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

               //Debug.Log("final cellpos: " + cellPosition + " tile: " + tile);
                if (tile != null)
                {
                    StartCoroutine(BreakTile(cellPosition, tile, breakDelay, collision.gameObject));
                }
                break;
            }
        }*/
    }

    IEnumerator BreakTile(Vector3Int cellPosition, TileBase tile, float delay, GameObject player)
    {
        Debug.Log("Breaking TIle at " + cellPosition);
        yield return new WaitForSeconds(delay);
        tilemap.SetTileFlags(cellPosition, TileFlags.None); // set its flags to none so no interactions to the tile
        tilemap.SetColor(cellPosition, Color.clear); // set its color to clear (dont need this but just to be safe)
        tilemap.SetTile(cellPosition, null); // remove the tile entirely

        StartCoroutine(ReplaceTile(cellPosition, tile, replaceDelay, player));
    }

    IEnumerator ReplaceTile(Vector3Int cellPosition, TileBase tile, float delay, GameObject player)
    {
        Debug.Log("replacing tile");
        yield return new WaitForSeconds(delay);
        tilemap.SetTile(cellPosition, tile);
    }
}
