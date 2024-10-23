using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    ArmourScript[] armourScripts;
    tankMain TankMain;
    TankMovement tankMovement;
    gunController GunController;

    [SerializeField] private Text healthUI;

    // Start is called before the first frame update
    void Start()
    {
        armourScripts = GetComponentsInChildren<ArmourScript>();
        TankMain = GetComponent<tankMain>();
        tankMovement = GetComponent<TankMovement>();
        GunController = GetComponentInChildren<gunController>();
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

        //adding more text for the performance of the certain parts of the vehicle
        updateText = updateText + "\nEngine power: " + tankMovement.getEnginePerformance() * 100 + "%\n";
        updateText = updateText + "Track speed: " + tankMovement.getSpeedPerformance() * 100 + "%\n";
        updateText = updateText + "Turret speed" + GunController.getTurretSpeedEfficiency() * 100 + "%";

        healthUI.text = updateText;
    }
}
