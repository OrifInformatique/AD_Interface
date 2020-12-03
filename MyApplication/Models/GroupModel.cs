using System.DirectoryServices;

namespace WebApplication1.Models
{
    public class GroupModel
    {
        public DirectoryEntry Group;

        public GroupModel(DirectoryEntry group)
        {
            Group = group;
        }

        public string PropertyToString(string property)
        {
            string result = string.Empty;
            foreach (var obj in Group.Properties[property])
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += ", ";
                }
                result += obj.ToString();
            }
            return result;
        }
    }
}