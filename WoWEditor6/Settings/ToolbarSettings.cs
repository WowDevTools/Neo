using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace WoWEditor6.Settings
{
    [Serializable]
    public class ToolbarSettings
    {
        [XmlIgnore]
        public static ToolbarSettings Settings { get; private set; }

        public ToolbarButtons Top { get; set; }
        public ToolbarButtons Left { get; set; }

        public static void Load()
        {
            /*if (File.Exists(".\\Config\\Toolbars.xml"))
            {
                Stream strm = null;
                try
                {
                    strm = File.OpenRead(".\\Config\\Toolbars.xml");
                    var serializer = new XmlSerializer(typeof (ToolbarSettings));

                    Settings = (ToolbarSettings) serializer.Deserialize(strm);
                    return;
                }
                catch (Exception e)
                {
                    Log.Warning("Unable to load Toolbars.xml: " + e.Message);
                }
                finally
                {
                    strm?.Dispose();
                }

                CreateDefault();
            }
            else*/
                CreateDefault();
        }

        private static void CreateDefault()
        {
            Settings = new ToolbarSettings
            {
                Top = new ToolbarButtons
                {
                    Buttons = new List<ToolbarButton>
                    {
                        new ToolbarButton {Function = ToolbarFunction.Terrain, Tooltip = "Switch to terrain editing mode" },
                        new ToolbarButton {Function = ToolbarFunction.KeyBinding, Tooltip = "Open keyboard/mouse settings dialog" },
						new ToolbarButton {Function = ToolbarFunction.Save, Tooltip = "Save all pending changes" }
                    }
                },

                Left = new ToolbarButtons
                {
                    Buttons = new List<ToolbarButton>()
                }
            };

            var serializer = new XmlSerializer(typeof (ToolbarSettings));
            Stream strm = null;
            try
            {
                Directory.CreateDirectory(".\\Config");
                strm = File.Open(".\\Config\\Toolbars.xml", FileMode.Create, FileAccess.Write, FileShare.None);
                serializer.Serialize(strm, Settings);
            }
            catch(Exception e)
            {
                Log.Warning("Unable to save Toolbars.xml: " + e.Message);
            }
            finally
            {
                strm?.Dispose();
            }
        }
    }
}
