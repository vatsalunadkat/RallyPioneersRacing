using System.Collections;
using UnityEngine;

public class SpeedPowerUp : MonoBehaviour
{
    [Header("Duration")]
    [Range(1, 10)]
    public float resetTime = 2f; // Default to 2 seconds, can be set from the editor

    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        //Comment Debug.Log("OnTriggerEnter called with: " + other.name);
        if (other.CompareTag("Player"))
        {
            //Comment Debug.Log("Player car detected");
            ModifiedCarController carController = other.attachedRigidbody.gameObject.GetComponent<ModifiedCarController>();
            if (carController != null)
            {
                //Comment Debug.Log("Starting speed increase coroutine on ModifiedCarController");
                carController.StartCoroutine(carController.IncreaseSpeed(resetTime));
            }
            else
            {
                //Comment Debug.LogError("ModifiedCarController script not found on the player car.");
            }
            HideAndShowAfterDelay(5f);
        }
    }

    private void HideAndShowAfterDelay(float delay)
    {
        // Hide the SpeedPowerUp object after delay seconds
        gameObject.SetActive(false);

        // Show the SpeedPowerUp object after delay seconds
        Invoke(nameof(ShowSpeedPowerUp), delay);
    }

    private void ShowSpeedPowerUp()
    {
        gameObject.SetActive(true);
    }

}
