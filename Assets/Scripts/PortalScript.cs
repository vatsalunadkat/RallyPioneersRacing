using UnityEngine;

public class PortalScript : MonoBehaviour
{
    [SerializeField] private Transform exitPortal;
    [SerializeField] private Transform exitPortalDirection;
    [SerializeField] private GameObject PortalBlue;
    [SerializeField] private GameObject PortalOrange;
    [SerializeField] private ParticleSystem portalEffects;

    private float portalCooldownTimer = 0.1f; // Set zero to disable.
    private float timer = 0.00f;
    private bool portalsActivated = true;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(exitPortal.position, exitPortalDirection.position);
        Gizmos.DrawSphere(exitPortal.position, 0.3f);
        Gizmos.DrawSphere(exitPortalDirection.position, 0.7f);
    }

    private void Start()
    {
        portalEffects.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void Update()
    {
        if (!portalsActivated)
        {
            timer += Time.deltaTime;
            if (timer > portalCooldownTimer)
            {
                portalsActivated = true;
                timer = 0f;
            }
        }
        else
        {
            PortalBlue.SetActive(true);
            PortalOrange.SetActive(true);
            GetComponent<SphereCollider>().enabled = true;
        }

        if (portalEffects != null && !portalEffects.isPlaying)
        {
            portalEffects.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null)// && other.GetComponent<ModifiedCarController>() != null)
        {
            other.transform.position = exitPortal.transform.position;
            Vector3 direction = (exitPortalDirection.position - exitPortal.position).normalized;
            other.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            float speed = other.GetComponent<Rigidbody>().velocity.magnitude;
            Vector3 exitVelocity = direction * speed * 1.5f;
            other.GetComponent<Rigidbody>().velocity = exitVelocity;
            GetComponent<SphereCollider>().enabled = false;
            PortalBlue.SetActive(false);
            PortalOrange.SetActive(false);
            portalsActivated = false;
            portalEffects.Play(true);
            // other.transform.rotation = exitPortal.transform.rotation;
        }
    }
}
