// Based on the https://github.com/UmbraSpaceIndustries/Konstruction/tree/master/Source/Konstruction/Konstruction/Welding
// GPLV3

namespace Kaboom
{
    public class WeldingData
    {
        public Part LinkedPartA { get; set; }
        public Part LinkedPartB { get; set; }
        public Part KaboomGluedPart { get; set; }
        public Part DockingPortA { get; set; }
        public Part DockingPortB { get; set; }
    }
}