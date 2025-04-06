using Stride.BepuPhysics.Definitions.Contacts;

namespace LD57
{
    public class BepuTrigger : StartupScript, IContactEventHandler
    {
        public bool NoContactResponse => true;
    }
}
