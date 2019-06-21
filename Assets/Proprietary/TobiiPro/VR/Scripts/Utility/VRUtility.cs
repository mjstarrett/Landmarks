//-----------------------------------------------------------------------
// Copyright © 2019 Tobii Pro AB. All rights reserved.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]

namespace Tobii.Research.Unity
{
    internal static class VRUtility
    {
        #region Private

        /// <summary>
        /// A transform to manipulate for the historical eye tracker origin.
        /// Use <see cref="TemporaryTransformWithAppliedPose(EyeTrackerOriginPose)"/>
        /// to get this transform with a provided pose applied. It is
        /// temporary since the object will change to the next supplied pose
        /// on the next call.
        /// </summary>
        private static Transform _historicalEyeTrackerOrigin;

        #endregion Private

        #region Helpers

        /// <summary>
        /// Get lens cup separation from HMD. A negative number indicates an error.
        /// </summary>
        internal static float LensCupSeparation
        {
            get
            {
                if (!VRWrap.SteamVRActive)
                {
                    return -1;
                }

                var error = 0;
                var result = VRWrap.GetFloatProperty("Prop_UserIpdMeters_Float", ref error);

                if (error != 0) // ETrackedPropertyError.TrackedProp_Success = 0
                {
                    return -1;
                }

                return result;
            }
        }

        /// <summary>
        /// Get a <see cref="Transform"/> object with the provided pose applied.
        /// It is temporary since it will change to then next provided pose on
        /// the next call, or by direct manipulation.
        /// </summary>
        /// <param name="pose">The pose to apply</param>
        /// <returns>The temporary transform</returns>
        internal static Transform TemporaryTransformWithAppliedPose(EyeTrackerOriginPose pose)
        {
            return _historicalEyeTrackerOrigin.ApplyPose(pose);
        }

        /// <summary>
        /// Get the eye tracker origin when using a Vive with SteamVR in Unity3D. Slow, only use for first lookup.
        /// </summary>
        internal static Transform EyeTrackerOriginVive
        {
            get
            {
                var cam = GameObject.Find("Camera (eye)");

                if (!cam)
                {
                    cam = GameObject.Find("Main Camera (eye)");
                    if (!cam)
                    {
                        // Here we can suspect that we do not have SteamVR in the project.
                        cam = GameObject.Find("Main Camera");
                        if (!cam)
                        {
                            cam = GameObject.Find("Camera");
                            if (!cam)
                            {
                                Debug.LogError("Failed to find either \"Camera (eye)\", \"Main Camera (eye)\", \"Main Camera\", or even \"Camera\". Perhaps use GetEyeTrackerOrigin(GameObject cam) instead?");
                                return null;
                            }
                        }
                    }
                }

                return GetEyeTrackerOrigin(cam);
            }
        }

        /// <summary>
        /// Get the eyetracker origin when using a Vive with SteamVR in Unity3, but provide the eye camera as an argument.
        /// </summary>
        /// <param name="cam">The eye camera game object.</param>
        /// <returns>The eyetracker origin.</returns>
        internal static Transform GetEyeTrackerOrigin(GameObject cam)
        {
            var originGO = GameObject.Find("EyetrackerOrigin");

            if (originGO)
            {
                return originGO.transform;
            }

            originGO = new GameObject("EyetrackerOrigin");
            var eyetrackerOrigin = originGO.transform;
            eyetrackerOrigin.parent = cam.transform;

            // Create a hidden game object with a transform to manipulate by pose information.
            var historicalEyeTrackerOriginObject = new GameObject("Historical Eye Tracker Origin");
            historicalEyeTrackerOriginObject.hideFlags = HideFlags.HideInHierarchy;
            _historicalEyeTrackerOrigin = historicalEyeTrackerOriginObject.transform;

            var zOffs = 0.015f;

            if (VRWrap.SteamVRActive)
            {
                var error = 0;
                var h2eDepth = VRWrap.GetFloatProperty("Prop_UserHeadToEyeDepthMeters_Float", ref error);

                if (error == 0) // ETrackedPropertyError.TrackedProp_Success = 0
                {
                    zOffs = h2eDepth;
                    Debug.Log("Got head to eye z depth from HMD: " + zOffs);
                }
            }
            else
            {
                Debug.Log("We seem to not be using SteamVR. Assume native Unity (OpenVR) support. Use offset: " + zOffs);
            }

            eyetrackerOrigin.localPosition = new Vector3(0, 0, zOffs);
            eyetrackerOrigin.localRotation = Quaternion.identity;
            return eyetrackerOrigin;
        }

