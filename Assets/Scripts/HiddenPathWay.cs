using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenPathWay : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            /*ContactPoint2D contact = collision.GetContact(0);
            Vector3 hitPosition = contact.point - new Vector2(contact.normal.x, contact.normal.y) * 0.01f;
            Vector3Int cellPosition = tilemap.WorldToCell(hitPosition);

            if (tilemap.HasTile(cellPosition))
            {
                tilemap.SetTile(cellPosition, null);
            }*/
            Debug.Log("player");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            /*ContactPoint2D contact = collision.GetContact(0);
            Vector3 hitPosition = contact.point - new Vector2(contact.normal.x, contact.normal.y) * 0.01f;
            Vector3Int cellPosition = tilemap.WorldToCell(hitPosition);

            if (!tilemap.HasTile(cellPosition))
            {
                tilemap.SetTile(cellPosition, tilemap.GetTile(cellPosition));
            }*/

            Debug.Log("player out");
        }
    }
}
