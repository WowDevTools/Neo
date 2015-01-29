using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace WoWEditor6.Settings
{
    public class Camera
    {
        public Keys[] Forward = {Keys.W};
        public Keys[] Backward = {Keys.S};
        public Keys[] Left = {Keys.A};
        public Keys[] Right = {Keys.D};
        public Keys[] Up = {Keys.Q};
        public Keys[] Down = {Keys.E};
    }

    public class KeyBindings
    {
        [XmlIgnore]
        public static KeyBindings Instance { get; private set; }

        public Camera Camera { get; set; } = new Camera();

        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(@".\Config");
                using (var strm = File.Open(@".\Config\Bindings.xml", FileMode.Create, FileAccess.Write))
                {
                    var serializer = new XmlSerializer(typeof(KeyBindings));
                    serializer.Serialize(strm, Instance);
                }
            }
            catch (Exception e)
            {
                Log.Warning("Unable to save Bindings.xml: " + e.Message);
            }
        }

        public static void Initialize()
        {
            try
            {
                using (var strm = File.OpenRead(@".\Config\Bindings.xml"))
                {
                    var serializer = new XmlSerializer(typeof (KeyBindings));
                    Instance = (KeyBindings) serializer.Deserialize(strm);
                }
            }
            catch(Exception)
            {
                Instance = new KeyBindings();
                try
                {
                    Directory.CreateDirectory(@".\Config");
                    using (var strm = File.Open(@".\Config\Bindings.xml", FileMode.Create, FileAccess.Write))
                    {
                        var serializer = new XmlSerializer(typeof (KeyBindings));
                        serializer.Serialize(strm, Instance);
                    }
                }
                catch(Exception e)
                {
                    Log.Warning("Unable to save Bindings.xml: " + e.Message);
                }
            }
        }
    }
}
