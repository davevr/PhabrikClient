

/**
 * Created by davidvronay on 8/23/16.
 */
namespace Phabrik.Core
{
    public class StructureTypeObj
    {
        public long Id { get; set; }
        public string structuretype { get; set; }
        public string structurename { get; set; }
        public string description { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public bool ispublic { get; set; }


        public string imageURL
        {
            get
            {
                string url = "structures/";
                url += System.Uri.EscapeUriString(structurename);
                url = PhabrikServer.BaseImageUrl + url + ".png";
                return url;
            }
        }
    }
}
