using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    private Rigidbody playerRB;
    public Vector3 Offset;
    public float speed;

    public void Init(Transform player)
    {
        this.player = player;
        playerRB = player.GetComponent<Rigidbody>();
        transform.position = player.position + player.TransformVector(Offset);
    }

    void FixedUpdate()
    {
        if (playerRB != null)
        {
            Vector3 playerForward = (playerRB.velocity + player.forward).normalized;
            transform.position = Vector3.Lerp(transform.position,
                player.position + player.TransformVector(Offset)
                    + playerForward * (-5f),
                speed * Time.deltaTime);
            transform.LookAt(player);
        }
    }
}
