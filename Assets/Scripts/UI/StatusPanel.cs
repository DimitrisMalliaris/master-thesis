using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusPanel : MonoBehaviour
{
    [SerializeField] Agent agent;
    [SerializeField] GoalController agentGoalController;
    [SerializeField] ActionController agentActionController;
    [SerializeField] TextMeshProUGUI agentName;
    [SerializeField] TextMeshProUGUI healingFactorText;
    [SerializeField] TextMeshProUGUI currentActionText;

    [SerializeField] GameObject agentDiedMessage;

    // Start is called before the first frame update
    void Start()
    {
        agentName.text = agent.gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        healingFactorText.text = agentGoalController.HealingFactor.ToString();

        if (agentActionController.CurrentAction == null)
            currentActionText.text = "No Action";
        else
            currentActionText.text = agentActionController.CurrentAction.name;

        if (agent == null)
        {
            ApplicationManager.Instance.OnAgentDied();
            agentDiedMessage.SetActive(true);
        }

    }
}
