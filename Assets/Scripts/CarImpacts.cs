
using UnityEngine;

public class CarImpacts : MonoBehaviour
{

    [SerializeField] private float collisionCooldown = 2f;
    [SerializeField] private bool readyForCollision = true;
    // [SerializeField] GameObject player;
    private void Start()
    {
        readyForCollision = true;
    }

    private void Update()
    {
        if (collisionCooldown > 0f)
        {
            collisionCooldown -= Time.deltaTime;
        }
        else
        {
            readyForCollision = true;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Opponent"))
        {
            FindObjectOfType<AudioManager>().Play("Impact");
            readyForCollision = false;
        }
    }
}
