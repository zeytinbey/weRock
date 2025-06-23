using UnityEngine;

public class StepTrigger : MonoBehaviour
{
    public StepManager stepManager;
    public GameObject stepObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stepManager.OnStepPressed(stepObject);
        }
    }
}
