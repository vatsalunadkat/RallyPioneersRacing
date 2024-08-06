using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private GameObject TutorialSystem;
    private TutorialScript TutorialScript;
    private GameObject Arrow;
    [SerializeField] public enum HintType { nos, oil, portal, time, checkpoint, shield, none };
    [SerializeField] public HintType currentHint = HintType.none;
    // Start is called before the first frame update
    void Start()
    {
        Arrow = transform.GetChild(0).gameObject;
        TutorialScript = TutorialSystem.GetComponent<TutorialScript>();
        Arrow.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ModifiedCarController>() != null)
        {
            Arrow.SetActive(true);
            switch (currentHint)
            {
                case HintType.nos:
                    TutorialScript.CurrentHelp = 0;
                    break;
                case HintType.oil:
                    TutorialScript.CurrentHelp = 1;
                    break;
                case HintType.portal:
                    TutorialScript.CurrentHelp = 2;
                    break;
                case HintType.time:
                    TutorialScript.CurrentHelp = 3;
                    break;
                case HintType.checkpoint:
                    TutorialScript.CurrentHelp = 4;
                    break;
                case HintType.shield:
                    TutorialScript.CurrentHelp = 5;
                    break;
                default:
                    TutorialScript.CurrentHelp = -1;
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<ModifiedCarController>() != null)
        {
            Arrow.SetActive(false);
            TutorialScript.CurrentHelp = -1;
            FindObjectOfType<AudioManager>().Play("Click");
        }
    }
}
