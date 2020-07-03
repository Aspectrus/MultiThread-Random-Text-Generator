using ADODB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadGeneration
{
    public class AccessDatabase
    {
        private Connection con = new ADODB.Connection();
        private string fileName;
        public AccessDatabase()
        {
        }
        public AccessDatabase(string fileName)
        {
            con.Open("Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + fileName + "; OLE DB Services = -1; Jet OLEDB:Engine Type=5");
            this.fileName = fileName;
        }
        public bool CreateNewAccessDatabase(string fileName)
        {
            if (File.Exists(fileName)) return true;
            bool result = false;
            ADOX.Catalog cat = new ADOX.Catalog();
            ADOX.Table table = new ADOX.Table();

            table.Name = "ThreadData";
            table.Columns.Append("ID", ADOX.DataTypeEnum.adInteger);
            table.Keys.Append("PrimaryKey", ADOX.KeyTypeEnum.adKeyPrimary, "ID", null, null);
            table.Columns.Append("ThreadID");
            table.Columns.Append("Time");
            table.Columns.Append("Data");

            try
            {
                cat.Create("Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + fileName + "; Jet OLEDB:Engine Type=5");
                ADODB.Connection con = cat.ActiveConnection;
                table.Columns["ID"].ParentCatalog = cat;
                table.Columns["ID"].Properties["AutoIncrement"].Value = true;
                cat.Tables.Append(table);

                if (con != null)
                    con.Close();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            cat = null;
            return result;
        }
        public void AddDataToMdbFile(TimerParams timerparams, string ThreadGeneratedData)
        {
            object obj = new object();
            string addquery = string.Format("INSERT INTO ThreadData(ThreadID, [Time], Data) VALUES('{0}', '{1}', '{2}');",
                timerparams.timerID,
                DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                ThreadGeneratedData
                );
            con.Execute(addquery, out obj, 0);
        }
    }
}
