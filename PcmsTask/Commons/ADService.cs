using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;

namespace PcmsTask.Commons
{
    public class ADService
    {
        public static bool IsAuthGroup(string ntid)
        {
            AppSettingsReader appSettingsReader = new AppSettingsReader();
            string adgroupname = (string)appSettingsReader.GetValue("ADGroupName", typeof(string));
            
            if (string.IsNullOrEmpty(adgroupname)) return true;
            
            DirectoryEntry vRoot = new DirectoryEntry();
            DirectorySearcher search = new DirectorySearcher(vRoot) {
                SearchScope = SearchScope.Subtree ,
                Filter = "(samaccountname=" + ntid.ToUpper() + ")"
            };
            search.PropertiesToLoad.Add("memberof");
            SearchResult result = search.FindOne();

            List<string> fieldList = new List<string>();
            if (result != null)
            {
                int propertyCount = result.Properties["memberof"].Count;
                int equalsIndex, commaIndex;
                for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
                {
                    string v = (string)result.Properties["memberof"][propertyCounter];
                    equalsIndex = v.IndexOf("CN=") + 2;
                    commaIndex = v.IndexOf(",", equalsIndex) -1;
                    string groupname = v.Substring(equalsIndex + 1, commaIndex - equalsIndex);
                    if (groupname == adgroupname) return true;
                }
            }
            return false;            
        }
    }
}