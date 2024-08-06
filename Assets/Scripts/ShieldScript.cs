using System.Collections;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player car detected shield");
            ModifiedCarController carController = other.attachedRigidbody.gameObject.GetComponent<ModifiedCarController>();

            if (carController != null)
            {
                carController.DeactivateShield();
                carController.ActivateShield();
                HideAndShowAfterDelay(5f);
            }
            else
            {
                Debug.LogError("ModifiedCarController script not found on the player car.");
            }
        }
    }

    private void HideAndShowAfterDelay(float delay)
    {
        gameObject.SetActive(false);
        Invoke(nameof(ShowShield), delay);
    }

    private void ShowShield()
    {
        gameObject.SetActive(true);
    }

}
