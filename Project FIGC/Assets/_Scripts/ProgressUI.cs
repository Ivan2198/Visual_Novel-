using UnityEngine;
using UnityEngine.UI;

public class ProgressUI : MonoBehaviour
{
    [SerializeField] private Image cooldownTimerUI;
    [SerializeField] private TreeStateMachine treeStateMachine;

    private void Update()
    {
        cooldownTimerUI.fillAmount = treeStateMachine.GetCooldownTimer();
    }


}
