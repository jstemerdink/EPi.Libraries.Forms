<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FormBlockControl.ascx.cs" Inherits="EPi.Libraries.Forms.Views.Blocks.FormContainerBlockControl" %>
<%@ Import Namespace="EPi.Libraries.Forms" %>
<% MvcUtility.RenderPartial("FormsContentArea", this.FakeArea, this.FakeContext); %>
