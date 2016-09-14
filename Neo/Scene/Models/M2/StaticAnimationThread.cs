using System.Collections.Generic;
using System.Threading;
using SharpDX;
using WoWEditor6.IO.Files.Models;

namespace WoWEditor6.Scene.Models.M2
{
    class StaticAnimationThread
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
            mIsRunning = true;
            mThread = new Thread(AnimationProc);
            mThread.Start();
        }

        public void Shutdown()
        {
            mIsRunning = false;
            mThread.Join();
        }

        public void AddAnimator(IM2Animator animator)
        {
            lock (mAnimators)
                mAnimators.Add(animator);
        }

        public void RemoveAnimator(IM2Animator animator)
        {
            lock (mAnimators)
                mAnimators.Remove(animator);
        }

        private void AnimationProc()
        {
            while(mIsRunning)
            {
                lock(mAnimators)
                {
                    foreach (var animator in mAnimators)
                        animator.Update(null);
                }

                Thread.Sleep(20);
            }
        }
    }
}
