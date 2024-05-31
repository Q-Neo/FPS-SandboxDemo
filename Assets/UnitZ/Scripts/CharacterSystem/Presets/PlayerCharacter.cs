using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(CharacterFootStep))]
[RequireComponent(typeof(CharacterAnimation))]
[RequireComponent(typeof(CharacterDriver))]
[RequireComponent(typeof(CharacterLiving))]
[RequireComponent(typeof(CharacterInventory))]
[RequireComponent(typeof(FPSController))]
[RequireComponent(typeof(CharacterMotor))]
//这些属性会在该脚本所附加的游戏对象上强制添加指定类型的组件，确保了PlayerCharacter对象拥有所需的所有组件。
public class PlayerCharacter : CharacterSystem
{
    [HideInInspector]
    public bool ToggleFlashlight = false;
    //声明了一个公有的布尔类型变量ToggleFlashlight，用于控制手电筒的开关状态，默认值为false。
    [SyncVar]
    public Quaternion CameraRotation;
    //声明了一个同步变量CameraRotation，用于存储摄像机的旋转信息。
    public override void Start()
    {
        DestroyOnDead = false;
        if (animator)
            animator.SetInteger("Shoot_Type", AttackType);
        base.Start();
    }
    //重写了父类的Start方法，在游戏对象被启用时调用。在方法中设置了DestroyOnDead变量为false，并根据是否存在动画组件来设置射击类型。
    public override void Update()
    {
        if (animator == null)
            return;
        animator.SetInteger("UpperState", 1);
        base.Update();
    }
    //重写了父类的Update方法，在每一帧更新时调用。在方法中设置了动画的上半身状态。
    public override void PlayMoveAnimation(float magnitude)
    {
        if (animator)
        {
            if (magnitude > 0.4f)
            {
                animator.SetInteger("LowerState", 1);
            }
            else
            {
                animator.SetInteger("LowerState", 0);
            }
        }

        base.PlayMoveAnimation(magnitude);
    }
    //重写了父类的PlayMoveAnimation方法，用于播放移动动画。根据传入的移动速度参数，控制播放移动动画的状态。
    public override void PlayAttackAnimation(bool attacking, int attacktype)
    {
        if (animator)
        {
            if (attacking)
            {
                animator.SetTrigger("Shoot");
            }
            animator.SetInteger("Shoot_Type", attacktype);
        }
        base.PlayAttackAnimation(attacking, attacktype);
    }
    //重写了父类的PlayAttackAnimation方法，用于播放攻击动画。根据传入的攻击状态和攻击类型参数，控制播放攻击动画的状态和类型。

    public override void OnThisThingDead()
    {
        if (NetID != -1)
        {
            RemoveCharacterData();
        }

        if (UnitZ.NetworkObject().scoreManager)
        {
            UnitZ.NetworkObject().scoreManager.AddDead(1, NetID);
            if (NetID != LastHitByID)
                UnitZ.NetworkObject().scoreManager.AddScore(1, LastHitByID);
        }


        CharacterItemDroper itemdrop = this.GetComponent<CharacterItemDroper>();
        if (itemdrop)
            itemdrop.DropItem();
        base.OnThisThingDead();
    }
}
//重写了父类的OnThisThingDead方法，用于在玩家角色死亡时触发。在方法中移除了角色数据、更新了得分信息、丢弃了角色物品，并调用了父类的OnThisThingDead方法。