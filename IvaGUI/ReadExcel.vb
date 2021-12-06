Imports System.Data.OleDb

Public Class ReadExcel
    Public DataTable ReadExcel(String fileName, String fileExt) {  
    String conn = String.Empty;  
    DataTable dtexcel = New DataTable();  
    If (fileExt.CompareTo(".xls") == 0)
        conn = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HRD=Yes;IMEX=1';"; //for below excel 2007  
    Else
        conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=NO';"; //for above excel 2007  
    Using (OleDbConnection con = New OleDbConnection(conn)) {  
        Try {  
            OleDbDataAdapter oleAdpt = New OleDbDataAdapter("select * from [Sheet1$]", con); //here we read data from sheet1  
            oleAdpt.Fill(dtexcel); //fill excel data into dataTable  
        } catch {}  
    }  
    Return dtexcel;  
}  

End Class
