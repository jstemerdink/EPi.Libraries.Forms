// Code by Jeremy Schneider
namespace EPi.Libraries.Forms
{
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    /// <summary>
    /// Class GhostForm.
    /// </summary>
    /// <seealso cref="HtmlForm" />
    /// <author>Jeroen Stemerdink</author>
    [ToolboxData("<{0}:GhostForm runat=server></{0}:GhostForm>")]
    public class GhostForm : HtmlForm
    {
        /// <summary>
        /// The render
        /// </summary>
        private bool render;

        /// <summary>
        /// Initializes a new instance of the <see cref="GhostForm"/> class.
        /// </summary>
        public GhostForm()
        {
            // By default, show the form tag
            this.render = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [render form tag].
        /// </summary>
        /// <value><c>true</c> if [render form tag]; otherwise, <c>false</c>.</value>
        public bool RenderFormTag
        {
            get
            {
                return this.render;
            }

            set
            {
                this.render = value;
            }
        }

        /// <summary>
        /// Renders the opening HTML tag of the control into the specified <see cref="T:System.Web.UI.HtmlTextWriter" /> object.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> that receives the rendered content.</param>
        protected override void RenderBeginTag(HtmlTextWriter writer)
        {
            // Only render the tag when render is set to true
            if (this.render)
            {
                base.RenderBeginTag(writer);
            }
        }

        /// <summary>
        /// Renders the closing tag for the <see cref="T:System.Web.UI.HtmlControls.HtmlContainerControl" /> control to the specified <see cref="T:System.Web.UI.HtmlTextWriter" /> object.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> that receives the rendered content.</param>
        protected override void RenderEndTag(HtmlTextWriter writer)
        {
            // Only render the tag when render is set to true
            if (this.render)
            {
                base.RenderEndTag(writer);
            }
        }
    }
}