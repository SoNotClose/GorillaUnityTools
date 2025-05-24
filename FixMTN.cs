using UnityEngine;
using System.Reflection;

public class FixMTN : MonoBehaviour
{
    // by @sonotclose /zenunity
    // if this doesnt work better tutorial in GorillaUnityTools
    private const bool Fix = true;
    public bool patched;
    public string output = "null";

    void Start()
    {
        output = "Awaiting Join Server";
        patched = false;
    }

    void Update()
    {
        if (Fix && GorillaGameManager.instance != null)
        {
            System.Type managerType = typeof(GorillaGameManager);
            object target = GorillaGameManager.instance;

            SetPrivateField(managerType, target, "fileURL", "");
            SetPrivateField(managerType, target, "photonManagerName", "");
            SetPrivateField(managerType, target, "checkInterval", 99999999f);
            SetPrivateField(managerType, target, "isFileTrue", false);

            Debug.Log("MTN patched successfully. by @sonotclose");
            output = "Patched ^_^";
            patched = true;
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
            Debug.LogWarning($"field '{fieldName}' not found.");
            output = "ur mtn isnt broken";
            patched = false;
        }
    }
}