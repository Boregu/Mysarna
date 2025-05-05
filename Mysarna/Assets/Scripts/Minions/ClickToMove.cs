using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour {
    public GameObject moveIndicatorPrefab; // Assign a prefab with MoveIndicator, or instantiate via script

    void Update() {
        if (Input.GetMouseButtonDown(1)) { // Right mouse button
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                // Find the SelectSystem in the scene
                SelectSystem selectSystem = FindObjectOfType<SelectSystem>();
                if (selectSystem != null) {
                    foreach (GameObject obj in selectSystem.GetSelectedObjects()) {
                        NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
                        if (agent != null) {
                            agent.SetDestination(hit.point);
                            // Add move indicator
                            GameObject indicatorObj = new GameObject("MoveIndicator");
                            var indicator = indicatorObj.AddComponent<MoveIndicator>();
                            indicator.unit = obj.transform;
                            indicator.destination = hit.point;
                        }
                    }
                }
            }
        }
    }
}
