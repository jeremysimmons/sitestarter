using System;
using System.Configuration.Provider;
using System.Security.Permissions;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Web.Providers
{
    public class DynamicPersonalizationProvider : PersonalizationProvider
    {
        public override string ApplicationName
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override void Initialize(string name,
            NameValueCollection config)
        {
            // Verify that config isn't null
            if (config == null)
                throw new ArgumentNullException("config");

            // Assign the provider a default name if it doesn't have one
            if (String.IsNullOrEmpty(name))
                name = "DynamicPersonalizationProvider";

            // Add a default "description" attribute to config if the
            // attribute doesn't exist or is empty
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description",
                    "Text file personalization provider");
            }

            // Call the base class's Initialize method
            base.Initialize(name, config);

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException
                        ("Unrecognized attribute: " + attr);
            }

            string directory = HttpContext.Current.Server.MapPath("~/App_Data/Personalization_Data");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Make sure we can read and write files in the
            // ~/App_Data/Personalization_Data directory
            FileIOPermission permission = new FileIOPermission
                (FileIOPermissionAccess.AllAccess,
                directory);
            permission.Demand();
        }

        protected override void LoadPersonalizationBlobs
            (WebPartManager webPartManager, string path, string userName,
            ref byte[] sharedDataBlob, ref byte[] userDataBlob)
        {
            // Load shared state
            StreamReader reader1 = null;
            sharedDataBlob = null;

            try
            {
                reader1 = new StreamReader(GetPath(null, path));
                sharedDataBlob =
                    Convert.FromBase64String(reader1.ReadLine());
            }
            catch (FileNotFoundException)
            {
                // Not an error if file doesn't exist
            }
            finally
            {
                if (reader1 != null)
                    reader1.Close();
            }

            // Load private state if userName holds a user name
            if (!String.IsNullOrEmpty(userName))
            {
                StreamReader reader2 = null;
                userDataBlob = null;

                try
                {
                    reader2 = new StreamReader(GetPath(userName, path));
                    userDataBlob =
                        Convert.FromBase64String(reader2.ReadLine());
                }
                catch (FileNotFoundException)
                {
                    // Not an error if file doesn't exist
                }
                finally
                {
                    if (reader2 != null)
                        reader2.Close();
                }
            }
        }

        protected override void ResetPersonalizationBlob
            (WebPartManager webPartManager, string path, string userName)
        {
            // Delete the specified personalization file
            try
            {
                File.Delete(GetPath(userName, path));
            }
            catch (FileNotFoundException) { }
        }

        protected override void SavePersonalizationBlob
            (WebPartManager webPartManager, string path, string userName,
            byte[] dataBlob)
        {
            StreamWriter writer = null;

            try
            {
                writer = new StreamWriter(GetPath(userName, path), false);
                writer.WriteLine(Convert.ToBase64String(dataBlob));
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public override PersonalizationStateInfoCollection FindState
            (PersonalizationScope scope, PersonalizationStateQuery query,
            int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        public override int GetCountOfState(PersonalizationScope scope,
            PersonalizationStateQuery query)
        {
            throw new NotSupportedException();
        }

        public override int ResetState(PersonalizationScope scope,
            string[] paths, string[] usernames)
        {
            throw new NotSupportedException();
        }

        public override int ResetUserState(string path,
            DateTime userInactiveSinceDate)
        {
            throw new NotSupportedException();
        }

        private string GetPath(string userName, string path)
        {
            SHA1CryptoServiceProvider sha =
                new SHA1CryptoServiceProvider();
            UnicodeEncoding encoding = new UnicodeEncoding();
            string hash = Convert.ToBase64String(sha.ComputeHash
                (encoding.GetBytes(path))).Replace('/', '_');

            if (String.IsNullOrEmpty(userName))
                return HttpContext.Current.Server.MapPath
    (String.Format("~/App_Data/Personalization_Data/{0}_Personalization.txt",
                    hash));
            else
            {
                // NOTE: Consider validating the user name here to prevent
                // malicious user names such as "../Foo" from targeting
                // directories other than ~/App_Data/Personalization_Data

                return HttpContext.Current.Server.MapPath
    (String.Format("~/App_Data/Personalization_Data/{0}_{1}_Personalization.txt",
                    userName.Replace('\\', '_'), hash));
            }
        }
    }
}