<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubjectReport.aspx.cs" Inherits="CMS.Web.SubjectReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
         <rsweb:ReportViewer ID="appReportViewer" runat="server" Height="1200px" width="99%" ZoomMode="PageWidth"
            BackColor="White" BorderColor="White" AsyncRendering="false" ShowExportControls="false">
        </rsweb:ReportViewer>
        
    </form>
</body>
</html>
