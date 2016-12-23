using System.Collections.Generic;
using System.Threading;
using Neo.IO.Files.Models;

namespace Neo.Scene.Models.M2
{
	internal class StaticAnimationThread
    {
        public static StaticAnimationThread Instance { get; private set; }

        private Thread mThread;
        private readonly List<IM2Animator> mAnimators = new List<IM2Animator>();
        private bool mIsRunning;

        static StaticAnimationThread()
        {
            Instance = new StaticAnimationThread();
        }

        public void Initialize()
        {
	        this.mIsRunning = true;
	        this.mThread = new Thread(AnimationProc);
	        this.mThread.Start();
        }

        public void Shutdown()
        {
	        this.mIsRunning = false;
	        this.mThread.Join();
        }

        public void AddAnimator(IM2Animator animator)
        {
            lock (this.mAnimators)
            {
	            this.mAnimators.Add(animator);
            }
        }

        public void RemoveAnimator(IM2Animator animator)
        {
            lock (this.mAnimators)
            {
	            this.mAnimators.Remove(animator);
            }
        }

        private void AnimationProc()
        {
            while(this.mIsRunning)
            {
                lock(this.mAnimators)
                {
                    foreach (var animator in this.mAnimators)
                    {
	                    animator.Update(null);
                    }
                }

                Thread.Sleep(20);
            }
        }
    }
}
