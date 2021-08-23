
using System.IO;

namespace CsvReaderBenchmarks
{
    using System.Data;
    using System.Data.OleDb;
    using System.IO;

    public sealed class OleDbBenchmark
    {
        private OleDbBenchmark()
        {
        }

        public static object Run(object[] args)
        {
            if (args.Length == 1)
            {
                Run((string) args[0]);
            }
            else
            {
                Run((string) args[0], (int) args[1]);
            }

            return null;
        }

        public static void Run(string path)
        {
            Run(path, -1);
        }

        public static void Run(string path, int field)
        {
            var directory = Path.GetDirectoryName(path);
            var file = Path.GetFileName(path);

            using (var cnn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + directory + @";Extended Properties=""Text;HDR=No;FMT=Delimited"""))
            {
                using (var cmd = new OleDbCommand(@"SELECT * FROM " + file, cnn))
                {
                    cnn.Open();

                    using (var dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        string s;

                        if (field == -1)
                        {
                            while (dr.Read())
                            {
                                for (var i = 0; i < dr.FieldCount; i++)
                                {
                                    s = dr.GetValue(i) as string;
                                }
                            }
                        }
                        else
                        {
                            while (dr.Read())
                            {
                                s = dr.GetValue(field) as string;
                            }
                        }
                    }
                }
            }
        }
    }

}