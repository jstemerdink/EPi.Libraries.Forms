namespace EPi.Libraries.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using EPiServer.Forms.Configuration;
    using EPiServer.Forms.Controllers;
    using EPiServer.Forms.Core.Internal.VisitorIdentify;
    using EPiServer.Forms.Helpers.Internal;
    using EPiServer.Forms.Implementation;
    using EPiServer.Framework.Configuration;
    using EPiServer.Framework.Localization;
    using EPiServer.Framework.Web.Resources;
    using EPiServer.Logging;
    using EPiServer.ServiceLocation;
    using EPiServer.Web;
    using EPiServer.Web.Routing;

    /// <summary>
    /// Class FormHelpers.
    /// </summary>
    /// <author>Jeroen Stemerdink</author>
    public static class FormHelpers
    {
        /// <summary>
        /// The log
        /// </summary>
        private static readonly ILogger Log = LogManager.GetLogger();

#pragma warning disable 649
        /// <summary>
        /// The form configuration
        /// </summary>
        private static Injected<IEPiServerFormsImplementationConfig> formConfig;
#pragma warning restore 649

#pragma warning disable 649
        /// <summary>
        /// The localization service
        /// </summary>
        private static Injected<LocalizationService> localizationService;
#pragma warning restore 649

#pragma warning disable 649
        /// <summary>
        /// The required client resource list
        /// </summary>
        private static Injected<IRequiredClientResourceList> requiredClientResourceList;
#pragma warning restore 649

#pragma warning disable 649
        /// <summary>
        /// The visitor identify service
        /// </summary>
        private static Injected<VisitorIdentifyService> visitorIdentifyService;
#pragma warning restore 649

        /// <summary>
        /// Serialize some localized text messages to clientside context
        /// </summary>
        /// <returns>System.String.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static string GetCommonMessages()
        {
            var data =
                new
                    {
                        viewMode =
                            new
                                {
                                    malformStepConfiguration =
                                        localizationService.Service.GetString(
                                            "/episerver/forms/viewmode/malformstepconfigruation"),
                                    commonValidationFail =
                                        localizationService.Service.GetString(
                                            "/episerver/forms/viewmode/commonvalidationfail")
                                },
                        fileUpload =
                            new
                                {
                                    overFileSize =
                                        localizationService.Service.GetString(
                                            "/episerver/forms/messages/fileupload/overFileSize"),
                                    invalidFileType =
                                        localizationService.Service.GetString(
                                            "/episerver/forms/messages/fileupload/invalidfiletype"),
                                    postedFile =
                                        localizationService.Service.GetString(
                                            "/episerver/forms/messages/fileupload/postedfile")
                                }
                    };

            return data.ToJson();
        }

        /// <summary>
        /// Gets external resources (script, css) for the given Form container object
        /// </summary>
        /// <param name="scripts">The scripts.</param>
        /// <param name="css">The CSS.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#")]
        public static void GetFormExternalResources(out List<string> scripts, out List<string> css)
        {
            scripts = new List<string>();
            css = new List<string>();

            List<IViewModeExternalResources> allInstances =
                ServiceLocator.Current.GetAllInstances<IViewModeExternalResources>().ToList();

            if (!allInstances.Any())
            {
                return;
            }

            foreach (IViewModeExternalResources externalResources in allInstances.Where(r => r.Resources != null))
            {
                scripts.AddRange(
                    externalResources.Resources.Where(t => "script".Equals(t.Item1, StringComparison.OrdinalIgnoreCase))
                        .Select(t => t.Item2));
                css.AddRange(
                    externalResources.Resources.Where(t => "css".Equals(t.Item1, StringComparison.OrdinalIgnoreCase))
                        .Select(t => t.Item2));
            }
        }

        /// <summary>
        /// Sets the resources.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        public static void SetResources(ControllerContext controllerContext)
        {
            if (controllerContext == null)
            {
                Log.Debug("Could not set the rquired resources beacause the controller context does not exist.");
                return;
            }

            ContextMode contextMode = controllerContext.RequestContext.GetContextMode();
            bool flag1 = (contextMode == ContextMode.Default) || (contextMode == ContextMode.Preview);

            if (!flag1)
            {
                return;
            }

            if (formConfig.Service.InjectFormOwnStylesheet)
            {
                string webResourceUrl = ModuleHelper.GetWebResourceUrl(
                    typeof(FormContainerBlockController),
                    "EPiServer.Forms.ClientResources.ViewMode.EPiServerForms.css");
                requiredClientResourceList.Service.Require(
                    new ClientResource
                        {
                            Name = "EPiServerForms.css",
                            Dependencies = new List<string> { "EPiServerForms_prerequisite.js" },
                            ResourceType = ClientResourceType.Html,
                            InlineContent =
                                "<link rel='stylesheet' type='text/css' data-epiforms-resource='EPiServerForms.css' href='"
                                + webResourceUrl + "' />"
                        }).AtHeader();
            }

            if (!formConfig.Service.WorkInNonJSMode)
            {
                string scriptContent1 =
                    "var epi = epi||{}; epi.EPiServer = epi.EPiServer||{}; epi.EPiServer.Forms = epi.EPiServer.Forms||{};\nepi.EPiServer.Forms.InjectFormOwnJQuery = "
                    + formConfig.Service.InjectFormOwnJQuery.ToString().ToLowerInvariant()
                    + ";epi.EPiServer.Forms.OriginalJQuery = typeof jQuery !== 'undefined' ? jQuery : undefined;";
                requiredClientResourceList.Service.RequireScriptInline(
                    scriptContent1,
                    "EPiServerForms_saveOriginalJQuery.js",
                    new List<string>()).AtHeader();
                if (formConfig.Service.InjectFormOwnJQuery)
                {
                    requiredClientResourceList.Service.RequireScript(
                        ModuleHelper.GetWebResourceUrl(
                            typeof(FormContainerBlockController),
                            "EPiServer.Forms.ClientResources.ViewMode.jquery-1.12.4.min.js"),
                        "Forms.jquery.js",
                        new List<string> { "EPiServerForms_saveOriginalJQuery.js" }).AtHeader();
                }

                string webResourceContent = ModuleHelper.GetWebResourceContent(
                    typeof(FormContainerBlockController),
                    "EPiServer.Forms.ClientResources.ViewMode.EPiServerForms_prerequisite.js");
                List<string> scripts;
                List<string> css;
                GetFormExternalResources(out scripts, out css);
                string scriptContent2 =
                    webResourceContent.Replace(
                        "___CurrentPageLink___",
                        FormsExtensions.GetCurrentPageLink().ToString())
                        .Replace("___CurrentPageLanguage___", FormsExtensions.GetCurrentPageLanguage())
                        .Replace("___ExternalScriptSources___", scripts.ToJson())
                        .Replace("___ExternalCssSources___", css.ToJson())
                        .Replace(
                            "___UploadExtensionBlackList___",
                            formConfig.Service.DefaultUploadExtensionBlackList)
                        .Replace("___Messages___", GetCommonMessages())
                        .Replace("___LocalizedResources___", FormsExtensions.GetLocalizedResources().ToJson());
                requiredClientResourceList.Service.RequireScriptInline(
                    scriptContent2,
                    "EPiServerForms_prerequisite.js",
                    new List<string> { "Forms.jquery.js" }).AtHeader();
                string resourceName = EPiServerFrameworkSection.Instance.ClientResources.Debug
                                          ? "EPiServer.Forms.ClientResources.ViewMode.EPiServerForms.js"
                                          : "EPiServer.Forms.ClientResources.ViewMode.EPiServerForms.min.js";
                requiredClientResourceList.Service.RequireScript(
                    ModuleHelper.GetWebResourceUrl(typeof(FormContainerBlockController), resourceName),
                    "EPiServerForms.js",
                    new List<string> { "Forms.jquery.js", "EPiServerForms_prerequisite.js" }).AtFooter();
            }
        }

        /// <summary>
        /// Check if Visitor is identified. If not, build the identifier and set to the Cookie.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        public static void SetVisitorIdentifierIfNeeded(HttpContextBase httpContext)
        {
            IVisitorIdentifyProvider identifyProvider =
                visitorIdentifyService.Service.GetVisitorIdentifyProvider(httpContext);
            if (!string.IsNullOrWhiteSpace(identifyProvider.GetVisitorIdentifier()))
            {
                return;
            }

            identifyProvider.SetVisitorIdentifier(null);
        }
    }
}