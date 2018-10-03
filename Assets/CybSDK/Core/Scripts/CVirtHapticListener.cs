using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CybSDK
{
    [RequireComponent(typeof(CVirtDeviceController))]
    public class CVirtHapticListener : MonoBehaviour
    {

        private static CVirtHapticListener singleInstance;

        public static CVirtHapticListener getInstance()
        {
            return singleInstance;
        }

        private List<CVirtHapticEmitter> emitters = new List<CVirtHapticEmitter>();

        private CVirtDeviceController deviceController;
        public int maxRange = 60;

        // Use this to set singletion instance
        void Awake()
        {
            if (singleInstance != null)
                Debug.LogError("There are more than one CVirtHapticListeners in the scene. Please ensure there is always exactly one CVirtHapticListener in the scene.");
            singleInstance = this;

            //Check if this object has a CVirtDeviceController attached
            deviceController = GetComponent<CVirtDeviceController>();
            if (deviceController == null)
                Debug.LogError("CVirtHapticListener gameobject does not have a CVirtDeviceController attached.");

            if (deviceController != null)
            {
                deviceController.OnCVirtDeviceControllerCallback += this.OnCVirtDeviceControllerCallback;
            }
        }

        public void OnCVirtDeviceControllerCallback(CVirtDevice virtDevice, CVirtDeviceController.CVirtDeviceControllerCallbackType callbackType)
        {
            switch (callbackType)
            {
                case CVirtDeviceController.CVirtDeviceControllerCallbackType.Connect:
                    virtDevice.HapticSetGain(4);
                    virtDevice.HapticSetFrequency(60);
                    virtDevice.HapticSetVolume(0);
                    //
                    virtDevice.HapticPlay();
                    break;

                case CVirtDeviceController.CVirtDeviceControllerCallbackType.Disconnect:
                    virtDevice.HapticStop();
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (deviceController != null && deviceController.GetDevice() != null)
            {
                CVirtDevice virtDevice = deviceController.GetDevice();

                float sumForce = 0;

                foreach (CVirtHapticEmitter emitter in emitters)
                {
                    float distance = Vector3.Distance(this.transform.position, emitter.transform.position);
                    if (distance < maxRange && distance < emitter.distance)
                    {
                        float force = emitter.EvaluateForce(this.transform.position);
                        if (sumForce < force)
                            sumForce = force;
                        //sumForce = SumUpDecibal(force, force);
                    }
                }

                if (virtDevice.HasHaptic())
                {
                    virtDevice.HapticSetVolume(Mathf.FloorToInt(100f * sumForce));
                }
            }
        }

        void OnDestroy()
        {
            if (deviceController != null && deviceController.GetDevice() != null)
            {
                CVirtDevice virtDevice = deviceController.GetDevice();
                if (virtDevice.HasHaptic())
                {
                    virtDevice.HapticStop();
                }
            }
        }

        private float SumUpDecibal(float a, float b)
        {
            var c = Mathf.Pow(10, a / 10);
            var d = Mathf.Pow(10, b / 10);
            var d2 = c + d;
            var d3 = Mathf.Log(d2) / Mathf.Log(10);
            var e = 10 * d3;
            var answer = (int)(1000 * e) / 1000;
            return answer;
        }

        public void AddEmitter(CVirtHapticEmitter emitter)
        {
            emitters.Add(emitter);
        }

        public void RemoveEmitter(CVirtHapticEmitter emitter)
        {
            emitters.Remove(emitter);
        }

    }

}
