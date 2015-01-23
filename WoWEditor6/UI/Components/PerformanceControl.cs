using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Direct2D1;

namespace WoWEditor6.UI.Components
{
    class PerformanceControl : IComponent
    {
        private readonly List<long> mMemorySamples = new List<long>();
        private readonly List<float> mFpsSamples = new List<float>();

        private DateTime mLastMemorySample = DateTime.Now;
        private DateTime mLastFpsSample = DateTime.Now;

        private int mFrameCount;

        private RectangleF mFpsRectangle;
        private RectangleF mMemoryRectangle;
        private Vector2 mPosition;
        private Vector2 mSize;

        private readonly StaticText mFpsMax;
        private readonly StaticText mFpsMin;
        private readonly StaticText mMemoryMax;
        private readonly StaticText mMemoryMin;
        private readonly StaticText mFpsCaption;
        private readonly StaticText mMemoryCaption;

        public Vector2 Position { get { return mPosition; } set { ClientAreaChanged(value, mSize); } }
        public Vector2 Size { get { return mSize; } set { ClientAreaChanged(mPosition, value); } }

        public PerformanceControl()
        {
            mMemorySamples.Add(Environment.WorkingSet);
            mFpsSamples.Add(0.0f);

            mFpsMax = new StaticText
            {
                FontSize = 13.0f,
                Weight = SharpDX.DirectWrite.FontWeight.Bold
            };

            mFpsMin = new StaticText
            {
                FontSize = 13.0f,
                Weight = SharpDX.DirectWrite.FontWeight.Bold
            };

            mMemoryMax = new StaticText
            {
                FontSize = 13.0f,
                Weight = SharpDX.DirectWrite.FontWeight.Bold
            };

            mMemoryMin = new StaticText
            {
                FontSize = 13.0f,
                Weight = SharpDX.DirectWrite.FontWeight.Bold
            };

            mFpsCaption = new StaticText
            {
                FontSize = 13.0f,
                Weight = SharpDX.DirectWrite.FontWeight.Bold,
                Text = "FPS",
                HorizontalAlignment = SharpDX.DirectWrite.TextAlignment.Trailing
            };

            mMemoryCaption = new StaticText
            {
                FontSize = 13.0f,
                Weight = SharpDX.DirectWrite.FontWeight.Bold,
                Text = "RAM-Usage",
                HorizontalAlignment = SharpDX.DirectWrite.TextAlignment.Trailing
            };
        }

        public void Reset()
        {
            mMemorySamples.Clear();
            mFpsSamples.Clear();
            mLastFpsSample = DateTime.Now;
            mLastMemorySample = DateTime.Now;
            mMemorySamples.Add(Environment.WorkingSet);
            mFpsSamples.Add(0.0f);
        }

        public void OnRender(RenderTarget target)
        {
            ++mFrameCount;

            var now = DateTime.Now;
            if((now - mLastMemorySample).TotalSeconds > 0.5f)
            {
                mLastMemorySample = now;
                mMemorySamples.Add(Environment.WorkingSet);
                while (mMemorySamples.Count > 20)
                    mMemorySamples.RemoveAt(0);
            }

            if((now - mLastFpsSample).TotalSeconds >= 1.0f)
            {
                mFpsSamples.Add(mFrameCount / (float) (now - mLastFpsSample).TotalSeconds);
                mLastFpsSample = now;
                mFrameCount = 0;

                while (mFpsSamples.Count > 20)
                    mFpsSamples.RemoveAt(0);
            }

            //target.FillRectangle(mMemoryRectangle, Brushes.Solid[0xBB333333]);
            //target.DrawRectangle(mMemoryRectangle, Brushes.White);
            target.FillRectangle(mFpsRectangle, Brushes.Solid[0xBB333333]);
            target.DrawRectangle(mFpsRectangle, Brushes.White);

            //DrawMemorySamples(target);
            DrawFpsSamples(target);

            //target.DrawTextLayout(new Vector2(Position.X, mMemoryRectangle.Top - 20.0f), mMemoryCaption, Brushes.White);
            target.DrawTextLayout(new Vector2(Position.X, mFpsRectangle.Top - 20.0f), mFpsCaption, Brushes.White);
        }

        public void OnMessage(Message message)
        {

        }

