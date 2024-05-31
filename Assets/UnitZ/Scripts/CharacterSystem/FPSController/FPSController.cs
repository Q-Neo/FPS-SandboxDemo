

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(PlayerView))]
[RequireComponent(typeof(CharacterInventory))]
[AddComponentMenu("Character/FPS Input Controller")]

public class FPSController : MonoBehaviour
{
    [HideInInspector]
    public CharacterSystem character;
    [HideInInspector]
    public CharacterMotor motor;
    [HideInInspector]
    public Vector3 inputDirection;
    private Vector2 mouseDirection;
    [HideInInspector]
    public PlayerView PlayerView;
    [HideInInspector]
    public float sensitivityXMult = 1;
    [HideInInspector]
    public float sensitivityYMult = 1;
    public float sensitivityX = 15;
    public float sensitivityY = 15;
    public float DampingSpeed = 300f;
    public float minimumX = -360;
    public float maximumX = 360;
    public float minimumY = -60;
    public float maximumY = 60;
    public float delayMouse = 0.5f;

    private float rotationX = 0;
    private float rotationY = 0;
    private float rotationXtemp = 0;
    private float rotationYtemp = 0;
    private Quaternion originalRotation;
    private Vector2 kickPower;
    private float fovTemp = 40;
    private float fovTarget;
    private Vector3 aimPosition;
    [HideInInspector]
    public bool zooming = false;
    [HideInInspector]
    public CharacterDriver Driver;
    //CharacterSystem character：角色系统，用于管理角色的状态和行为。
    //CharacterMotor motor：角色运动控制器，负责处理角色的移动和跳跃等操作。
   // Vector3 inputDirection：玩家输入的移动方向。
   // Vector2 mouseDirection：玩家输入的鼠标移动方向。
   // PlayerView PlayerView：玩家视图控制器，管理玩家的视角。
   // float sensitivityXMult和sensitivityYMult：鼠标灵敏度的倍数，用于调整水平和垂直方向上的灵敏度。
   // float sensitivityX和sensitivityY：水平和垂直方向上的鼠标灵敏度。
    //float DampingSpeed：旋转速度的衰减速度。
    //float minimumX和maximumX：水平旋转的最小和最大角度。
   // float minimumY和maximumY：垂直旋转的最小和最大角度。
   // float delayMouse：鼠标延迟。
   // 其他变量用于存储旋转角度、鼠标输入等信息。
   
    void Start()
    {
        character = gameObject.GetComponent<CharacterSystem>();
        Driver = gameObject.GetComponent<CharacterDriver>();
        PlayerView = gameObject.GetComponent<PlayerView>();

        motor = GetComponent<CharacterMotor>();
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;

        originalRotation = transform.localRotation;
        if (PlayerView)
        {
            fovTemp = PlayerView.FPSCamera.MainCamera.fieldOfView;
            fovTarget = fovTemp;
        }
    }
//在游戏对象启动时调用，初始化角色、驱动器、玩家视图和角色运动控制器等组件，并设置初始相机视野。
    public void Zoom(float zoom)
    {
        // zoom
        fovTarget = zoom;
        zooming = !zooming;
    }
    //包括缩放视野、射击时的相机震动、隐藏武器、加速、攀爬、驾驶、移动、跳跃、瞄准、切换视图、坐下、获取当前武器等操作。
    public void Zoom(float zoom, Vector3 offset)
    {
        // zoom
        fovTarget = zoom;
        zooming = !zooming;
        aimPosition = offset;
        if (!zooming)
            aimPosition = Vector3.zero;
    }

    public void Kick(Vector2 power)
    {
        // kick function e.g when shoot a gun, camera kick up
        kickPower = power;
    }

    public void HideGun(bool visible)
    {
        // hide FPS camera
        if (PlayerView.FPSCamera.MainCamera)
        {
            PlayerView.FPSCamera.MainCamera.enabled = visible;
        }
    }

    public void Boost(float mult)
    {
        motor.boostMults = mult;
    }

    float climbDirection;

    public void Climb(float speed)
    {
        motor.Climb(speed);

    }

    public void Drive(Vector2 axis, bool jump)
    {
        if (Driver)
            Driver.Drive(axis, jump);
    }

    public void OutVehicle()
    {
        if (Driver)
            Driver.OutVehicle();
    }

    public void MoveCommand(Vector3 direction, bool jump)
    {
        if (Driver.DrivingSeat == null)
        {
            Move(direction);
            Jump(jump);
        }
        else
        {
            Drive(new Vector2(direction.x, direction.z), jump);
        }
    }

    public void Move(Vector3 directionVector)
    {
        // move along with direction.
        if (character == null)
            return;

        inputDirection = directionVector;
        if (directionVector != Vector3.zero)
        {
            var directionLength = directionVector.magnitude;
            directionVector = directionVector / directionLength;
            directionLength = Mathf.Min(1, directionLength);
            directionLength = directionLength * directionLength;
            directionVector = directionVector * directionLength;
        }

        Quaternion rotation = transform.rotation;

        if (PlayerView.FPSCamera)
        {
            rotation = PlayerView.FPSCamera.transform.rotation;
        }
        Vector3 angle = rotation.eulerAngles;
        angle.x = 0;
        angle.z = 0;
        rotation.eulerAngles = angle;
        character.MoveTo(rotation * directionVector);
    }

