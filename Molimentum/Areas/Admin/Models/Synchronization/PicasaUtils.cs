using System.Globalization;
using Google.GData.Photos;

namespace Molimentum.Areas.Admin.Models.Synchronization
{
    public static class PicasaUtils
    {
        public static string GetEtag(this PicasaEntry entry)
        {
            // workaround - picasa query does not return etags
            return entry.Updated.ToUniversalTime().ToString(DateTimeFormatInfo.InvariantInfo);
        }
    }
}
