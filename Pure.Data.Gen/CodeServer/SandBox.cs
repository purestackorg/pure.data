using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor;

namespace Pure.Data.Gen.CodeServer.RazorPaser
{
    public class SandBox
    {
        public static AppDomain SandboxCreator()
        {
            Evidence ev = new Evidence();
            ev.AddHostEvidence(new Zone(SecurityZone.Internet));
            PermissionSet permSet = SecurityManager.GetStandardSandbox(ev);
            // We have to load ourself with full trust
            StrongName razorEngineAssembly = typeof(RazorEngineService).Assembly.Evidence.GetHostEvidence<StrongName>();
            // We have to load Razor with full trust (so all methods are SecurityCritical)
            // This is because we apply AllowPartiallyTrustedCallers to RazorEngine, because
            // We need the untrusted (transparent) code to be able to inherit TemplateBase.
            // Because in the normal environment/appdomain we run as full trust and the Razor assembly has no security attributes
            // it will be completely SecurityCritical. 
            // This means we have to mark a lot of our members SecurityCritical (which is fine).
            // However in the sandbox domain we have partial trust and because razor has no Security attributes that means the
            // code will be transparent (this is where we get a lot of exceptions, because we now have different security attributes)
            // To work around this we give Razor full trust in the sandbox as well.
            StrongName razorAssembly = typeof(RazorTemplateEngine).Assembly.Evidence.GetHostEvidence<StrongName>();
            AppDomainSetup adSetup = new AppDomainSetup();
            adSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            AppDomain newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet, razorEngineAssembly, razorAssembly);
            return newDomain;
        }
    }
}