        private void DrawMemorySamples(RenderTarget target)
        {
            var maxValue = mMemorySamples.Max();
            var minValue = mMemorySamples.Min();
            if (minValue == maxValue) maxValue = minValue + 50;

            var baseOffset = 0.0f;
            var step = mSize.X / 19.0f;
            var diff = (DateTime.Now - mLastMemorySample);

            if (mMemorySamples.Count == 20)
                baseOffset = ((float)diff.TotalSeconds / 0.5f) * step;

            var curPos = mMemoryRectangle.Left - baseOffset;
            var endY = mMemoryRectangle.Bottom - 5;
            var height = mMemoryRectangle.Height - 20;

            target.PushAxisAlignedClip(mMemoryRectangle, AntialiasMode.Aliased);

            for (var i = 1; i < mMemorySamples.Count; ++i)
            {
                var sat = (mMemorySamples[i] - (float)minValue) / (maxValue - minValue);
                var satPrev = (mMemorySamples[i - 1] - (float)minValue) / (maxValue - minValue);
                target.DrawLine(new Vector2(curPos, endY - satPrev * height), new Vector2(curPos + step, endY - sat * height), Brushes.White);
                curPos += step;
            }

            var satLast = (mMemorySamples.Last() - (float) minValue) / (maxValue - minValue);
            target.DrawLine(new Vector2(curPos, endY - satLast * height),
                new Vector2(curPos + ((float)diff.TotalSeconds / 0.5f) * step, endY - satLast * height), Brushes.White);

            target.PopAxisAlignedClip();

            DrawMemoryStrings(target, maxValue, minValue);
        }

        private void DrawMemoryStrings(RenderTarget target, long maxValue, long minValue)
        {
            float maxMemValue = maxValue;
            var maxSuffix = " B";
            if (maxMemValue > 1000)
            {
                maxSuffix = " KB";
                maxMemValue /= 1000.0f;
                if (maxMemValue > 1000)
                {
                    maxSuffix = " MB";
                    maxMemValue /= 1000.0f;
                    if (maxMemValue > 1000)
                    {
                        maxSuffix = " GB";
                        maxMemValue /= 1000.0f;
                    }
                }
            }

            float minMemValue = minValue;
            var minSuffix = " B";
            if(minMemValue > 1000)
            {
                minSuffix = " KB";
                minMemValue /= 1000.0f;
                if(minMemValue > 1000)
                {
                    minSuffix = " MB";
                    minMemValue /= 1000.0f;
                    if(minMemValue > 1000)
                    {
                        minSuffix = " GB";
                        minMemValue /= 1000.0f;
                    }
                }
            }

            mMemoryMax.Text = maxMemValue.ToString("F2") + maxSuffix;
            mMemoryMin.Text = minMemValue.ToString("F2") + minSuffix;

            target.DrawTextLayout(new Vector2(mPosition.X + 3.0f, mMemoryRectangle.Top - 20.0f), mMemoryMax, Brushes.White);
            target.DrawTextLayout(new Vector2(mPosition.X + 3.0f, mMemoryRectangle.Bottom + 2), mMemoryMin, Brushes.White);
        }

        private void DrawFpsSamples(RenderTarget target)
        {
            var maxValue = mFpsSamples.Max();
            var minValue = mFpsSamples.Min();
            if (MathUtil.WithinEpsilon(minValue, maxValue, 0.5f)) maxValue = minValue + 10;

            var baseOffset = 0.0f;
            var step = mSize.X / 19.0f;
            var diff = (DateTime.Now - mLastFpsSample);

            if (mFpsSamples.Count == 20)
                baseOffset = (float)diff.TotalSeconds * step;

            var curPos = mFpsRectangle.Left - baseOffset;
            var endY = mFpsRectangle.Bottom - 5;
            var height = mFpsRectangle.Height - 20;

            target.PushAxisAlignedClip(mFpsRectangle, AntialiasMode.Aliased);

            for (var i = 1; i < mFpsSamples.Count; ++i)
            {
                var sat = (mFpsSamples[i] - minValue) / (maxValue - minValue);
                var satPrev = (mFpsSamples[i - 1] - minValue) / (maxValue - minValue);
                target.DrawLine(new Vector2(curPos, endY - satPrev * height), new Vector2(curPos + step, endY - sat * height), Brushes.White);
                curPos += step;
            }

            var satLast = (mFpsSamples.Last() - minValue) / (maxValue - minValue);
            target.DrawLine(new Vector2(curPos, endY - satLast * height),
                new Vector2(curPos + (float)diff.TotalSeconds * step, endY - satLast * height), Brushes.White);

            target.PopAxisAlignedClip();

            mFpsMin.Text = minValue.ToString("F2");
            mFpsMax.Text = maxValue.ToString("F2");

            target.DrawTextLayout(new Vector2(Position.X + 3.0f, mFpsRectangle.Top - 20.0f), mFpsMax, Brushes.White);
            target.DrawTextLayout(new Vector2(Position.X + 3.0f, mFpsRectangle.Bottom + 2.0f), mFpsMin, Brushes.White);
        }

        private void ClientAreaChanged(Vector2 position, Vector2 size)
        {
            mPosition = position;
            mSize = size;

            mSize.Y = Math.Max(80, size.Y);

            var perQuadHeight = (mSize.Y - 80) / 2.0f;
            mFpsRectangle = new RectangleF(mPosition.X, mPosition.Y + 20, mSize.X, perQuadHeight);
            mMemoryRectangle = new RectangleF(mPosition.X, mPosition.Y + 65 + perQuadHeight, mSize.X, perQuadHeight);

            mMemoryMin.Size = mMemoryMax.Size = mFpsMin.Size = mFpsMax.Size = mFpsCaption.Size = mMemoryCaption.Size = new Size2F(size.X, 15.0f);

        }
    }
}
