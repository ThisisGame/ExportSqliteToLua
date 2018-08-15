using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Data.SQLite;
using System.Collections;
using System.Data;

namespace ThisisGame
{
    class Program
    {
        static void Main(string[] args)
        {
            string tmpdbpath = args[0];
            string tmpluapath = args[1];

            if(File.Exists(tmpdbpath)==false)
            {
                Console.WriteLine("not found " + tmpdbpath);
                Console.ReadKey();
            }


            SQLiteDatabase db = new SQLiteDatabase(tmpdbpath);
            DataTable recipe;
            String query = "SELECT name FROM sqlite_master " +
                    "WHERE type = 'table'" +
                    "ORDER BY 1";

            recipe = db.GetDataTable(query);

            ArrayList list = new ArrayList();
            foreach (DataRow row in recipe.Rows)
            {
                list.Add(row.ItemArray[0].ToString());
            }

            string tmpExportLuaDirName = tmpluapath;
            if(Directory.Exists(tmpExportLuaDirName)==false)
            {
                Directory.CreateDirectory(tmpExportLuaDirName);
            }

            //foreach (var item in list)
            for(int tableIndex=0;tableIndex<list.Count;tableIndex++)
            {
                Console.WriteLine(tableIndex + "/" + list.Count + "    : " + list[tableIndex]);

                DataTable tmpDataTable = db.GetDataTable("select * from " + list[tableIndex]);

                StreamWriter tmpStreamWrite = new StreamWriter(tmpExportLuaDirName + "/" + tmpDataTable.TableName+".lua");
                tmpStreamWrite.WriteLine(tmpDataTable.TableName+"=");
                tmpStreamWrite.WriteLine("{");

                foreach (var tableRow in tmpDataTable.Rows)
                {
                    string tmpOneTableStr = "    {";
                    DataRow tmpRow = tableRow as DataRow;
                    for (int i = 0; i < tmpRow.ItemArray.Length; i++)
                    {
                        if(i!=0)
                        {
                            tmpOneTableStr = tmpOneTableStr + "," ;
                        }

                        if(tmpDataTable.Columns[i].DataType.Name=="String")
                        {
                            tmpOneTableStr = tmpOneTableStr + tmpDataTable.Columns[i].ColumnName + "=\"" + tmpRow.ItemArray[i]+"\"";
                        }
                        else
                        {
                            tmpOneTableStr = tmpOneTableStr + tmpDataTable.Columns[i].ColumnName + "=" + tmpRow.ItemArray[i];
                        }
                    }

                    tmpOneTableStr = tmpOneTableStr + "},";
                    tmpStreamWrite.WriteLine(tmpOneTableStr);
                }

                tmpStreamWrite.WriteLine("}");

                tmpStreamWrite.WriteLine("return "+ tmpDataTable.TableName);
                tmpStreamWrite.Flush();
                tmpStreamWrite.Close();
            }

            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
