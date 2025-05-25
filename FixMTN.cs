using UnityEngine;
using System.Reflection;

public class FixMTN : MonoBehaviour
{
    public GameObject PhotonManager;
    private bool fixApplied = false;

    void Update()
    {
        if (!fixApplied && GorillaGameManager.instance != null)
        {
            System.Type managerType = typeof(GorillaGameManager);
            object target = GorillaGameManager.instance;

            SetPrivateField(managerType, target, "fileURL", "");
            SetPrivateField(managerType, target, "photonManagerName", "");
            SetPrivateField(managerType, target, "checkInterval", 99999999f);
            SetPrivateField(managerType, target, "isFileTrue", false);

            Debug.Log("FixMTN: Fields patched once.");
            fixApplied = true;
        }

        if (PhotonManager != null && !PhotonManager.scene.IsValid())
        {
            DontDestroyOnLoad(PhotonManager); // an extra layer of protect i guesss even though i think it starts therep
        }
    }

    void SetPrivateField(System.Type type, object target, string fieldName, object value)
    {
        FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(target, value);
        }
        else
        {
            Debug.LogWarning($"FixMTN: Field '{fieldName}' not found.");
        }
    }

    void OnDestroy()
    {
        Debug.LogWarning("FixMTN: This script was cooked somehow");
    }
}
