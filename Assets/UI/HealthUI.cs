using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    ArmourScript[] armourScripts;
    [SerializeField] tankMain TankMain;

    [SerializeField] private Text healthUI;

    // Start is called before the first frame update
    void Start()
    {
        armourScripts = GetComponentsInChildren<ArmourScript>();
    }

    // Update is called once per frame
    void Update()
    {
        updateGUI();
    }

    private void updateGUI()
    {
        //this will deal with processing the health values
        string updateText = "Health Values\nTotalHealth: " + TankMain.getHealthPercentage() * 100f + "%\n";

        for (int i = 0; i < armourScripts.Length; i++)
        {
            updateText = updateText + armourScripts[i].getName() + ": " + armourScripts[i].getHealthPercent() * 100f + "%\n";
        }



        healthUI.text = updateText;
    }
}
