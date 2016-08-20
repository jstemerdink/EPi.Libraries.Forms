// Copyright � 2016 Jeroen Stemerdink.
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
    using System.Web.Mvc;

    using EPiServer.Core;
    using EPiServer.Editor;
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
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <exception cref="NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.FakeContext = MvcUtility.GetFormControllerContext();

            FormHelpers.SetResources(this.FakeContext);

            if (!PageEditing.PageIsInEditMode)
            {
                try
                {
                    GhostForm form = this.Page.Form as GhostForm;

                    if (form != null)
                    {
                        form.RenderFormTag = false;
                    }
                }
                catch (InvalidOperationException)
                {
                }
            }

            ContentArea contentArea = new ContentArea();
            contentArea.Items.Add(new ContentAreaItem { ContentLink = this.CurrentBlock.Content.ContentLink });

            this.FakeArea = contentArea;
            this.DataBind();
        }

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            FormHelpers.SetVisitorIdentifierIfNeeded(this.FakeContext.HttpContext);
        }
    }
}