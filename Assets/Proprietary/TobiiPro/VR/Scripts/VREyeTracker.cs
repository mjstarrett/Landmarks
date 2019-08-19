//-----------------------------------------------------------------------
// Copyright © 2019 Tobii Pro AB. All rights reserved.
//-----------------------------------------------------------------------

using System.Threading;
using UnityEngine;

namespace Tobii.Research.Unity
{
    public class VREyeTracker : EyeTrackerBase
    {
        #region Public Properties

        /// <summary>
        /// Get <see cref="VREyeTracker"/> instance. This is assigned
        /// in Awake(), so call earliest in Start().
        /// </summary>
        public static VREyeTracker Instance { get; private set; }

        /// <summary>
        /// Get the latest gaze data. If there are new arrivals,
        /// they will be processed before returning.
        /// </summary>
        public IVRGazeData LatestGazeData
        {
            get
            {
                if (UnprocessedGazeDataCount > 0)
                {
                    // We have more data.
                    ProcessGazeEvents();
                }

                return _latestGazeData;
            }
        }

        /// <summary>
        /// Get the latest processed processed gaze data.
        /// Don't care if there a newer one has arrived.
        /// </summary>
        public IVRGazeData LatestProcessedGazeData { get { return _latestGazeData; } }

        /// <summary>
        /// Pop and get the next gaze data object from the queue.
        /// </summary>
        public IVRGazeData NextData
        {
            get
            {
                if (_gazeDataQueue.Count < 1)
                {
                    return default(IVRGazeData);
                }

                return _gazeDataQueue.Next;
            }
        }

        /// <summary>
        /// Connect or disconnect the gaze stream.
        /// </summary>
        public override bool SubscribeToGazeData
        {
            get
            {
                return _subscribeToGaze;
            }

            set
            {
                _subscribeToGaze = value;
                base.SubscribeToGazeData = value;
            }
        }

        public override int GazeDataCount { get { return _gazeDataQueue.Count; } }

        public override int UnprocessedGazeDataCount { get { return _originalGazeData.Count; } }

        #endregion Public Properties

        #region Private Fields

        ///// <summary>
        ///// The IEyeTracker instance.
        ///// </summary>
        //private IEyeTracker _eyeTracker = null;

        /// <summary>
        /// Flag to remember if we are subscribing to gaze data.
        /// </summary>
        private bool _subscribingToHMDGazeData;

        /// <summary>
        /// The eye tracker origin.
        /// </summary>
        private Transform _eyeTrackerOrigin;

        /// <summary>
        /// Locked access and size management.
        /// </summary>
        private LockedQueue<HMDGazeDataEventArgs> _originalGazeData = new LockedQueue<HMDGazeDataEventArgs>(maxCount: _maxGazeDataQueueSize);

        /// <summary>
        /// Size managed queue.
        /// </summary>
        private SizedQueue<IVRGazeData> _gazeDataQueue = new SizedQueue<IVRGazeData>(maxCount: _maxGazeDataQueueSize);

        /// <summary>
        /// The list of eye tracker poses kept for each frame.
        /// Just keep roughly one second of poses at 90 fps. 100 is a nice round number.
        /// </summary>
        private PoseList _eyeTrackerOriginPoses = new PoseList(100);

        /// <summary>
        /// Hold the latest processed gaze data. Initialized to an invalid object.
        /// </summary>
        private IVRGazeData _latestGazeData = new VRGazeData();

        #endregion Private Fields

        #region Override Methods

        protected override void OnAwake()
        {
            Instance = this;
            base.OnAwake();
        }

        protected override void OnStart()
        {
            // The eye tracker origin is not exactly in the camera position when using the SteamVR plugin in Unity.
            _eyeTrackerOrigin = VRUtility.EyeTrackerOriginVive;

            base.OnStart();
        }

        protected override void OnUpdate()
        {
            // Save the current pose for the current time.
            _eyeTrackerOriginPoses.Add(_eyeTrackerOrigin.GetPose(EyeTrackingOperations.GetSystemTimeStamp()));

            base.OnUpdate();
        }

        #endregion Override Methods

        #region Private Eye Tracking Methods

        protected override void ProcessGazeEvents()
        {
            const int maxIterations = 20;

            var gazeData = _latestGazeData;

            for (int i = 0; i < maxIterations; i++)
            {
                var originalGaze = _originalGazeData.Next;

                // Queue empty
                if (originalGaze == null)
                {
                    break;
                }

                var bestMatchingPose = _eyeTrackerOriginPoses.GetBestMatchingPose(originalGaze.SystemTimeStamp);
                if (!bestMatchingPose.Valid)
                {
                    Debug.Log("Did not find a matching pose");
                    continue;
                }

                gazeData = new VRGazeData(originalGaze, bestMatchingPose);
                _gazeDataQueue.Next = gazeData;
            }

            var queueCount = UnprocessedGazeDataCount;
            if (queueCount > 0)
            {
                Debug.LogWarning("We didn't manage to empty the queue: " + queueCount + " items left...");
            }

            _latestGazeData = gazeData;
        }

        protected override void StartAutoConnectThread()
        {
            if (_autoConnectThread != null)
            {
                return;
            }

            _autoConnectThread = new Thread(() =>
            {
                AutoConnectThreadRunning = true;

                while (AutoConnectThreadRunning)
                {
                    var eyeTrackers = EyeTrackingOperations.FindAllEyeTrackers();

                    foreach (var eyeTrackerEntry in eyeTrackers)
                    {
                        if (eyeTrackerEntry.SerialNumber.StartsWith("VR"))
                        {
                            FoundEyeTracker = eyeTrackerEntry;
                            AutoConnectThreadRunning = false;
                            return;
                        }
                    }

                    Thread.Sleep(200);
                }
            });

            _autoConnectThread.IsBackground = true;
            _autoConnectThread.Start();
        }

        protected override void UpdateSubscriptions()
        {
            if (_eyeTracker == null)
            {
                return;
            }

            if (SubscribeToGazeData && !_subscribingToHMDGazeData)
            {
                _eyeTracker.HMDGazeDataReceived += HMDGazeDataReceivedCallback;
                _subscribingToHMDGazeData = true;
            }
            else if (!SubscribeToGazeData && _subscribingToHMDGazeData)
            {
                _eyeTracker.HMDGazeDataReceived -= HMDGazeDataReceivedCallback;
                _subscribingToHMDGazeData = false;
            }
        }

        private void HMDGazeDataReceivedCallback(object sender, HMDGazeDataEventArgs eventArgs)
        {
            _originalGazeData.Next = eventArgs;
        }

        #endregion Private Eye Tracking Methods
    }
}