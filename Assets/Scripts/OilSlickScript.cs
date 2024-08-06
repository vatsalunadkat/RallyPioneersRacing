using System.Collections;
using UnityEngine;

public class OilSlickScript : MonoBehaviour
{
    private bool hasRotated = false;
    private ModifiedCarController carController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasRotated)
        {
            // Debug.Log("Player car detected");

            // Get the ModifiedCarController script attached to the player car
            carController = other.attachedRigidbody.gameObject.GetComponent<ModifiedCarController>();

            if (carController != null)
            {
                if (carController.isShieldActive)
                {
                    // Debug.Log("Shield is active player, skipping oil slick rotation and deactivating shield.");


                    hasRotated = true;  // Ensure the car does not rotate again immediately
                    StartCoroutine(DeactivateShieldWithDelay(carController, 0.5f));
                }
                else
                {
                    StartCoroutine(PerformAndResetRotation(carController));
                }
            }
            // else
            // {
            //     // Debug.LogError("ModifiedCarController script not found on the player car.");
            // }
        }
        else if (other.CompareTag("Opponent") && !hasRotated)
        {
            // Debug.Log("Opponent car detected");
            OpponentStateMachine opponentController = other.attachedRigidbody.gameObject.GetComponent<OpponentStateMachine>();
            if (opponentController != null)
            {
                // Debug.Log("Starting oil slick coroutine on ModifiedCarController");
                //FindObjectOfType<AudioManager>().Play("Screech");
                StartCoroutine(opponentController.Perform360Rotation());
            }
            // else
            // {
            //     Debug.LogError("OpponentStateMachine script not found on the opponent car.");
            // }
        }
    }

    private IEnumerator DeactivateShieldWithDelay(ModifiedCarController carController, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (carController != null)
        {
            carController.DeactivateShield();
            // Debug.Log("Shield deactivated after delay");
        }
    }

    private IEnumerator PerformAndResetRotation(ModifiedCarController carController)
    {
        hasRotated = true;
        yield return carController.Perform360Rotation();
        hasRotated = false;
    }
}
