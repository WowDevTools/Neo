using System;

namespace Neo.IO.Files.Models
{
    class ModelFactory
    {
        public static ModelFactory Instance { get; private set; }

        public FileDataVersion Version { get; set; }

        static ModelFactory()
        {
            Instance = new ModelFactory();
        }

        public IM2Animator CreateAnimator(M2File file)
        {
            var wodModel = file as WoD.M2File;
            if (wodModel != null)
                return new WoD.M2Animator(wodModel);

            var wotlkModel = file as Wotlk.M2File;
            if (wotlkModel != null)
                return new Wotlk.M2Animator(wotlkModel);

            throw new NotImplementedException("Version not yet implemented for m2 animations");
        }

        public M2File CreateM2(string file)
        {
            switch(Version)
            {
                case FileDataVersion.Warlords:
                    return new WoD.M2File(file);

                case FileDataVersion.Lichking:
                    return new Wotlk.M2File(file);

                default:
                    throw new NotImplementedException("Version not yet implemented for m2 models");
            }
        }

        public IWorldModelRoot CreateWmo()
        {
            switch(Version)
            {
                case FileDataVersion.Warlords:
                    return new WoD.WorldModelRoot();

                case FileDataVersion.Lichking:
                    return new Wotlk.WorldModelRoot();

                default:
                    throw new NotImplementedException("Version not yet supported for WMO models");
            }
        }
    }
}
