using UnityEngine;
using System.Collections;

namespace CybSDK
{

    public class CVirtHapticEmitter : MonoBehaviour
    {

        private CVirtHapticListener hapticListener;
        public bool autoStart = false;
        protected bool playing = false;

        public bool loop = false;
        public float duration = 3.0f;
        public float distance = 4.0f;

        public AnimationCurve forceOverTime = AnimationCurve.EaseInOut(0, 1, 1, 0);
        public AnimationCurve forceOverDistance = AnimationCurve.Linear(0, 0, 1, 1);

        private float timeStart;

        // Use this for initialization
        protected virtual void Start()
        {
            if (this.autoStart == true)
            {
                this.Play();
            }
        }

        public void Play()
        {
            if (this.playing == true)
                this.Stop();

            this.playing = true;
            //
            timeStart = Time.time;
            hapticListener = CVirtHapticListener.getInstance();
            if (hapticListener != null)
                hapticListener.AddEmitter(this);
        }

        public void Stop()
        {
            if (this.playing == true)
            {
                this.playing = false;
                //
                if (hapticListener != null)
                    hapticListener.RemoveEmitter(this);
            }
        }

        // Use this for deinitialization
        void OnDestroy()
        {
            this.Stop();
        }

        public virtual float EvaluateForce(Vector3 listenerPosition)
        {
            if (this.isActiveAndEnabled == false)
                return 0f;

            float timeDelta = (Time.time - timeStart) % duration;
            int loopCount = (int)((Time.time - timeStart) / duration);

            if (playing == true && (loop || loopCount == 0))
            {
                //Evaluate time
                float forceTimeP = forceOverTime.Evaluate(timeDelta / duration);

                //Evaluate distance
                float dist = Vector3.Distance(this.transform.position, listenerPosition) / distance;
                dist = Mathf.Max(Mathf.Min(dist, 1), 0);
                float forceDistP = 1 - forceOverDistance.Evaluate(dist);
                return forceTimeP * forceDistP;
            }
            else
            {
                return 0;
            }
        }

    }

}
