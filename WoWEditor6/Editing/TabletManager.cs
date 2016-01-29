using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WintabDN;

namespace WoWEditor6.Editing
{
    class TabletManager
    {
        /// <summary>
        /// A value between 0 and 40 dependent on how hard you are pressing the pen down
        /// </summary>
        public float TabletPressure { get; private set; }
        /// <summary>
        /// returns true if there is a tablet connected
        /// </summary>
        public bool IsConnected { get; private set; }

        private CWintabContext m_logContext = null;
        private CWintabData m_wtData = null;

        public static TabletManager Instance { get; private set; }

        static TabletManager()
        {
            Instance = new TabletManager();
        }
        private TabletManager()
        {
            TryConnect();
        }

        public void TryConnect()
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
                Log.Fatal(ex.ToString());
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

                // Set system cursor
                logContext.Options |= (uint)ECTXOptionValues.CXO_SYSTEM;

                if (logContext == null)
                    return null;

                // Modify the digitizing region.
                logContext.Name = "WintabDN Event Data Context";

                // output in a 10000 x 10000 grid
                logContext.OutOrgX = logContext.OutOrgY = 0;
                logContext.OutExtX = 10000;
                logContext.OutExtY = 10000;


                // Open the context, which will also tell Wintab to send data packets.
                status = logContext.Open();

                // set IsConnected to the status of the tablet true = tablet ready false = tablet not found / tablet not supported
                IsConnected = status;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return logContext;
        }

        private void CloseCurrentContext()
        {
            try
            {
                if (m_logContext != null)
                {
                    m_logContext.Close();
                    m_logContext = null;
                    m_wtData = null;
                }

            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
            }
        }

        private void HandlePenMessage(Object sender_I, MessageReceivedEventArgs eventArgs_I)
        {
            if (m_wtData == null)
                return;

            try
            {
                uint pktID = (uint)eventArgs_I.Message.WParam;
                WintabPacket pkt = m_wtData.GetDataPacket(pktID);

                if (pkt.pkContext != 0)
                {
                    // Normalize data between 0 and 40
                    float normalizedValue = (40f - 0f) / ((float)CWintabInfo.GetMaxPressure() - 0f) * ((float)pkt.pkNormalPressure.pkAbsoluteNormalPressure - (float)CWintabInfo.GetMaxPressure()) + 40f;
                    TabletPressure = normalizedValue;
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
            }
        }
    }
}
