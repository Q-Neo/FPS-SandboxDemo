using UnityEngine;
using System.Collections;

public class FPSCamera : MonoBehaviour
{
    public Vector3 ThirdViewOffset = new Vector3(1, 0, -2);
    public float ThirdViewZoomMult = 1;
    public int ThirdViewInvert = 1;
    public Camera MainCamera;
    public ItemSticker FPSItemView;
    public Transform RayPointer;
    [HideInInspector]
    public float HorizonSway = 0.4f;
    public float VerticalSway = 0.4f;
    public Vector3 aimOffset;
    public bool IsThirdView;
    public bool zooming;
    public bool hideWhenScoping;

    private Vector3 swayOffset;
    private Vector3 positionTmpOffset;
    private Vector3 rootDirTmp;
    private Vector3 fpsDirTmp;
    private float rootDirHDot;
    private float rootDirVDot;
    private Vector3 rootDifPos;
    private bool isThirdViewTmp;


    //ThirdViewOffset：第三人称视角相对于角色位置的偏移量。
    //ThirdViewZoomMult：第三人称视角缩放倍数。
    //ThirdViewInvert：第三人称视角的反向设置。
    //MainCamera：主摄像机对象的引用。
    //FPSItemView：第一人称视角下的玩家物品视图。
    //RayPointer：用于指向物体的射线指针。
    //HorizonSway：水平摇摆参数。
    //VerticalSway：垂直摇摆参数。
    //aimOffset：瞄准时的偏移量。
    //IsThirdView：是否处于第三人称视角。
    //zooming：是否处于缩放状态。
    //hideWhenScoping：在缩放时是否隐藏物体。
    void Start()
    {
        positionTmpOffset = FPSItemView.transform.localPosition;
    }
//在开始时，将 FPSItemView 的本地位置赋值给 positionTmpOffset，用于后续的位置偏移计算。
    public void Aim(Vector3 offset)
    {
        aimOffset = offset;
    }
//设置瞄准时的偏移量。

    public void Zoom()
    {

    }

    void Update()
    {
        if (FPSItemView == null)
            return;
//当 FPSItemView 为空时，直接返回。

        if ((IsThirdView && !zooming) || (IsThirdView && (zooming && !hideWhenScoping)))
        {
            RaycastHit hitRay;
            Vector3 dir = this.transform.forward;
            if (Physics.Raycast(MainCamera.transform.position, MainCamera.transform.forward, out hitRay))
            {
                dir = (hitRay.point - RayPointer.position).normalized;

            }
            if (Vector3.Dot(dir, this.transform.forward) > 0.7f)
            {
                RayPointer.forward = dir;
            }

            Vector3 thirdPos = new Vector3(ThirdViewOffset.x * ThirdViewInvert, ThirdViewOffset.y, ThirdViewOffset.z * ThirdViewZoomMult);
            MainCamera.transform.localPosition = Vector3.Lerp(MainCamera.transform.localPosition, thirdPos, Time.deltaTime * 30);
            FPSItemView.transform.localScale = Vector3.zero;
        }
        //如果处于第三人称视角并且没有在缩放，或者处于第三人称视角并且正在缩放但不需要隐藏时：
        //发射一条射线，根据射线命中的物体调整摄像机的方向。
        //将主摄像机和 FPSItemView 进行相应的位置调整和缩放。
        else
        {
            MainCamera.transform.localPosition = Vector3.zero;
            FPSItemView.transform.localScale = Vector3.one;
            RayPointer.forward = this.transform.forward;
        }
        //否则，恢复到第一人称视角，将射线指针指向摄像机正前方，并恢复 FPSItemView 的正常大小。
        if (isThirdViewTmp != IsThirdView)
        {
            this.gameObject.transform.root.gameObject.SendMessageUpwards("OnViewChanged", SendMessageOptions.DontRequireReceiver);
            isThirdViewTmp = IsThirdView;
        }
        //如果当前视角状态与上一帧不同，则发送消息通知视角变化。

        float DirH = Vector3.Dot(this.gameObject.transform.root.transform.right.normalized, rootDirTmp.normalized) * HorizonSway;
        float DirV = Vector3.Dot(this.transform.up.normalized, fpsDirTmp.normalized) * VerticalSway;

        if (aimOffset != Vector3.zero)
        {
            DirH = 0;
            DirV = 0;
        }
        rootDirHDot = Mathf.Lerp(rootDirHDot, DirH, 10 * Time.deltaTime);
        rootDirVDot = Mathf.Lerp(rootDirVDot, DirV, 10 * Time.deltaTime);
        swayOffset.x = rootDirHDot;
        swayOffset.y = rootDirVDot;
        //计算摇摆效果，并根据瞄准状态调整水平和垂直摇摆。
        Vector3 offsetTarget = positionTmpOffset + (-swayOffset) + aimOffset;
        FPSItemView.transform.localPosition = Vector3.Lerp(FPSItemView.transform.localPosition, offsetTarget, 5 * Time.deltaTime);
        rootDirTmp = this.gameObject.transform.root.transform.forward;
        fpsDirTmp = this.transform.forward;
    }
//最后，根据计算出的位置偏移，将 FPSItemView 的本地位置逐渐调整到目标位置。
}
