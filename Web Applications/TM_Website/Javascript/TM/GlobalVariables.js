var traceTMObjects  =   true;
var isUndefined     =   function (object) { return  object   === undefined; };
var isDefined       =   function (object) { return  object   !== undefined; };
    
/** typedef */ window.TM              = { trace : traceTMObjects };
/** typedef */ window.TM.Gui          = { trace : traceTMObjects };
/** typedef */ window.TM.QUnit        = { trace : traceTMObjects };
/** typedef */ window.TM.Const        = { trace : traceTMObjects };
/** typedef */ window.TM.Debug        = { trace : traceTMObjects };    // this fails in IE (see below)
/** typedef */ window.TM.Events       = { trace : traceTMObjects };
/** typedef */ window.TM.WebServices  = { trace : traceTMObjects };
/** typedef */ window.TM.ControlPanel = { trace : traceTMObjects };

/** typedef */ window.TM.WebServices.Data         = { trace : traceTMObjects };
/** typedef */ window.TM.WebServices.Config       = { trace : traceTMObjects };
/** typedef */ window.TM.WebServices.Helper       = { trace : traceTMObjects };
/** typedef */ window.TM.WebServices.WS_Utils     = { trace : traceTMObjects };
/** typedef */ window.TM.WebServices.WS_Data      = { trace : traceTMObjects };
/** typedef */ window.TM.WebServices.WS_Users     = { trace : traceTMObjects };
/** typedef */ window.TM.WebServices.WS_Libraries = { trace : traceTMObjects };

/** typedef */ window.TM.Events.Gui               = { trace : traceTMObjects };

/** typedef */ window.TM.Gui.GuidanceItemViewer   = { trace : traceTMObjects };
/** typedef */ window.TM.Gui.GuidanceItemEditor   = { trace : traceTMObjects };
/** typedef */ window.TM.Gui.AppliedFiltersList   = { trace : traceTMObjects };
/** typedef */ window.TM.Gui.DataTableViewer      = { trace : traceTMObjects };
/** typedef */ window.TM.Gui.AppliedFilters       = { trace : traceTMObjects };
/** typedef */ window.TM.Gui.TopRigthLinks        = { trace : traceTMObjects };
/** typedef */ window.TM.Gui.CurrentUser          = { trace : traceTMObjects };
/** typedef */ window.TM.Gui.TextSearch           = { trace : traceTMObjects };
/** typedef */ window.TM.Gui.DataTable            = { trace : traceTMObjects };
/** typedef */ window.TM.Gui.Dialog               = { trace : traceTMObjects };
/** typedef */ window.TM.Gui.Main                 = { trace : traceTMObjects };



//Extra IE mappings
if (window.TM.Debug === undefined)
    {
        window.TM.Debug = {};
    }

    
//Global vars & Constants

window.TM.Const.emptyGuid = "00000000-0000-0000-0000-000000000000";
window.TM.Const.EmptyFunction = function() {};
window.TM.WebServices.Data.lastDataTableData = { aoColumns : [] , aaData: [] };
window.TM.WebServices.Data.filteredDataTable = { aoColumns : [] , aaData: [] };

//Global methods:
window.htmlEscape = function(str)
    {
        return String(str)
                .replace(/&/g, '&amp;')
                .replace(/"/g, '&quot;')
                .replace(/'/g, '&#39;')
                .replace(/</g, '&lt;')
                .replace(/>/g, '&gt;');
    };
        
var htmlUnEscape = function(str) 
    {
        return String(str).replace(/&amp;/g , '&')
                          .replace(/&quot;/g, '"')
                          .replace(/&#39;/g , '\'')
                          .replace(/&lt;/g  , '<')
                          .replace(/&gt;/g  , '>');
    };

if (window.console === undefined)
{
    window.console =
        {
            log         : function() {},
            error       : function() {}
        };
}


//ClickJacking protection (breaks embeded editor, we are going to live with the X-Frame-Options protection)
/*
if (self != top) 
{
    top.location = self.location; 
}*/