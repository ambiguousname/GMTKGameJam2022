using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerBullet")
        {
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerBullet")
        {
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider, false);
        }
    }
}