    public void Sprint(bool sprint)
    {
        character.Sprint = sprint;
    }

    public void Jump(bool jump)
    {
        // jump funtion
        motor.inputJump = jump;
    }

    public void Aim(Vector2 direction)
    {
        // aim function
        mouseDirection = direction;
    }

    public void SwithView()
    {
        PlayerView.SwithView();
    }
    public void SwithSideView()
    {
        PlayerView.SwithViewSide();
    }

    public void Sit()
    {

        character.Sit();
    }

    public FPSItemEquipment FPSItem()
    {
        if (character == null)
            return null;

        if (character.inventory == null)
            return null;

        return character.inventory.FPSEquipment;
    }

    public void Trigger1(bool fire)
    {
        FPSItemEquipment FPSitem = FPSItem();
        if (FPSitem == null)
            return;

        if (fire)
        {
            FPSitem.Trigger();
        }
        else
        {
            FPSitem.OnTriggerRelease();
        }
    }

    public void Trigger2(bool fire)
    {
        FPSItemEquipment FPSitem = FPSItem();
        if (FPSitem == null)
            return;

        if (fire)
        {
            FPSitem.Trigger2();
        }
        else
        {
            FPSitem.OnTrigger2Release();
        }
    }

    public void Reload()
    {
        if (character == null)
            return;

        if (character.inventory != null && character.inventory.FPSEquipment != null)
            character.inventory.FPSEquipment.Reload();
    }

    public void Interactive()
    {
        if (character == null)
            return;

        if (character.inventory != null)
            character.Interactive(PlayerView.FPSCamera.RayPointer.position, PlayerView.FPSCamera.RayPointer.forward);
    }

    public void Checking()
    {
        if (character == null)
            return;

        character.Checking(PlayerView.FPSCamera.RayPointer.position, PlayerView.FPSCamera.RayPointer.forward);
    }

    void Update()
    {
        if ((!MouseLock.MouseLocked && !MouseLock.IsMobileControl) || character == null)
            return;

        if (character.IsMine)
        {
            
            if (!zooming)
            {
                // mouse sensitivity is normal
                sensitivityXMult = 1;
                sensitivityYMult = sensitivityXMult;
                fovTarget = fovTemp;
            }
            else
            {
                // mouse sensitivity must decrease when zoom
                sensitivityXMult = fovTarget / fovTemp;
                sensitivityYMult = sensitivityXMult;
            }
            if (PlayerView.FPSCamera.MainCamera)
            {
                PlayerView.FPSCamera.MainCamera.fieldOfView = Mathf.Lerp(PlayerView.FPSCamera.MainCamera.fieldOfView, fovTarget, 0.5f);

            }
            if (PlayerView)
            {
                PlayerView.FPSCamera.aimOffset = aimPosition;
                PlayerView.FPSCamera.zooming = zooming;
            }

            // sprint speed.
            motor.boostMults += (1 - motor.boostMults) * Time.deltaTime;

            // camera kick
            kickPower.y += (0 - kickPower.y) / 20f;
            kickPower.x += (0 - kickPower.x) / 20f;

            // calculation all rotation with mouse direction and sensitivity
            rotationXtemp += (mouseDirection.x * sensitivityX * sensitivityXMult);
            rotationYtemp += (mouseDirection.y * sensitivityY * sensitivityYMult);
            // add Lerp for more smooth rotation
            rotationX = Mathf.Lerp(rotationX, rotationXtemp, DampingSpeed * Time.deltaTime);
            rotationY = Mathf.Lerp(rotationY, rotationYtemp, DampingSpeed * Time.deltaTime);

            // set rotation to 0 when it more than 360 degree
            if (rotationX >= 360)
            {
                rotationX = 0;
                rotationXtemp = 0;
            }
            // set rotation to 0 when it less than -360 degree
            if (rotationX <= -360)
            {
                rotationX = 0;
                rotationXtemp = 0;
            }

            // limit rotation
            if(character.MovementPreset.Length > character.MovementIndex)
            {
                maximumY = character.MovementPreset[character.MovementIndex].FPSCameraMaxY;
                minimumY = character.MovementPreset[character.MovementIndex].FPSCameraMinY;
            }

            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);
            rotationYtemp = ClampAngle(rotationYtemp, minimumY, maximumY);

            // get actual Quaternion. X and Y
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX + kickPower.x, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY + kickPower.y, Vector3.left);

            if (PlayerView.FPSCamera)
            {
                PlayerView.FPSCamera.transform.localRotation = originalRotation * Quaternion.AngleAxis(0, Vector3.up) * yQuaternion;
                this.transform.localRotation = (originalRotation * xQuaternion * Quaternion.AngleAxis(0, Vector3.left));
            }
            else
            {
                this.transform.localRotation = originalRotation * xQuaternion * yQuaternion;
            }
        }
    }
//每帧更新角色的行为，包括处理鼠标输入、旋转视角、设置相机视野、移动角色等操作。
    public void Stun(float val)
    {
        // kick function e.g when shoot a gun, camera kick up
        kickPower.y = val;
    }

    static float ClampAngle(float angle, float min, float max)
    {
        // just clamp angle function
        if (angle < -360.0f)
            angle += 360.0f;

        if (angle > 360.0f)
            angle -= 360.0f;

        return Mathf.Clamp(angle, min, max);
    }
}
