// Copyright © 2016 Jeroen Stemerdink.
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
namespace EPi.Libraries.Forms.Views.Blocks
{
    using System;
    using System.Globalization;
    using System.Web.Mvc;
    using System.Web.UI;

    using EPiServer.Core;
    using EPiServer.Forms.Controllers;
    using EPiServer.Forms.Implementation.Elements;
    using EPiServer.Framework.DataAnnotations;
    using EPiServer.Framework.Web;
    using EPiServer.Web;

    /// <summary>
    /// Class FormContainerBlockControl.
    /// </summary>
    /// <seealso cref="EPiServer.Web.BlockControlBase{FormContainerBlock}" />
    /// <author>Jeroen Stemerdink</author>
    [TemplateDescriptor(Inherited = true, TemplateTypeCategory = TemplateTypeCategories.UserControl)]
    public partial class FormContainerBlockControl : BlockControlBase<FormContainerBlock>
    {
        /// <summary>
        /// Gets the fake area.
        /// </summary>
        /// <value>The fake area.</value>
        protected ContentArea FakeArea { get; private set; }

        /// <summary>
        /// Gets the fake context.
        /// </summary>
        /// <value>The fake context.</value>
        protected ControllerContext FakeContext { get; private set; }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {

            using (FormContainerBlockController formContainerBlockController = new FormContainerBlockController())
            {
                this.FakeContext = MvcUtility.GetFormControllerContext(formContainerBlockController);
            }

            this.SetFormAttributes();

            FormHelpers.SetResources(this.FakeContext);

            ContentArea contentArea = new ContentArea();
            contentArea.Items.Add(new ContentAreaItem { ContentLink = this.CurrentBlock.Content.ContentLink });

            this.FakeArea = contentArea;

            base.OnLoad(e);
        }

        /// <summary>
        /// Add the form attributes needed for the 'Forms form" to the WebForm.
        /// </summary>
        private void SetFormAttributes()
        {
            bool validationFail;
            bool.TryParse(this.FakeContext.Controller.ViewBag.ValidationFail, out validationFail);
            string validationFailCssClass = validationFail ? "ValidationFail" : string.Empty;

            this.Page.Form.ID = this.CurrentBlock.Content.ContentGuid.ToString();
            this.Page.Form.ClientIDMode = ClientIDMode.Static;
            this.Page.Form.Action = "post";
            this.Page.Form.Attributes.Add("data-epiforms-type", "form");
            this.Page.Form.Attributes.Add("enctype", "multipart/form-data");
            this.Page.Form.Attributes.Add("class", string.Format(CultureInfo.InvariantCulture, "EPiServerForms {0}", validationFailCssClass));
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            FormHelpers.SetVisitorIdentifierIfNeeded(this.FakeContext.HttpContext);
            base.OnPreRender(e);
        }
    }
}