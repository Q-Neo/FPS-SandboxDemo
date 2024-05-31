

using UnityEngine;
using UnityEngine.Networking;

public class CharacterAnimation : NetworkBehaviour
{
    private Animator animator;
    public HumanBodyBones UpperChest = HumanBodyBones.UpperChest;
    public Transform headCamera;
    [HideInInspector]
    public Quaternion UpperBodyRotation;
    public Vector3 Offset;
    public float Speed = -1;
    private CharacterSystem character;
    [SyncVar]
    float cameraRotationSync = 0;
    private float cameraRotationSyncTmp = 0;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        character = this.GetComponent<CharacterSystem>();
        if (headCamera == null)
        {
            FPSCamera fpscam = this.GetComponentInChildren<FPSCamera>();
            headCamera = fpscam.gameObject.transform;
        }
    }

    [Command(channel = 1)]
    void CmdCameraUpdate(float camRotaion)
    {
        cameraRotationSync = camRotaion;
    }

    private float timeLastTrip;
    private float latencyTime;
    void onCameraSyn()
    {
        if (cameraRotationSync != cameraRotationSyncTmp)
        {
            latencyTime = Time.time - timeLastTrip;
            timeLastTrip = Time.time;
        }
    }

    float timeTmpsending;
    void Update()
    {
        if (animator == null || character == null)
            return;

        if (isLocalPlayer)
        {
            // update upper body rotation to host
            UpperBodyRotation = Quaternion.identity;
            float headCameraRotation = headCamera.transform.localRotation.eulerAngles.x;
            if (character.MovementPreset.Length > character.MovementIndex)
                headCameraRotation += character.MovementPreset[character.MovementIndex].UpperChestOffset;

            UpperBodyRotation.eulerAngles = new Vector3(UpperBodyRotation.eulerAngles.x + Offset.x, UpperBodyRotation.eulerAngles.y + Offset.y, Speed * headCameraRotation + Offset.z);

            float fps = (1 / Time.deltaTime);
            float delay = (fps / character.currentSendingRate) * Time.deltaTime;
            if (Time.time > timeTmpsending + delay)
            {
                CmdCameraUpdate(headCameraRotation);
                timeTmpsending = Time.time;
            }
        }
        else
        {
            // rotate upper body by data from server
            float lerpValue = (Time.time - timeLastTrip) / latencyTime;
            Quaternion rotationTarget = UpperBodyRotation;
            rotationTarget.eulerAngles = new Vector3(rotationTarget.eulerAngles.x + Offset.x, rotationTarget.eulerAngles.y + Offset.y, Speed * cameraRotationSync + Offset.z);
            UpperBodyRotation = Quaternion.Lerp(UpperBodyRotation, rotationTarget, 30 * Time.deltaTime);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetBoneLocalRotation(UpperChest, UpperBodyRotation);
    }
}
