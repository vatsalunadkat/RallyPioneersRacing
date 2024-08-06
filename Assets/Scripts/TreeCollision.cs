using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCollision : MonoBehaviour
{
    [SerializeField] public GameObject treePrefab;
    private Rigidbody treeRB;
    [SerializeField] private bool isUprooted = false;
    // Start is called before the first frame update
    void Start()
    {
        treeRB = GetComponent<Rigidbody>();
        isUprooted = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Opponent")) && !isUprooted)
        {
            isUprooted = true;
            treeRB.isKinematic = false;

            if (treePrefab != null)
            {
                GameObject uprootedTree = Instantiate(treePrefab, transform.position, transform.rotation);
                uprootedTree.transform.localScale = transform.localScale;
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
