﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.CMSHelper;
using CMS.PortalEngine;

public partial class CMSModules_Widgets_LiveDialogs_WidgetSelector : CMSLiveModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Public user is not allowed for widgets
        if (!CMSContext.CurrentUser.IsAuthenticated())
        {
            RedirectToAccessDenied(GetString("widgets.security.notallowed"));
        }

        selectElem.AliasPath = QueryHelper.GetString("aliaspath", String.Empty);
        selectElem.ZoneId = QueryHelper.GetString("zoneid", String.Empty);
        selectElem.ZoneType = WidgetZoneTypeCode.ToEnum(QueryHelper.GetString("zonetype", ""));

        bool isInline = QueryHelper.GetBoolean("inline", false);
        selectElem.IsInline = isInline;

        // Base tag is added in master page
        base.AddBaseTag = false;

        // Proceeds the current item selection
        string javascript = @"
            function SelectCurrentWidget() 
            {                
                SelectWidget(selectedValue);
            }
            function SelectWidget(value)
            {
                if (value != null)
                {
                    window.close();";
        if (isInline)
        {
            javascript += @"
                    var editor = wopener.currentEditor || wopener.CMSPlugin.currentEditor;
                    if (editor) {
                        editor.getCommand('InsertWidget').open(value);
                    }";
        }
        else
        {
            javascript += @"
	                if (wopener.OnSelectWidget)
                    {                    
                          wopener.OnSelectWidget(value);                      
                    }	   ";
        }

        javascript += @"  
	            }
		        else
		        {
                    alert(document.getElementById('" + hdnMessage.ClientID + @"').value);		    
		        }                
            }            
            // Cancel action
            function Cancel()
            {
                window.close();
            } ";


        ScriptHelper.RegisterStartupScript(this, typeof(string), "WidgetSelector", ScriptHelper.GetScript(javascript));
        selectElem.SelectFunction = "SelectWidget";
        selectElem.IsLiveSite = true;

        // Set the title and icon
        this.CurrentMaster.Title.TitleText = GetString("widgets.selectortitle");
        this.CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Widget/object.png");

        // Remove default css class
        if (this.CurrentMaster.PanelBody != null)
        {
            Panel pnl = this.CurrentMaster.PanelBody.FindControl("pnlContent") as Panel;
            if (pnl != null)
            {
                pnl.CssClass = String.Empty;
            }
        }
    }
}
