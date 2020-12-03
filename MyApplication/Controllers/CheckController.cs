using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Properties;

namespace WebApplication1.Controllers
{
    public class CheckController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays a list of users that are not part of both the `SAIUSER` AD group and not in any of the GGDAC groups
        /// </summary>
        /// <returns></returns>
        public ActionResult SAIGroup()
        {
            List<Principal> users = new List<Principal>();
            GroupPrincipal saiUserGroup = GroupPrincipal.FindByIdentity(new PrincipalContext(ContextType.Domain, Settings.Default.ADPath), "SAIUSER");

            string[] saiGroups = new string[]
            {
                "GGDAC-10-SaiLecteur",
                "GGDAC-20-SaiMSPEDU",
                "GGDAC-50-SaiAdministration",
                "GGDAC-70-SaiDirection",
                "GGDAC-80-SaiCoordinateurs",
                "GGDAC-90-SaiCoordinateurGlobal"
            };
            foreach (string saiGroup in saiGroups)
            {
                GroupPrincipal group = GroupPrincipal.FindByIdentity(new PrincipalContext(ContextType.Domain, Settings.Default.ADPath), saiGroup);
                if (group is null)
                {
                    continue;
                }

                foreach (Principal member in group.Members)
                {
                    if (member is GroupPrincipal && !member.IsMemberOf(saiUserGroup))
                    {
                        users.Add(member);
                    }
                }
                foreach (Principal member in saiUserGroup.Members)
                {
                    if (member is GroupPrincipal && !member.IsMemberOf(group))
                    {
                        users.Add(member);
                    }
                }
            }
            ViewBag.Users = users.ToArray();
            return View();
        }

        /// <summary>
        /// Displays a list of users that share the same phone number
        /// </summary>
        /// <returns></returns>
        public ActionResult DuplicatePhone()
        {
            Dictionary<string, UserModel> phones = new Dictionary<string, UserModel>();
            SortedDictionary<string, List<UserModel>> users = new SortedDictionary<string, List<UserModel>>();
            DirectorySearcher adSearcher = new DirectorySearcher(new DirectoryEntry("LDAP://" + Settings.Default.ADPath));
            adSearcher.PageSize = int.MaxValue;
            adSearcher.Filter = "(&(telephoneNumber=*))";
            SearchResultCollection coll = adSearcher.FindAll();
            foreach (SearchResult item in coll)
            {
                UserModel currentUser = new UserModel(item.GetDirectoryEntry());
                string phoneNumber = currentUser.PropertyToString("telephoneNumber");
                if (phones.ContainsKey(phoneNumber))
                {
                    if (users.ContainsKey(phoneNumber))
                    {
                        users[phoneNumber].Add(currentUser);
                    }
                    else
                    {
                        users.Add(phoneNumber, new List<UserModel>() { phones[phoneNumber], currentUser });
                    }
                }
                else
                {
                    phones.Add(phoneNumber, currentUser);
                }
            }
            ViewBag.Users = users;
            return View();
        }

        /// <summary>
        /// Displays a list of groups that do not have a description
        /// </summary>
        /// <returns></returns>
        public ActionResult GroupsDescription()
        {
            List<GroupModel> groups = new List<GroupModel>();
            List<ErrorModel> errors = new List<ErrorModel>();
            DirectorySearcher adSearcher = new DirectorySearcher(new DirectoryEntry("LDAP://" + Settings.Default.ADPath));
            adSearcher.PageSize = int.MaxValue;
            adSearcher.Filter = "(&(!description=*)(objectCategory=group))";
            SearchResultCollection coll = adSearcher.FindAll();
            foreach (SearchResult item in coll)
            {
                GroupModel group = new GroupModel(item.GetDirectoryEntry());
                groups.Add(group);
                ErrorModel error = new ErrorModel(group.PropertyToString("samAccountName"), "", "description", "", "", "Not empty", "");
                errors.Add(error);
                error.InsertOrUpdate();
            }
            ViewBag.Groups = groups;
            return View();
        }

        public ActionResult UsersAddress()
        {
            string[] properties = new string[] { "streetAddress", "postalCode", "l", "st", "co" };
            Dictionary<string, Dictionary<string, string>> listAddress = new Dictionary<string, Dictionary<string, string>>()
            {
                { "Orif Pomy", new Dictionary<string, string>() {
                    { "streetAddress", "Ch. du Mont-de-Brez 2" },
                    { "postalCode", "1405" },
                    { "l", "Pomy" },
                    { "st", "VD" },
                    { "co", "Switzerland" }
                } },
                { "Orif Aigle", new Dictionary<string, string>() {
                    { "streetAddress", "Chemin de Pré Yonnet" },
                    { "postalCode", "1860" },
                    { "l", "Aigle" },
                    { "st", "VD" },
                    { "co", "Switzerland" }
                }}
            };
            List<ErrorModel> errors = new List<ErrorModel>();
            DirectorySearcher adSearcher = new DirectorySearcher(new DirectoryEntry("LDAP://" + Settings.Default.ADPath));
            adSearcher.PageSize = int.MaxValue;
            adSearcher.Filter = "(&(company=*)(objectCategory=person))";
            SearchResultCollection coll = adSearcher.FindAll();
            foreach (SearchResult item in coll)
            {
                UserModel user = new UserModel(item.GetDirectoryEntry());
                string company = user.PropertyToString("company");
                if (listAddress.ContainsKey(company))
                {
                    Dictionary<string, string> expectedAdress = listAddress[company];
                    foreach (string property in properties)
                    {
                        if (user.PropertyToString(property) != expectedAdress[property])
                        {
                            ErrorModel error = new ErrorModel(user.PropertyToString("samAccountName"), user.PropertyToString("samAccountName"), property, "company", user.PropertyToString(property), expectedAdress[property], company);
                            errors.Add(error);
                            error.InsertOrUpdate();
                        }
                    }
                }
            }
            ViewBag.Errors = errors;
            return View();
        }
    }
}
