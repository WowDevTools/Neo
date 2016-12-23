using System;
using System.IO;
using System.Xml.Serialization;
using OpenTK.Input;

namespace Neo.Settings
{
    public class CameraKeys
    {
        public Key[] Forward =
        {
	        Key.W
        };

        public Key[] Backward =
        {
	        Key.S
        };

        public Key[] Left =
        {
	        Key.A
        };

        public Key[] Right =
        {
	        Key.D
        };

        public Key[] Up =
        {
	        Key.Q
        };

        public Key[] Down =
        {
	        Key.E
        };
    }

    public class InteractionKeys
    {
        public Key[] Edit =
        {
	        Key.ShiftLeft,
	        Key.ShiftRight
        };

        public Key[] EditInverse =
        {
	        Key.ControlLeft,
	        Key.ControlRight
        };
    }

    public class KeyBindings
    {
        [XmlIgnore]
        public static KeyBindings Instance { get; private set; }

        public CameraKeys CameraKeys { get; private set; }
        public InteractionKeys InteractionKeys { get; private set; }

	    private KeyBindings()
        {
	        this.CameraKeys = new CameraKeys();
	        this.InteractionKeys = new InteractionKeys();
        }

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

                Save();
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
