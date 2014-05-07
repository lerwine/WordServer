using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Erwine.Leonard.T.WordServer
{
    public class Global : System.Web.HttpApplication
    {
        public WordLookupManager CurrentWordLookupManager { get; private set; }

        public bool UseConnectedDatabase
        {
            get
            {
                bool result;
                return (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["UseConnectedDatabase"]) &&
                    Boolean.TryParse(ConfigurationManager.AppSettings["UseConnectedDatabase"], out result) && result);
            }
        }

        public string WordNetDbEntitiesConnectionString
        {
            get
            {
                return (ConfigurationManager.ConnectionStrings["UseConnectedDatabase"] == null ||
                    String.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings["UseConnectedDatabase"].ConnectionString)) ? null :
                    ConfigurationManager.ConnectionStrings["UseConnectedDatabase"].ConnectionString;
            }
        }
        
        protected void Application_Start(object sender, EventArgs e)
        {
            this.CurrentWordLookupManager = new WordLookupManager();
            if (!this.UseConnectedDatabase || this.WordNetDbEntitiesConnectionString == null)
                WordLookupManager.InitializeStaticDb();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}