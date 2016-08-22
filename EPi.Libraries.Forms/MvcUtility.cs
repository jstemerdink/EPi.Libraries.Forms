// Copyright © 2016 Jeroen Stemerdink / Tim Cromarty.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
namespace EPi.Libraries.Forms
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using EPiServer.Forms.Controllers;
    using EPiServer.Logging;

    /// <summary>
    /// Class MvcUtility.
    /// </summary>
    /// <author>Jeroen Stemerdink, Tim Cromarty</author>
    /// <remarks>See https://clicktricity.com/2010/06/22/using-mvc-renderaction-within-a-webform/ for the base.</remarks>
    public static class MvcUtility
    {
        /// <summary>
        /// The log
        /// </summary>
        private static readonly ILogger Log = LogManager.GetLogger();

        /// <summary>
        /// Gets the form controller context.
        /// </summary>
        /// <returns>ControllerContext.</returns>
        public static ControllerContext GetFormControllerContext()
        {
            try
            {
                using (FormContainerBlockController formContainerBlockController = new FormContainerBlockController())
                {
                    return GetFormControllerContext(formContainerBlockController);
                }
            }
            catch (ArgumentNullException argumentNullException)
            {
                Log.Critical(
                    "Cannot create controllercontext for the FormContainerBlock: \r\n {0}",
                    argumentNullException);
            }

            return null;
        }

        /// <summary>
        /// Gets the form controller context.
        /// </summary>
        /// <returns>ControllerContext.</returns>
        public static ControllerContext GetFormControllerContext(FormContainerBlockController formContainerBlockController)
        {
            try
            {
                HttpContextBase httpContextBase = new HttpContextWrapper(HttpContext.Current);
                RouteData routeData = new RouteData();
                routeData.Values.Add("controller", "FormContainerBlock");
                routeData.Values.Add("action", "Index");

                return new ControllerContext(
                    new RequestContext(httpContextBase, routeData),
                    formContainerBlockController);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Log.Critical(
                    "Cannot create controllercontext for the FormContainerBlock: \r\n {0}",
                    argumentNullException);
            }

            return null;
        }

        /// <summary>
        /// Renders the partial.
        /// </summary>
        /// <param name="partialViewName">Partial name of the view.</param>
        /// <param name="model">The model.</param>
        /// <param name="controllerContext">The controller context.</param>
        public static void RenderPartial(string partialViewName, object model, ControllerContext controllerContext)
        {
            if ((controllerContext == null) ||(model == null) || string.IsNullOrWhiteSpace(partialViewName))
            {
                return;
            }

            try
            {
                IView view = FindPartialView(controllerContext, partialViewName);

                ViewContext viewContext = new ViewContext(
                    controllerContext,
                    view,
                    new ViewDataDictionary { Model = model },
                    new TempDataDictionary(),
                    controllerContext.RequestContext.HttpContext.Response.Output);

                view.Render(viewContext, controllerContext.RequestContext.HttpContext.Response.Output);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                Log.Critical("Cannot render partial view for: {0} \r\n {1}", partialViewName, invalidOperationException);
            }
            catch (NotImplementedException notImplementedException)
            {
                Log.Critical("Cannot render partial view for: {0} \r\n {1}", partialViewName, notImplementedException);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Log.Critical("Cannot render partial view for: {0} \r\n {1}", partialViewName, argumentNullException);
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeException)
            {
                Log.Error("Cannot render partial view for: {0} \r\n {1}", partialViewName, argumentOutOfRangeException);
            }
        }

        // Find the view, if not throw an exception
        /// <summary>
        /// Finds the partial view.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="partialViewName">Partial name of the view.</param>
        /// <returns>IView.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the partial view could not be found.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the partial view could not be found.</exception>
        private static IView FindPartialView(ControllerContext controllerContext, string partialViewName)
        {
            ViewEngineResult result = ViewEngines.Engines.FindPartialView(controllerContext, partialViewName);

            if (result.View != null)
            {
                return result.View;
            }

            StringBuilder locationsText = new StringBuilder();

            foreach (string location in result.SearchedLocations)
            {
                locationsText.AppendLine();
                locationsText.Append(location);
            }

            throw new InvalidOperationException(
                string.Format(CultureInfo.InvariantCulture, "Partial view {0} not found. Locations Searched: {1}", partialViewName, locationsText));
        }
    }
}