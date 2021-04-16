using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Properties;

namespace WebApplication1.Controllers
{
    public class InfoController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("~/");
        }

        /// <summary>
        /// Looks up an user by either username (username) or names (first name and last name)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="findBy"></param>
        /// <returns></returns>
        public ActionResult FindUser(string username, string firstname, string lastname, string findBy = "username")
        {
            List<UserModel> users = new List<UserModel>();
            if ((findBy == "username" && !string.IsNullOrWhiteSpace(username))
            || (findBy == "names" && (!string.IsNullOrWhiteSpace(firstname) || !string.IsNullOrWhiteSpace(lastname))))
            {
                DirectorySearcher adSearcher = new DirectorySearcher(new DirectoryEntry("LDAP://" + Settings.Default.ADPath));
                adSearcher.PageSize = int.MaxValue;
                if (findBy == "username")
                {
                    adSearcher.Filter = "(&(samAccountName=" + username + ")(objectCategory=person))";
                }
                else if (findBy == "names")
                {
                    adSearcher.Filter = "(&";
                    if (!string.IsNullOrWhiteSpace(firstname))
                    {
                        adSearcher.Filter += "(givenName=" + firstname + ")";
                    }
                    if (!string.IsNullOrWhiteSpace(lastname))
                    {
                        adSearcher.Filter += "(sn=" + lastname + ")";
                    }
                    adSearcher.Filter += "(objectCategory=person))";
                }
                try
                {
                    SearchResultCollection coll = adSearcher.FindAll();
                    foreach (SearchResult item in coll)
                    {
                        users.Add(new UserModel(item.GetDirectoryEntry()));
                    }
                }
                catch (System.ArgumentException)
                {
                }
            }
            if (findBy != "username" && findBy != "names")
            {
                findBy = "username";
            }
            ViewBag.FindBy = findBy;
            ViewBag.Users = users;
            return View();
        }

        /// <summary>
        /// Displays the details of an user by their samaccountname
        /// </summary>
        /// <param name="id">The samaccountname of the user</param>
        /// <returns></returns>
        public ActionResult DetailUser(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                DirectorySearcher adSearcher = new DirectorySearcher(new DirectoryEntry("LDAP://" + Settings.Default.ADPath));
                adSearcher.Filter = "(&(samAccountName=" + id + ")(objectCategory=person))";
                try
                {
                    GetUserProperties(id);
                    SearchResult result = adSearcher.FindOne();
                    if (result != null)
                    {
                        ViewBag.User = new UserModel(result.GetDirectoryEntry());
                        return View();
                    }
                }
                catch (System.ArgumentException)
                {
                }
            }
            return RedirectToAction("FindUser");
        }

        /// <summary>
        /// Displays the possible groups for a groupname searched
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public ActionResult FindGroup(string group)
        {
            List<GroupModel> groups = new List<GroupModel>();
            if (!string.IsNullOrWhiteSpace(group))
            {
                DirectorySearcher adSearcher = new DirectorySearcher(new DirectoryEntry("LDAP://" + Settings.Default.ADPath));
                adSearcher.PageSize = int.MaxValue;
                adSearcher.Filter = "(&(samAccountName=" + group + ")(objectCategory=group))";
                try
                {
                    SearchResultCollection coll = adSearcher.FindAll();
                    foreach (SearchResult item in coll)
                    {
                        groups.Add(new GroupModel(item.GetDirectoryEntry()));
                    }
                }
                catch (System.ArgumentException)
                {
                }
            }
            ViewBag.Groups = groups;
            return View();
        }

        /// <summary>
        /// Displays the list of users in a group
        /// </summary>
        /// <param name="id"></param>
        /// <param name="id2">"1" if the sub groups are to display too, "0" otherwise</param>
        /// <returns></returns>
        public ActionResult DetailGroup(string id, string id2 = "0")
        {
            if (!string.IsNullOrEmpty(id))
            {
                GroupPrincipal group = GroupPrincipal.FindByIdentity(new PrincipalContext(ContextType.Domain, Settings.Default.ADPath), id);
                if (group != null)
                {
                    List<UserModel> users = new List<UserModel>();
                    List<GroupModel> groups = new List<GroupModel>();

                    var members = group.GetMembers(id2 != "0");
                    foreach (var member in members)
                    {
                        if (member is UserPrincipal)
                        {
                            users.Add(new UserModel(member.GetUnderlyingObject() as DirectoryEntry));
                        }
                    }

                    var subgroups = group.GetMembers();
                    foreach (var subgroup in subgroups)
                    {
                        if (subgroup is GroupPrincipal)
                        {
                            groups.Add(new GroupModel(subgroup.GetUnderlyingObject() as DirectoryEntry));
                        }
                    }

                    ViewBag.Group = new GroupModel(group.GetUnderlyingObject() as DirectoryEntry);
                    ViewBag.Users = users.ToArray();
                    ViewBag.Groups = groups.ToArray();
                    ViewBag.Recursive = id2 != "0";
                    return View();
                }
            }
            return RedirectToAction("FindGroup");
        }

        /// <summary>
        /// Displays subgroups and is called by ajax js
        /// </summary>
        /// <param name="samAccountName"></param>
        /// <returns></returns>
        public ActionResult AjaxSubGroups(string samAccountName)
        {
            List<GroupModel> groups = new List<GroupModel>();
            if (!string.IsNullOrWhiteSpace(samAccountName))
            {
                DirectorySearcher adSearcher = new DirectorySearcher(new DirectoryEntry("LDAP://" + Settings.Default.ADPath));
                adSearcher.PageSize = int.MaxValue;
                adSearcher.Filter = "(&(samAccountName=" + samAccountName + ")(objectCategory=group))";
                SearchResult result = adSearcher.FindOne();
                if (result != null)
                {
                    DirectoryEntry subGroups = result.GetDirectoryEntry();
                    for (int i = 0; i < subGroups.Properties["memberOf"].Count; i++)
                    {
                        DirectorySearcher adSearcher2 = new DirectorySearcher(new DirectoryEntry("LDAP://" + Settings.Default.ADPath));
                        adSearcher2.Filter = "(&(distinguishedName=" + subGroups.Properties["memberOf"][i] + "))";
                        SearchResult result2 = adSearcher2.FindOne();
                        if (result2 != null)
                        {
                            groups.Add(new GroupModel(result2.GetDirectoryEntry()));
                        }
                    }
                }
            }
            ViewBag.groups = groups.ToArray();
            return View();
        }

        public string GetUsername()
        {
            var user = System.Web.HttpContext.Current.User;
            var name = user.Identity.Name;

            var slashIndex = name.IndexOf("\\");
            return slashIndex > -1
                ? name.Substring(slashIndex + 1)
                : name.Substring(0, name.IndexOf("@"));
        }

        public string GetProperty(SearchResult searchResult, string PropertyName)
        {
            if (searchResult.Properties.Contains(PropertyName))
            {
                return searchResult.Properties[PropertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetUserProperties(string username)
        {
            string property = "";
            DirectorySearcher adSearcher = new DirectorySearcher(new DirectoryEntry("LDAP://" + Settings.Default.ADPath));
            adSearcher.Filter = "(&(samAccountName=" + username + ")(objectCategory=person))";

            foreach (SearchResult searchResult in adSearcher.FindAll())
            {
                property = GetProperty(searchResult, "comment");
            }

            return property;
        }

        public bool CheckUserRight(string username)
        {
            string canAccess = GetUserProperties(username);
            if(canAccess != "AD_Interface_admin")
            {
                return false;
            } else
            {
                return true;
            }
        }
    }
}
