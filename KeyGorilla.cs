using UnityEngine;
using UnityEngine.InputSystem;
using GorillaLocomotion;
using GorillaNetworking;
using System.Reflection;
// updated with button support
public class KeyGorilla : MonoBehaviour
{
    public Player GPlayer;
    public Transform CameraTransform;
    public float MoveSpeed = 5f;
    public float TurnSpeed = 2f;
    private Rigidbody PlayerRigidbody;
    private Vector3 RotationVector = Vector3.zero;
    private static float KeyboardDelay = 0f;
    public float SprintMultiplier = 2f;
    public float FOV = 60f;

    /*
    credits to 
    SPEEDY.DLL - for the player movemnt system
    JRVR - motification
    SNC - all i did is improve sprintin and add pc button support for high versions
    */

    void Start()
    {
        if (GPlayer == null) { Debug.LogError("GPlayer is null"); }
        if (CameraTransform == null) { CameraTransform = Camera.main.transform; }
        PlayerRigidbody = GPlayer?.GetComponent<Rigidbody>();
        if (PlayerRigidbody == null) { Debug.LogError("Rigidbody is null"); }
    }

    void Update()
    {
        if (CameraTransform == null || PlayerRigidbody == null) { return; }

        Vector3 moveDirection = Vector3.zero;

        if (Keyboard.current.wKey.isPressed) { moveDirection += CameraTransform.forward; }
        if (Keyboard.current.sKey.isPressed) { moveDirection -= CameraTransform.forward; }
        if (Keyboard.current.aKey.isPressed) { moveDirection -= CameraTransform.right; }
        if (Keyboard.current.dKey.isPressed) { moveDirection += CameraTransform.right; }

        moveDirection.y = 0;
        moveDirection.Normalize();

        if (Keyboard.current.leftShiftKey.isPressed) { MoveSpeed = 15f * SprintMultiplier; }
        else { MoveSpeed = 5f; }

        if (Keyboard.current.spaceKey.isPressed)
        {
            moveDirection -= CameraTransform.up;
            moveDirection.y = 2;
        }

        PlayerRigidbody.MovePosition(PlayerRigidbody.position + moveDirection * MoveSpeed * Time.deltaTime);

        if (Mouse.current.rightButton.isPressed)
        {
            float mouseX = Mouse.current.delta.x.ReadValue() * TurnSpeed;
            float mouseY = Mouse.current.delta.y.ReadValue() * TurnSpeed;

            RotationVector.x -= mouseY;
            RotationVector.y += mouseX;
            RotationVector.x = Mathf.Clamp(RotationVector.x, -90f, 90f);

            CameraTransform.localRotation = Quaternion.Euler(RotationVector);
        }

        AdjustFOV();
        HandleBtnClk();
    }

    void AdjustFOV()
    {
        if (Keyboard.current.iKey.isPressed) { FOV = Mathf.Min(FOV + 1f, 130f); } // roblox type shittttttttttttt
        if (Keyboard.current.oKey.isPressed) { FOV = Mathf.Max(FOV - 1f, 60f); }

        Camera.main.fieldOfView = FOV;
    }

    void HandleBtnClk()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            if (Camera.main == null) { Debug.LogError("Camera.main is null!"); return; }

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 512f, LayerMask.GetMask("GorillaInteractable")))
            {
                GorillaPressableButton pressableButton = hit.collider.GetComponentInParent<GorillaPressableButton>();
                if (pressableButton)
                {
                    var collider = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHandTriggerCollider")?.GetComponent<Collider>();
                    if (collider != null)
                    {
                        typeof(GorillaPressableButton).GetMethod("OnTriggerEnter", BindingFlags.NonPublic | BindingFlags.Instance)
                            .Invoke(pressableButton, new object[] { collider });
                    }
                }

                GorillaKeyboardButton keyboardButton = hit.collider.GetComponentInParent<GorillaKeyboardButton>();
                if (keyboardButton && Time.time > KeyboardDelay)
                {
                    KeyboardDelay = Time.time + 0.1f;
                    var collider = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHandTriggerCollider")?.GetComponent<Collider>();
                    if (collider != null)
                    {
                        typeof(GorillaKeyboardButton).GetMethod("OnTriggerEnter", BindingFlags.NonPublic | BindingFlags.Instance)
                            .Invoke(keyboardButton, new object[] { collider });
                    }
                }

                GorillaPlayerLineButton playerLineButton = hit.collider.GetComponentInParent<GorillaPlayerLineButton>();
                if (playerLineButton && Time.time > KeyboardDelay)
                {
                    KeyboardDelay = Time.time + 0.1f;
                    var collider = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHandTriggerCollider")?.GetComponent<Collider>();
                    if (collider != null)
                    {
                        typeof(GorillaPlayerLineButton).GetMethod("OnTriggerEnter", BindingFlags.NonPublic | BindingFlags.Instance)
                            .Invoke(playerLineButton, new object[] { collider });
                    }
                }
            }
        }
    }
}
