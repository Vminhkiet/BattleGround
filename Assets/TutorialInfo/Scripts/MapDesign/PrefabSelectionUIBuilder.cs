using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class PrefabSelectionUIBuilder
{
    public static void BuildSelectionUI(
        GameObject[] prefabs,
        Transform scrollViewContent,
        GameObject uiToggleButtonPrefab,
        List<Toggle> toggleList,
        List<GameObject> previewList,
        Action<int> onSelectCallback,
        Vector3 previewOffset,
        Vector3 previewRotation,
        float previewScale)
    {
        // Ensure a ToggleGroup exists
        ToggleGroup toggleGroup = scrollViewContent.GetComponent<ToggleGroup>();
        if (toggleGroup == null)
        {
            toggleGroup = scrollViewContent.gameObject.AddComponent<ToggleGroup>();
            toggleGroup.allowSwitchOff = false;
        }

        // Cleanup
        foreach (var preview in previewList)
            GameObject.Destroy(preview);
        previewList.Clear();

        foreach (var toggle in toggleList)
            GameObject.Destroy(toggle.gameObject);
        toggleList.Clear();

        // Build buttons
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i] == null)
            {
                Debug.LogWarning($"Prefab at index {i} is null. Skipping.");
                continue;
            }

            GameObject toggleButton = GameObject.Instantiate(uiToggleButtonPrefab, scrollViewContent);
            toggleButton.name = $"PrefabToggle_{prefabs[i].name}";

            Toggle toggle = toggleButton.GetComponent<Toggle>();
            if (toggle == null)
            {
                Debug.LogError($"Toggle button prefab '{uiToggleButtonPrefab.name}' missing Toggle component.");
                GameObject.Destroy(toggleButton);
                continue;
            }

            toggle.group = toggleGroup;
            toggleList.Add(toggle);

            int index = i;
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    onSelectCallback?.Invoke(index);
                }
            });

            // Create live preview
            GameObject preview = GameObject.Instantiate(prefabs[i]);
            previewList.Add(preview);

            preview.transform.SetParent(toggleButton.transform, false);
            preview.transform.localPosition = previewOffset;
            preview.transform.localRotation = Quaternion.Euler(previewRotation);
            preview.transform.localScale = Vector3.one * previewScale;

            RemovePhysics(preview);
            SetRaycastTargetable(preview, false);
        }
    }

    static void RemovePhysics(GameObject obj)
    {
        foreach (Collider c in obj.GetComponentsInChildren<Collider>())
            GameObject.Destroy(c);
        foreach (Rigidbody rb in obj.GetComponentsInChildren<Rigidbody>())
            GameObject.Destroy(rb);
    }

    static void SetRaycastTargetable(GameObject obj, bool targetable)
    {
        Graphic[] graphics = obj.GetComponentsInChildren<Graphic>();
        foreach (Graphic g in graphics)
            g.raycastTarget = targetable;
    }
}
