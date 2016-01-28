using System;
using SharpDX;
using WoWEditor6.Scene;
using WoWEditor6.UI;
using WintabDN;
using System.Windows;

namespace WoWEditor6.Editing
{
    class EditManager
    {
        public static EditManager Instance { get; private set; }

        private DateTime mLastChange = DateTime.Now;

        private float mInnerRadius = 45.0f;
        private float mOuterRadius = 55.0f;

        // Stuff for the pen system 
        // can be cleaned up
        private CWintabContext m_logContext = null;
        private CWintabData m_wtData = null;

        public float InnerRadius
        {
            get { return mInnerRadius; }
            set { HandleInnerRadiusChanged(value); }
        }

        public float OuterRadius
        {
            get { return mOuterRadius; }
            set { HandleOuterRadiusChanged(value); }
        }

        public bool IsTexturing { get { return (CurrentMode & EditMode.Texturing) != 0; } }

        public Vector3 MousePosition { get; set; }
        public bool IsTerrainHovered { get; set; }

        public EditMode CurrentMode { get; private set; }

        static EditManager()
        {
            Instance = new EditManager();
        }

        EditManager()
        {
            InitDataCapture();
        }

        public void UpdateChanges()
        {
            ModelSpawnManager.Instance.OnUpdate();
            ModelEditManager.Instance.Update();

            var diff = DateTime.Now - mLastChange;
            if (diff.TotalMilliseconds < (IsTexturing ? 40 : 20))
                return;

            mLastChange = DateTime.Now;
            if ((CurrentMode & EditMode.Sculpting) != 0)
                TerrainChangeManager.Instance.OnChange(diff);
            else if ((CurrentMode & EditMode.Texturing) != 0)
                TextureChangeManager.Instance.OnChange(diff);
        }

        public void EnableSculpting()
        {
            CurrentMode |= EditMode.Sculpting;
            CurrentMode &= ~EditMode.Texturing;
        }

        public void DisableSculpting()
        {
            CurrentMode &= ~EditMode.Sculpting;
        }

        public void EnableTexturing()
        {
            CurrentMode |= EditMode.Texturing;
            CurrentMode &= ~EditMode.Sculpting;
        }

        public void DisableTexturing()
        {
            CurrentMode &= ~EditMode.Texturing;
        }

        private void HandleInnerRadiusChanged(float value)
        {
            mInnerRadius = value;
            WorldFrame.Instance.UpdateBrush(mInnerRadius, mOuterRadius);
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleInnerRadiusChanged(value);
        }

        private void HandleOuterRadiusChanged(float value)
        {
            mOuterRadius = value;
            WorldFrame.Instance.UpdateBrush(mInnerRadius, mOuterRadius);
            if (EditorWindowController.Instance.TexturingModel != null)
                EditorWindowController.Instance.TexturingModel.HandleOuterRadiusChanged(value);
        }

        private void InitDataCapture()
        {
            try
            {
                CloseCurrentContext();

                m_logContext = OpenQueryDigitizerContext();

                m_wtData = new CWintabData(m_logContext);
                m_wtData.SetWTPacketEventHandler(HandlePenMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private CWintabContext OpenQueryDigitizerContext()
        {
            bool status = false;
            CWintabContext logContext = null;

            try
            {
                // Get the default digitizing context.
                // Default is to receive data events.
                logContext = CWintabInfo.GetDefaultDigitizingContext(ECTXOptionValues.CXO_MESSAGES);

                // Set system cursor if caller wants it.
                logContext.Options |= (uint)ECTXOptionValues.CXO_SYSTEM;

                if (logContext == null)
                {
                    return null;
                }

                // Modify the digitizing region.
                logContext.Name = "WintabDN Event Data Context";

                // output in a 10000 x 10000 grid
                logContext.OutOrgX = logContext.OutOrgY = 0;
                logContext.OutExtX = 10000;
                logContext.OutExtY = 10000;


                // Open the context, which will also tell Wintab to send data packets.
                status = logContext.Open();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return logContext;
        }

        public void HandlePenMessage(Object sender_I, MessageReceivedEventArgs eventArgs_I)
        {
            if (m_wtData == null)
            {
                return;
            }

            try
            {
                uint pktID = (uint)eventArgs_I.Message.WParam;
                WintabPacket pkt = m_wtData.GetDataPacket(pktID);

                if (pkt.pkContext != 0)
                {
                    // Normalize data between 0 and 40
                    float normalizedValue = (40f - 0f) / ((float)CWintabInfo.GetMaxPressure() - 0f) * ((float)pkt.pkNormalPressure.pkAbsoluteNormalPressure - (float)CWintabInfo.GetMaxPressure()) + 40f;
                    TextureChangeManager.Instance.Amount = normalizedValue;
                    TerrainChangeManager.Instance.Amount = normalizedValue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void CloseCurrentContext()
        {
            try
            {
                //TraceMsg("Closing context...\n");
                if (m_logContext != null)
                {
                    m_logContext.Close();
                    m_logContext = null;
                    m_wtData = null;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
