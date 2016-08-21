Change the form on your page/master to the GhostForm to make this work with the clientside functionality of Forms.

<%@ Register TagPrefix="a" Namespace="EPi.Libraries.Forms" Assembly="EPi.Libraries.Forms" %>

<a:GhostForm runat="server">
    ...
</a:GhostForm>


See my blog for more information: https://jstemerdink.wordpress.com/2016/08/18/forms-and-webforms-pt2/

NOTE: other blocks/functionalities that need the form tag will not function, e.g. searchboxes, so be careful of the other content on the page containing the form.