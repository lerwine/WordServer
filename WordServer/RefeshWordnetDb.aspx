<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RefeshWordnetDb.aspx.cs" Inherits="WordServer.RefeshWordnetDb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Refresh WordNet SQL Database</h1>
    <div>
        The new database files should exist in the App_Data/dict folder. If you are confident that they are there, you can click the &quot;Proceed&quot; button.<br />
        Also, it is a good idea that you back up the WordNet SQL database before proceeding.</div>
        <div>
            <asp:Button ID="ProceedButton" runat="server" Text="Proceed" OnClick="ProceedButton_Click" /></div>
    </form>
</body>
</html>
