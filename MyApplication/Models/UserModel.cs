using ActiveDs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using WebApplication1.Properties;

namespace WebApplication1.Models
{
    public class UserModel
    {
        public DirectoryEntry User;

        public UserModel(DirectoryEntry user)
        {
            User = user;
        }

        public string PropertyToString(string property)
        {
            string result = string.Empty;
            foreach (var obj in User.Properties[property])
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += ", ";
                }
                result += obj.ToString();
            }
            return result;
        }

        public bool IsActive()
        {
            if (User.Properties.Contains("userAccountControl"))
            {
                return !Convert.ToBoolean((int)User.Properties["userAccountControl"].Value & 0x0002);
            }
            else
            {
                return true;
            }
        }

        public string GetAccountExpires(string format = "")
        {
            IADsLargeInteger lgInt = (IADsLargeInteger)User.Properties["accountExpires"].Value;
            long expires = ((long)lgInt.HighPart << 32) + lgInt.LowPart;
            // the values 0 and (2^63 - 1) mean "Never"
            if (expires == 0 || expires == 9223372032559808511)
            {
                return "Never";
            }
            else
            {
                try
                {
                    return DateTime.FromFileTime(expires).ToString(format);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return "error";
                }
            }
        }

        public UserModel GetManager()
        {
            if (!string.IsNullOrWhiteSpace(PropertyToString("manager")))
            {
                DirectorySearcher adSearcher = new DirectorySearcher(new DirectoryEntry("LDAP://" + Settings.Default.ADPath));
                adSearcher.Filter = ("(&(distinguishedName=" + PropertyToString("manager") + "))");
                SearchResult result = adSearcher.FindOne();
                if (result != null)
                {
                    return new UserModel(result.GetDirectoryEntry());
                }
            }
            return null;
        }

        public UserModel[] GetDirectReports()
        {
            List<UserModel> reports = new List<UserModel>();
            foreach (string property in User.Properties["DirectReports"])
            {
                DirectorySearcher adSearcher = new DirectorySearcher(new DirectoryEntry("LDAP://" + Settings.Default.ADPath));
                adSearcher.Filter = ("(&(distinguishedName=" + property + "))");
                SearchResult result = adSearcher.FindOne();
                if (result != null)
                {
                    reports.Add(new UserModel(result.GetDirectoryEntry()));
                }
            }
            return reports.ToArray();
        }

        public GroupModel[] GetGroups()
        {
            List<GroupModel> groups = new List<GroupModel>();
            object obGroups = User.Invoke("Groups");
            foreach (object ob in (IEnumerable)obGroups)
            {
                DirectoryEntry obGpEntry = new DirectoryEntry(ob);
                groups.Add(new GroupModel(obGpEntry));
            }
            return groups.ToArray();
        }
    }
}