        #endregion Helpers
    }

    #region Queues and Lists

    /// <summary>
    /// Simple lock-protected queue. Will not grow above max count.
    /// </summary>
    /// <typeparam name="T">The class type for the queue</typeparam>
    internal sealed class LockedQueue<T>
    {
        private SizedQueue<T> _sizedQueue;

        /// <summary>
        /// Create a locked queue with size management.
        /// </summary>
        /// <param name="maxCount">Max size of the queue</param>
        internal LockedQueue(int maxCount)
        {
            _sizedQueue = new SizedQueue<T>(maxCount);
        }

        /// <summary>
        /// Enqueue or dequeue.
        /// </summary>
        internal T Next
        {
            get
            {
                lock (_sizedQueue)
                {
                    return _sizedQueue.Next;
                }
            }

            set
            {
                lock (_sizedQueue)
                {
                    _sizedQueue.Next = value;
                }
            }
        }

        /// <summary>
        /// Get queue size.
        /// </summary>
        internal int Count
        {
            get
            {
                lock (_sizedQueue)
                {
                    return _sizedQueue.Count;
                }
            }
        }
    }

    /// <summary>
    /// Simple size managed queue. Avoids overgrowing.
    /// </summary>
    /// <typeparam name="T">The class type for the queue</typeparam>
    internal sealed class SizedQueue<T>
    {
        private Queue<T> _queue = new Queue<T>();
        private int _maxCount;

        /// <summary>
        /// Create a size managed queue.
        /// </summary>
        /// <param name="maxCount">Max size of the queue</param>
        internal SizedQueue(int maxCount)
        {
            _maxCount = maxCount;
        }

        /// <summary>
        /// Enqueue or dequeue.
        /// </summary>
        internal T Next
        {
            get
            {
                if (_queue.Count < 1)
                {
                    return default(T);
                }

                return _queue.Dequeue();
            }

            set
            {
                _queue.Enqueue(value);

                // Manage queue size.
                while (_queue.Count > _maxCount)
                {
                    _queue.Dequeue();
                }
            }
        }

        /// <summary>
        /// Get queue size.
        /// </summary>
        internal int Count
        {
            get
            {
                return _queue.Count;
            }
        }
    }

    /// <summary>
    /// Size managed list of poses.
    /// </summary>
    internal sealed class PoseList
    {
        private List<EyeTrackerOriginPose> _list = new List<EyeTrackerOriginPose>();
        private int _maxCount;

        internal PoseList(int maxCount)
        {
            _maxCount = maxCount;
        }

        internal void Add(EyeTrackerOriginPose pose)
        {
            // Save the current pose for the current time.
            _list.Add(pose);

            while (_list.Count > _maxCount)
            {
                _list.RemoveAt(0);
            }
        }

        internal EyeTrackerOriginPose this[int index]
        {
            get
            {
                return _list[index];
            }

            set
            {
                _list[index] = value;
            }
        }

        internal int Count
        {
            get
            {
                return _list.Count;
            }
        }

        /// <summary>
        /// Look up the best matching pose corresponding to the provided time stamp.
        /// If the list is empty, an invalid pose will be returned.
        /// If the time stamp is:
        ///  - before the first pose, the first pose will be returned.
        ///  - after the last pose, the last pose will be returned.
        ///  - a perfect match, the corresponding pose will be returned.
        ///  - between two poses, the interpolated pose will be returned.
        /// </summary>
        /// <param name="timeStamp">The gaze time stamp in the system clock</param>
        /// <returns>The best matching pose</returns>
        internal EyeTrackerOriginPose GetBestMatchingPose(long timeStamp)
        {
            var comparer = new EyeTrackerOriginPose(timeStamp);
            var index = _list.BinarySearch(comparer);

            if (_list.Count == 0)
            {
                // No poses to compare. Return invalid object.
                return comparer;
            }

            if (index < 0)
            {
                // No direct hit. This should be the common case.
                // Bitwise complement gives the index of the next larger item.
                index = ~index;

                if (index > 0)
                {
                    if (index == _list.Count)
                    {
                        // There is no larger time stamp. Return the last item.
                        return _list[_list.Count - 1];
                    }

                    // Interpolate a new pose and return it.
                    return _list[index - 1].Interpolate(_list[index], timeStamp);
                }

                // If index is zero, then the time stamp we provided is before
                // the first item in the poses list. This is normally long ago.
                // Anyway, return the first item.
                return _list[0];
            }

            // Direct hit. Could happen, but should be very rare.
            return _list[index];
        }
    }

