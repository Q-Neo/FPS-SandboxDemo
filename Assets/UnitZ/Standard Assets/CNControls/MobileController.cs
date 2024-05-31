using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CnControls;
//引用了System.Collections、System.Collections.Generic、UnityEngine和CnControls命名空间。
//使用了[RequireComponent(typeof(FPSController))]属性，确保对象上有FPSController组件。

[RequireComponent(typeof(FPSController))]
public class MobileController : MonoBehaviour
{

    public GameObject[] controls;
    //存储UI控件的数组。
    void Start()
    {

        if (controls.Length <= 0)
        {
            controls = new GameObject[this.transform.childCount];
            for (int i = 0; i < this.transform.childCount; i++)
            {
                controls[i] = this.transform.GetChild(i).gameObject;
            }
        }
    }
    //在对象启动时调用，初始化UI控件数组。如果未手动分配控件，则自动将子对象作为控件添加到数组中。
    void SetVisible(bool visible)
    {
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i].SetActive(visible);
        }
    }

    public void SwithView()
    {
        if (UnitZ.playerManager.PlayingCharacter != null)
        {
            FPSController fpsControl = UnitZ.playerManager.PlayingCharacter.GetComponent<FPSController>();
            if (fpsControl)
                fpsControl.SwithView();
        }
    }
    //控制UI控件的可见性。

    public void SwithSideView()
    {
        if (UnitZ.playerManager.PlayingCharacter != null)
        {
            FPSController fpsControl = UnitZ.playerManager.PlayingCharacter.GetComponent<FPSController>();
            if (fpsControl)
                fpsControl.SwithSideView();
        }
    }
    //用于切换游戏角色的视角

    void Update()
    {

        if (UnitZ.playerManager.PlayingCharacter != null)
        {
            SetVisible(true);
            FPSInputController oldController = UnitZ.playerManager.PlayingCharacter.GetComponent<FPSInputController>();
            if (oldController)
            {
                oldController.enabled = false;
            }

            FPSController fpsControl = UnitZ.playerManager.PlayingCharacter.GetComponent<FPSController>();
            if (fpsControl)
            {
                MouseLock.IsMobileControl = true;
                fpsControl.MoveCommand(new Vector3(CnInputManager.GetAxis("Horizontal"), 0, CnInputManager.GetAxis("Vertical")), CnInputManager.GetButton("Jump"));
                fpsControl.Aim(new Vector2(CnInputManager.GetAxis("Touch X"), CnInputManager.GetAxis("Touch Y")));
                fpsControl.Trigger1(CnInputManager.GetButton("Touch Fire1"));
                fpsControl.Trigger2(CnInputManager.GetButtonDown("Fire2"));

                if (CnInputManager.GetButtonDown("Fire3"))
                {
                    fpsControl.OutVehicle();
                    fpsControl.Interactive();
                }

                if (CnInputManager.GetButtonDown("Submit"))
                {
                    fpsControl.Reload();
                }

                fpsControl.Checking();
            }

        }
        else
        {
            SetVisible(false);
        }
    }
}
//在每帧更新时检查游戏角色的存在。
//果角色存在，则启用相关控制器，并根据移动设备上的输入更新角色的移动、瞄准、交互等操作。
//如果角色不存在，则隐藏UI控件。