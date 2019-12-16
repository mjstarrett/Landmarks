using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
namespace KATVR
{
    public class KATDevice_Walk : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        #region Basic Variable - 基础变量

        /* Runtime是否启动 */
        public static bool Launched;
        /* 身体转向角度 */
        public static int bodyYaw;
        /* 是否移动 */
        public static int isMoving;
        /* 前进方向 -1 为前进 0 为停止 1 为倒退 */
        public static int moveDirection;
        /* 默认移动速度 从0到1*/
        public static float moveSpeed;
        /* 行走的能量值 */
        public static double WalkPower;
        /* 玩家在现实中行走的距离 单位是米 */
        public static float meter;
        /* 最大移动能量 */
        public static float maxMovePower, bodyRotation;
        private static float newBodyYaw, newCameraYaw;




        #region Rec
        //[HideInInspector]
        public float data_bodyYaw, data_meter, data_moveSpeed, data_DisplayedSpeed;
        //[HideInInspector]
        public double data_walkPower;
        //[HideInInspector]
        public int data_moveDirection, data_isMoving;
        #endregion

        #endregion


        #region Function - 函数使用
        public void Initialize(int count)
        {
            Ini(count);
        }
        public bool LaunchDevice()
        {
            if (CheckForLaunch())
            {
                Launch();
                Launched = true;
            }
            else
            {
                Launched = false;
            }
            return Launched;
        }
        public bool Stop()
        {
            Halt();
            return true;
        }
        public void UpdateData()
        {
            if (Launched)
            {
                GetWalkerData(0, ref bodyYaw, ref WalkPower, ref moveDirection, ref isMoving, ref meter);

                bodyYaw = (int)Math.Floor((float)bodyYaw / 1024 * 360);
                bodyRotation = (float)bodyYaw - newBodyYaw + newCameraYaw;
                WalkPower = Math.Round((double)WalkPower, 2);
                moveSpeed = (float)WalkPower / 3000f;
                moveDirection = -moveDirection;
                if (moveSpeed > 1) moveSpeed = 1;
                else if (moveSpeed < 0.3f) moveSpeed = 0;
                data_bodyYaw = bodyRotation;
                data_walkPower = WalkPower;
                data_moveSpeed = data_DisplayedSpeed = moveSpeed;
                data_moveDirection = moveDirection;
                data_isMoving = isMoving;
                data_meter = meter;
            }
        }

        public void ResetCamera(Transform handset)
        {
            if (handset != null)
            {
                newCameraYaw = handset.transform.localEulerAngles.y;
                newBodyYaw = (float)bodyYaw;
            }
            else
            {
                Debug.LogError("数据不存在");
            }
        }
        #endregion

        #region Dllinput - 动态链接库

        [DllImport("walkerbase", CallingConvention = CallingConvention.Cdecl)]
        static extern void Ini(int count);

        [DllImport("walkerbase", CallingConvention = CallingConvention.Cdecl)]
        static extern int Launch();

        [DllImport("walkerbase", CallingConvention = CallingConvention.Cdecl)]
        static extern void Halt();

        [DllImport("walkerbase", CallingConvention = CallingConvention.Cdecl)]
        static extern bool GetWalkerData(int index, ref int bodyyaw, ref double walkpower, ref int movedirection, ref int ismoving, ref float distancer);

        [DllImport("walkerbase", CallingConvention = CallingConvention.Cdecl)]
        static extern bool CheckForLaunch();

        #endregion
    }
}

