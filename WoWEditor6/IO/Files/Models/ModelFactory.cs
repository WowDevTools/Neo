using System;

namespace WoWEditor6.IO.Files.Models
{
    class ModelFactory
    {
        public static ModelFactory Instance { get; } = new ModelFactory();

        public FileDataVersion Version { get; set; }

        public IM2Animator CreateAnimator(M2File file)
        {
            var wodModel = file as WoD.M2File;
            if (wodModel != null)
                return new WoD.M2Animator(wodModel);

            throw new NotImplementedException("Version not yet implemented for m2 animations");
        }

        public M2File CreateM2(string file)
        {
            switch(Version)
            {
                case FileDataVersion.Warlords:
                    return new WoD.M2File(file);

                default:
                    throw new NotImplementedException("Version not yet implemented for m2 models");
            }
        }

        public WmoRoot CreateWmo()
        {
            switch(Version)
            {
                case FileDataVersion.Warlords:
                    return new WoD.WmoRoot();

                default:
                    throw new NotImplementedException("Version not yet supported for WMO models");
            }
        }
    }
}