    #endregion Queues and Lists

    /// <summary>
    /// Reflection wrapper to avoid SteamVR dependency.
    /// </summary>
    internal static class VRWrap
    {
        private static Type _steamVRType;
        private static Type _openVRType;
        private static Type _cVRSystemType;
        private static object _steamVRInstance;
        private static object _openVRSystem;
        private static object _steamVRHMD;
        private static System.Reflection.MethodInfo _steamGetFloatTrackedDevicePropertyMethod;

        /// <summary>
        /// Simpler get float property from Vive.
        /// </summary>
        /// <param name="prop">The property to get</param>
        /// <param name="error">An error reference</param>
        /// <returns>The float value</returns>
        internal static float GetFloatProperty(string property, ref int error)
        {
            var hmd = SteamVRHMD;
            if (hmd == null)
            {
                error = 5; // TrackedProp_InvalidDevice = 5
                return 0;
            }

            if (_steamGetFloatTrackedDevicePropertyMethod == null)
            {
                _steamGetFloatTrackedDevicePropertyMethod = hmd.GetType().GetMethod("GetFloatTrackedDeviceProperty");

                if (_steamGetFloatTrackedDevicePropertyMethod == null)
                {
                    error = 5; // TrackedProp_InvalidDevice = 5
                    return 0;
                }
            }

            var propertyType = _steamGetFloatTrackedDevicePropertyMethod.GetParameters()[1].ParameterType;
            var arguments = new object[] { (uint)0, Enum.Parse(propertyType, property), error };
            var result = (float)_steamGetFloatTrackedDevicePropertyMethod.Invoke(hmd, arguments);

            if (error != 0)
            {
                return 0;
            }

            return result;
        }

        internal static Type SteamVRType
        {
            get
            {
                if (_steamVRType == null)
                {
                    _steamVRType = Type.GetType("SteamVR", throwOnError: false);
                    if (_steamVRType == null)
                    {
                        // Newer SteamVR versions have moved the SteamVR class into the Valve.VR namespace.
                        _steamVRType = Type.GetType("Valve.VR.SteamVR", throwOnError: false);
                    }
                }

                return _steamVRType;
            }
        }

        internal static Type OpenVRType
        {
            get
            {
                if (_openVRType == null)
                {
                    _openVRType = Type.GetType("Valve.VR.OpenVR", throwOnError: false);
                }

                return _openVRType;
            }
        }

        internal static Type CVRSystemType
        {
            get
            {
                if (_cVRSystemType == null)
                {
                    _cVRSystemType = Type.GetType("Valve.VR.CVRSystem", throwOnError: false);
                }

                return _cVRSystemType;
            }
        }

        internal static object SteamVRInstance
        {
            get
            {
                if (_steamVRInstance == null)
                {
                    var steamVRType = SteamVRType;

                    if (steamVRType == null)
                    {
                        return null;
                    }

                    var property = steamVRType.GetProperty("instance");

                    if (property == null)
                    {
                        return null;
                    }

                    _steamVRInstance = property.GetValue(null, null);
                }

                return _steamVRInstance;
            }
        }

        internal static object OpenVRSystem
        {
            get
            {
                if (_openVRSystem == null)
                {
                    var openVRType = OpenVRType;

                    if (openVRType == null)
                    {
                        return null;
                    }

                    var property = openVRType.GetProperty("System");

                    if (property == null)
                    {
                        return null;
                    }

                    _openVRSystem = property.GetValue(null, null);
                }

                return _openVRSystem;
            }
        }

        internal static bool SteamVRActive
        {
            get
            {
                var steamVRType = SteamVRType;

                if (steamVRType == null)
                {
                    return false;
                }

                var steamVRActive = steamVRType.GetProperty("active").GetValue(null, null);

                if (steamVRActive == null)
                {
                    return false;
                }

                return (bool)steamVRActive;
            }
        }

        internal static object SteamVRHMD
        {
            get
            {
                if (_steamVRHMD == null)
                {
                    var steamVRType = SteamVRType;

                    if (steamVRType == null)
                    {
                        return null;
                    }

                    var steamVRInstance = SteamVRInstance;

                    if (steamVRInstance == null)
                    {
                        return null;
                    }

                    _steamVRHMD = steamVRType.GetProperty("hmd").GetValue(steamVRInstance, null);
                }

                return _steamVRHMD;
            }
        }
    }
}