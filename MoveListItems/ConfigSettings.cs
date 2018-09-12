using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveListItems
{
    public class ConfigSettings
    {
        public static readonly string SourceSiteUrl = Convert.ToString(ConfigurationManager.AppSettings[Constant.CONST_SourceSiteUrl]);
        public static readonly string DestinationSiteUrl = Convert.ToString(ConfigurationManager.AppSettings[Constant.CONST_DestinationSiteUrl]);
        public static readonly string SourceUserName = Convert.ToString(ConfigurationManager.AppSettings[Constant.CONST_SourceUserName]);
        public static readonly string SourcePassword = Convert.ToString(ConfigurationManager.AppSettings[Constant.CONST_SourcePassword]);
        public static readonly string DestinationUserName = Convert.ToString(ConfigurationManager.AppSettings[Constant.CONST_DestinationUserName]);
        public static readonly string DestinationPassword = Convert.ToString(ConfigurationManager.AppSettings[Constant.CONST_DestinationPassword]);
        public static readonly string SourceLibrary = Convert.ToString(ConfigurationManager.AppSettings[Constant.CONST_SourceLibrary]);
        public static readonly string DestinationLibrary = Convert.ToString(ConfigurationManager.AppSettings[Constant.CONST_DestinationLibrary]);
    }
}
