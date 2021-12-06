using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using RestSharp;
using AltoHttp;
using System.Net;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using RestSharp.Authenticators;
using ADOX;

namespace ReadExcelFileApp
{
    public partial class Form1 : Form {
        string filePath = string.Empty;
        string fileExt = string.Empty;

        public static string SelectedTable = string.Empty;

        public Form1() {
            InitializeComponent();
            this.Width = 700;
            this.Height = 500;
        }

        private void Form1_Load(object sender, EventArgs e) {
            dataGridView1.Visible = false;
        }


        private void btnChoose_Click(object sender, EventArgs e) {
            OpenFileDialog fileDialog = new OpenFileDialog();//open dialog to choose file
            if (fileDialog.ShowDialog() == DialogResult.OK)//if there is a file choosen by the user
            {
                filePath = fileDialog.FileName;//get the path of the file
                fileExt = Path.GetExtension(filePath);//get the file extension
                textBox1.Text = fileDialog.FileName;
                if (fileExt.CompareTo(".xls") == 0 || fileExt.CompareTo(".xlsx") == 0)
                {
                    try
                    {

                        DataTable dtExcel = new DataTable();
                        dtExcel = ReadExcel(filePath, fileExt);//read excel file
                        dataGridView1.Visible = true;
                        dataGridView1.DataSource = dtExcel;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Please choose .xls or .xlsx file only.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);//custom messageBox to show error
                }
            }
        }

      


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }


        public string[] GetExcelSheetNames(string excelFileName)
        {
            OleDbConnection con = null;
            DataTable dt = null;
            String conStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + excelFileName + ";Extended Properties=Excel 8.0;";
            con = new OleDbConnection(conStr);
            con.Open();
            dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            if (dt == null)
            {
                return null;
            }

            String[] excelSheetNames = new String[dt.Rows.Count];
            int i = 0;

            foreach (DataRow row in dt.Rows)
            {
                excelSheetNames[i] = row["TABLE_NAME"].ToString();
                i++;
            }

            return excelSheetNames;
        }


        public DataTable ReadExcel(string fileName, string fileExt)
        {
            string conn = string.Empty;
            DataTable dtexcel = new DataTable();
            if (fileExt.CompareTo(".xls") == 0)//compare the extension of the file
                conn = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HRD=Yes;IMEX=1';";//for below excel 2007
            else
                conn = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=Yes';", fileName);//for above excel 2007
            using (OleDbConnection con = new OleDbConnection(conn))
            {
                try
                {
                    string sheetName = dropdownSheet.Text;
                    OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from [" + sheetName + "$]", con);//here we read data from sheet1
                    oleAdpt.Fill(dtexcel);//fill excel data into dataTable
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            return dtexcel;
        }


        private void copyAlltoClipboard()
        {
            //to remove the first blank column from datagridview
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectAll();
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }


        private void exportBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = true;
                Microsoft.Office.Interop.Excel.Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
                Microsoft.Office.Interop.Excel.Worksheet sheet1 = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
                int StartCol = 1;
                int StartRow = 1;
                int j = 0, i = 0;

                //Write Headers
                for (j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[StartRow, StartCol + j];
                    myRange.Value2 = dataGridView1.Columns[j].HeaderText;
                }

                StartRow++;

                //Write datagridview content
                for (i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    for (j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        try
                        {
                            Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[StartRow + i, StartCol + j];
                            myRange.Value2 = dataGridView1[j, i].Value == null ? "" : dataGridView1[j, i].Value;
                        }
                        catch
                        {
                            ;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }



        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                System.Windows.MessageBox.Show("Exception Occurred while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
     

        private void button1_Click(object sender, EventArgs e)
        {
            var url = @"https://tfs.worldbank.org/tfs/fi-prod-administrative-and-health-services/22e309d4-033a-49f2-a5d1-8a8f4a105138/_apis/git/repositories/9c74b8ac-4640-42c3-98c8-efa06d059554/blobs/5b8d3d33e8c0168b6352e70c73055401f40e0ee0";
            Process.Start(@"https://tfs.worldbank.org/tfs/fi-prod-administrative-and-health-services/22e309d4-033a-49f2-a5d1-8a8f4a105138/_apis/git/repositories/9c74b8ac-4640-42c3-98c8-efa06d059554/blobs/5b8d3d33e8c0168b6352e70c73055401f40e0ee0");
        }


        private void btnUploadSavedFile_Click(object sender, EventArgs e)
        {
            string url = "https://tfs.worldbank.org/tfs/fi-prod-administrative-and-health-services/22e309d4-033a-49f2-a5d1-8a8f4a105138/_apis/git/repositories/9c74b8ac-4640-42c3-98c8-efa06d059554/Items?path=%2Fsrc%2Fmain%2Fresources%2FTestData.xlsx&recursionLevel=0&includeContentMetadata=true&versionDescriptor.version=loop_feature&versionDescriptor.versionOptions=0&versionDescriptor.versionType=0&includeContent=true&resolveLfs=true";
            string token = "nji3ns73xbs7ihgb4jozrecywr4quozzgkjmlzjjt74ywqg5wewa";
            string credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", token)));

            UriBuilder uri = new UriBuilder();
            uri.Host = "tfs.worldbank.org";
            uri.Path = "/tfs/fi-prod-administrative-and-health-services/22e309d4-033a-49f2-a5d1-8a8f4a105138/_apis/git/repositories/9c74b8ac-4640-42c3-98c8-efa06d059554/Items?path=%2Fsrc%2Fmain%2Fresources%2FTestData.xlsx&recursionLevel=0&includeContentMetadata=true&versionDescriptor.version=loop_feature&versionDescriptor.versionOptions=0&versionDescriptor.versionType=0&includeContent=true&resolveLfs=true";
            uri.Scheme = "https";
            
            RestClient restClient = new RestClient(uri.ToString());
            restClient.AddDefaultHeader("Bearer ", token);
            restClient.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer");


            RestRequest restRequest = new RestRequest("/tfs/fi-prod-administrative-and-health-services/22e309d4-033a-49f2-a5d1-8a8f4a105138/_apis/git/repositories/9c74b8ac-4640-42c3-98c8-efa06d059554/Items?path=%2Fsrc%2Fmain%2Fresources%2FTestData.xlsx&recursionLevel=0&includeContentMetadata=true&versionDescriptor.version=loop_feature&versionDescriptor.versionOptions=0&versionDescriptor.versionType=0&includeContent=true&resolveLfs=true", Method.GET);
            IRestResponse restResponse = restClient.Execute(restRequest);
            Console.WriteLine(restResponse.Content);

        }
    }
}



