using System.Collections.Generic;
using SharpDX.Direct2D1;
using Neo.UI.Components;

namespace Neo.UI
{
    class ComponentRoot : IComponent
    {
        private readonly List<IComponent> mElements = new List<IComponent>();

        public void AddComponent(IComponent component)
        {
            lock (mElements) mElements.Add(component);
        }

        public void RemoveComponent(IComponent component)
        {
            lock (mElements) mElements.Remove(component);
        }

        public void OnRender(RenderTarget target)
        {
            lock(mElements)
            {
                foreach (var component in mElements)
                    component.OnRender(target);
            }
        }

        public void OnMessage(Message message)
        {
            lock(mElements)
            {
                for (var i = mElements.Count - 1; i >= 0; --i)
                    mElements[i].OnMessage(message);
            }
        }
    }
}
