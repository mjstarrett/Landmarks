using UnityEngine;
using System.Collections;
using KATVR;
public class KATDevice : MonoBehaviour {

    #region Common Variable - 通用变量
    public enum DeviceTypeList { KAT_WALK, ComingSoon };
    [HideInInspector]
    public DeviceTypeList device;
    public enum LanguageList { 简体中文, English}
    [HideInInspector]
    public LanguageList displayLanguage;

    [HideInInspector]
    public Transform targetMoveObject, targetRotateObject, vrCameraRig, vrHandset;
    #endregion

    [HideInInspector]
    public KATDevice_Walk KWalk;

    [HideInInspector]
    public float multiply, multiplyBack;
    public enum MovementStyleList { Translate, Velocity }
    [HideInInspector]
    public MovementStyleList MovementStyle;
    [HideInInspector]
    public Rigidbody target_Rig;
    [HideInInspector]
    public KeyCode ResetCameraKey;

    void Awake()
    {
        SetupDevice(device);
    }

    void Start()
    {
        ActiveDevice(device);
    }

    void Update () {
        DeviceUpdate(device);
	}

    #region Common Function
    public void SetCameraController(Transform CameraRig)
    {

    }
    #endregion

    #region 启动函数

    public void SetupDevice(DeviceTypeList Type)
    {
        switch (Type)
        {
            case DeviceTypeList.KAT_WALK:
                KWalk = this.gameObject.AddComponent<KATDevice_Walk>();
                KATVR_Global.KDevice_Walk = KWalk;
                break;
            case DeviceTypeList.ComingSoon:
                break;
            default:
                break;
        }
    }

    public void ActiveDevice(DeviceTypeList Type)
    {
        switch (Type)
        {
            case DeviceTypeList.KAT_WALK:
                KWalk.Initialize(1);
                KWalk.LaunchDevice();
                if (target_Rig == null)
                    if (targetMoveObject.GetComponent<Rigidbody>())
                        target_Rig = targetMoveObject.GetComponent<Rigidbody>();
                    else {
                        MovementStyle = MovementStyleList.Translate;
                        Debug.LogWarning("未能找到目标移动对象上的Rigidbody组件,移动方式将转换为Translate。\nCan not find Rigidbody component in Movement Object, the Movement Style will be changed to Translate.");
                    }
                break;
            case DeviceTypeList.ComingSoon:
                break;
            default:
                break;
        }
    }
    public void DeviceUpdate(DeviceTypeList Type)
    {
        switch (Type)
        {
            case DeviceTypeList.KAT_WALK:
                KWalk.UpdateData();
                TargetTransform(MovementStyle);
                if (Input.GetKeyDown(ResetCameraKey))
                    KWalk.ResetCamera(vrHandset);
                break;
            case DeviceTypeList.ComingSoon:
                break;
            default:
                break;
        }
    }
    #endregion

    #region Function For KAT WALK

    void TargetTransform(MovementStyleList Type)
    {
        vrCameraRig.position = targetRotateObject.position;
        if (KWalk.data_moveDirection > 0) KWalk.data_moveSpeed *= multiply;
        else if (KWalk.data_moveDirection < 0) KWalk.data_moveSpeed *= multiplyBack;
        switch (Type)
        {
            #region Translate
            case MovementStyleList.Translate:
                targetMoveObject.Translate(targetRotateObject.forward / 100 * KWalk.data_moveSpeed * KWalk.data_moveDirection);
                targetRotateObject.localEulerAngles = new Vector3(targetRotateObject.localEulerAngles.x, KWalk.data_bodyYaw, targetRotateObject.localEulerAngles.z);
                break;
            #endregion
            #region Velocity
            case MovementStyleList.Velocity:
                target_Rig.velocity = targetRotateObject.forward * KWalk.data_moveSpeed * KWalk.data_moveDirection;
                targetRotateObject.localEulerAngles = new Vector3(targetRotateObject.localEulerAngles.x, KWalk.data_bodyYaw, targetRotateObject.localEulerAngles.z);
                break;
            #endregion
            default:
                break;
        }
    }
    #endregion
}
