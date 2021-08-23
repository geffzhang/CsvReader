using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using LumenWorks.Framework.IO.Csv;

namespace LumenWorks.Framework.Tests.Unit.IO.Csv
{
    [TestFixture]
    public class CsvReaderTest
    {
        [Test]
        public void ConstructorThrowsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (var csv = new CsvReader(null, false))
                {
                }
            });
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void ConstructorThrowsOutOfRange(int bufferSize)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false, bufferSize))
                {
                }
            });
        }

        [Test]
        public void ArgumentTestCtor4()
        {
            using (var csv = new CsvReader(new StringReader(""), false, 123))
            {
                Assert.AreEqual(123, csv.BufferSize);
            }
        }

        [Test]
        public void ArgumentTestIndexer1()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
                {
                    var s = csv[-1];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer2()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
                {
                    var s = csv[CsvReaderSampleData.SampleData1RecordCount];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer3()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
                {
                    var s = csv["asdf"];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer4()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
                {
                    var s = csv[CsvReaderSampleData.SampleData1Header0];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer5()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
                {
                    var s = csv[null];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer6()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
                {
                    var s = csv[string.Empty];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer7()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
                {
                    var s = csv[null];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer8()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
                {
                    var s = csv[string.Empty];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer9()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
                {
                    var s = csv["asdf"];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer10()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
                {
                    var s = csv[-1, 0];
                }
            });
        }

        [Test]
        public void ArgumentTestCopyCurrentRecordTo1()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
                {
                    csv.CopyCurrentRecordTo(null);
                }
            });
        }

        [Test]
        public void ArgumentTestCopyCurrentRecordTo2()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
                {
                    csv.CopyCurrentRecordTo(new string[1], -1);
                }
            });
        }

        [Test]
        public void ArgumentTestCopyCurrentRecordTo3()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
                {
                    csv.CopyCurrentRecordTo(new string[1], 1);
                }
            });
        }

        [Test]
        public void ArgumentTestCopyCurrentRecordTo4()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
                {
                    csv.ReadNextRecord();
                    csv.CopyCurrentRecordTo(new string[CsvReaderSampleData.SampleData1RecordCount - 1], 0);
                }
            });
        }

        [Test]
        public void ArgumentTestCopyCurrentRecordTo5()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
                {
                    csv.ReadNextRecord();
                    csv.CopyCurrentRecordTo(new string[CsvReaderSampleData.SampleData1RecordCount], 1);
                }
            });
        }

        [Test]
        public void ParsingTest1()
        {
            const string data = "1\r\n\r\n1";

            using (var csv = new CsvReader(new StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest2()
        {
            // ["Bob said, ""Hey!""",2, 3 ]
            const string data = "\"Bob said, \"\"Hey!\"\"\",2, 3 ";

            using (var csv = new CsvReader(new StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(@"Bob said, ""Hey!""", csv[0]);
                Assert.AreEqual("2", csv[1]);
                Assert.AreEqual("3", csv[2]);

                Assert.IsFalse(csv.ReadNextRecord());
            }

            using (var csv = new CsvReader(new StringReader(data), false, ',', '"', '"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(@"Bob said, ""Hey!""", csv[0]);
                Assert.AreEqual("2", csv[1]);
                Assert.AreEqual(" 3 ", csv[2]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest3()
        {
            const string data = "1\r2\n";

            using (var csv = new CsvReader(new StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("2", csv[0]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest4()
        {
            const string data = "\"\n\r\n\n\r\r\",,\t,\n";

            using (var csv = new CsvReader(new StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());

                Assert.AreEqual(4, csv.FieldCount);

                Assert.AreEqual("\n\r\n\n\r\r", csv[0]);
                Assert.AreEqual("", csv[1]);
                Assert.AreEqual("", csv[2]);
                Assert.AreEqual("", csv[3]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest5()
        {
            Checkdata5(1024);

            // some tricky ones ...

            Checkdata5(1);
            Checkdata5(9);
            Checkdata5(14);
            Checkdata5(39);
            Checkdata5(166);
            Checkdata5(194);
        }

        [Test]
        public void ParsingTest5_RandomBufferSizes()
        {
            var random = new Random();

            for (var i = 0; i < 1000; i++)
                Checkdata5(random.Next(1, 512));
        }

        public void Checkdata5(int bufferSize)
        {
            const string data = CsvReaderSampleData.SampleData1;

            try
            {
                using (var csv = new CsvReader(new StringReader(data), true, bufferSize))
                {
                    CsvReaderSampleData.CheckSampleData1(csv, true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("BufferSize={0}", bufferSize), ex);
            }
        }

        [Test]
        public void ParsingTest6()
        {
            using (var csv = new CsvReader(new System.IO.StringReader("1,2"), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual("2", csv[1]);
                Assert.AreEqual(',', csv.Delimiter);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(2, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest7()
        {
            using (var csv = new CsvReader(new System.IO.StringReader("\r\n1\r\n"), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(',', csv.Delimiter);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.AreEqual("1", csv[0]);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest8()
        {
            const string data = "\"bob said, \"\"Hey!\"\"\",2, 3 ";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false, ',', '\"', '\"', '#', ValueTrimmingOptions.UnquotedOnly))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("bob said, \"Hey!\"", csv[0]);
                Assert.AreEqual("2", csv[1]);
                Assert.AreEqual("3", csv[2]);
                Assert.AreEqual(',', csv.Delimiter);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(3, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest9()
        {
            const string data = ",";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(string.Empty, csv[0]);
                Assert.AreEqual(string.Empty, csv[1]);
                Assert.AreEqual(',', csv.Delimiter);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(2, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest10()
        {
            const string data = "1\r2";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("2", csv[0]);
                Assert.AreEqual(1, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest11()
        {
            const string data = "1\n2";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("2", csv[0]);
                Assert.AreEqual(1, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest12()
        {
            const string data = "1\r\n2";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("2", csv[0]);
                Assert.AreEqual(1, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest13()
        {
            const string data = "1\r";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest14()
        {
            const string data = "1\n";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest15()
        {
            const string data = "1\r\n";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest16()
        {
            const string data = "1\r2\n";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false, '\r', '"', '\"', '#', ValueTrimmingOptions.UnquotedOnly))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual("2", csv[1]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(2, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest17()
        {
            const string data = "\"July 4th, 2005\"";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("July 4th, 2005", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest18()
        {
            const string data = " 1";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false, ',', '\"', '\"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(" 1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest19()
        {
            var data = string.Empty;

            using (var csv = new CsvReader(new System.IO.StringReader(data), false))
            {
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest20()
        {
            const string data = "user_id,name\r\n1,Bruce";

            using (var csv = new CsvReader(new System.IO.StringReader(data), true))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual("Bruce", csv[1]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(2, csv.FieldCount);
                Assert.AreEqual("1", csv["user_id"]);
                Assert.AreEqual("Bruce", csv["name"]);
                Assert.IsFalse(csv.ReadNextRecord());
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest21()
        {
            const string data = "\"data \r\n here\"";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false, ',', '\"', '\"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("data \r\n here", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest22()
        {
            const string data = "\r\r\n1\r";

            // NB Need all values as NET 2 doesn't support optional args
            using (var csv = new CsvReader(new System.IO.StringReader(data), false, '\r', '\"', '\"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(3, csv.FieldCount);

                Assert.AreEqual(string.Empty, csv[0]);
                Assert.AreEqual(string.Empty, csv[1]);
                Assert.AreEqual(string.Empty, csv[2]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(string.Empty, csv[1]);
                Assert.AreEqual(1, csv.CurrentRecordIndex);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest23()
        {
            const string data = "\"double\"\"\"\"double quotes\"";

            // NB Need all values as NET 2 doesn't support optional args
            using (var csv = new CsvReader(new System.IO.StringReader(data), false, ',', '\"', '\"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("double\"\"double quotes", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest24()
        {
            const string data = "1\r";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest25()
        {
            const string data = "1\r\n";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest26()
        {
            const string data = "1\n";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest27()
        {
            const string data = "'bob said, ''Hey!''',2, 3 ";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false, ',', '\'', '\'', '#', ValueTrimmingOptions.UnquotedOnly))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("bob said, 'Hey!'", csv[0]);
                Assert.AreEqual("2", csv[1]);
                Assert.AreEqual("3", csv[2]);
                Assert.AreEqual(',', csv.Delimiter);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(3, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest28()
        {
            const string data = "\"data \"\" here\"";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false, ',', '\0', '\\', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("\"data \"\" here\"", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest29()
        {
            var data = new string('a', 75) + "," + new string('b', 75);

            using (var csv = new CsvReader(new System.IO.StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(new string('a', 75), csv[0]);
                Assert.AreEqual(new string('b', 75), csv[1]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(2, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest30()
        {
            const string data = "1\r\n\r\n1";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(1, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest31()
        {
            const string data = "1\r\n# bunch of crazy stuff here\r\n1";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false, ',', '\"', '\"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(1, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest32()
        {
            const string data = "\"1\",Bruce\r\n\"2\n\",Toni\r\n\"3\",Brian\r\n";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false, ',', '\"', '\"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual("Bruce", csv[1]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(2, csv.FieldCount);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("2\n", csv[0]);
                Assert.AreEqual("Toni", csv[1]);
                Assert.AreEqual(1, csv.CurrentRecordIndex);
                Assert.AreEqual(2, csv.FieldCount);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("3", csv[0]);
                Assert.AreEqual("Brian", csv[1]);
                Assert.AreEqual(2, csv.CurrentRecordIndex);
                Assert.AreEqual(2, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest33()
        {
            const string data = "\"double\\\\\\\\double backslash\"";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false, ',', '\"', '\\', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("double\\\\double backslash", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest34()
        {
            const string data = "\"Chicane\", \"Love on the Run\", \"Knight Rider\", \"This field contains a comma, but it doesn't matter as the field is quoted\"\r\n" +
                      "\"Samuel Barber\", \"Adagio for Strings\", \"Classical\", \"This field contains a double quote character, \"\", but it doesn't matter as it is escaped\"";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false, ',', '\"', '\"', '#', ValueTrimmingOptions.UnquotedOnly))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("Chicane", csv[0]);
                Assert.AreEqual("Love on the Run", csv[1]);
                Assert.AreEqual("Knight Rider", csv[2]);
                Assert.AreEqual("This field contains a comma, but it doesn't matter as the field is quoted", csv[3]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(4, csv.FieldCount);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("Samuel Barber", csv[0]);
                Assert.AreEqual("Adagio for Strings", csv[1]);
                Assert.AreEqual("Classical", csv[2]);
                Assert.AreEqual("This field contains a double quote character, \", but it doesn't matter as it is escaped", csv[3]);
                Assert.AreEqual(1, csv.CurrentRecordIndex);
                Assert.AreEqual(4, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest35()
        {
            using (var csv = new CsvReader(new StringReader("\t"), false, '\t'))
            {
                Assert.AreEqual(2, csv.FieldCount);

                Assert.IsTrue(csv.ReadNextRecord());

                Assert.AreEqual(string.Empty, csv[0]);
                Assert.AreEqual(string.Empty, csv[1]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest36()
        {
            using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
            {
                csv.SupportsMultiline = false;
                CsvReaderSampleData.CheckSampleData1(csv, true);
            }
        }

        [Test]
        public void ParsingTest37()
        {
            using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
            {
                csv.SupportsMultiline = false;
                CsvReaderSampleData.CheckSampleData1(csv, true);
            }
        }

        [Test]
        public void ParsingTest38()
        {
            using (var csv = new CsvReader(new StringReader("abc,def,ghi\n"), false))
            {
                var fieldCount = csv.FieldCount;

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("abc", csv[0]);
                Assert.AreEqual("def", csv[1]);
                Assert.AreEqual("ghi", csv[2]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest39()
        {
            using (var csv = new CsvReader(new StringReader("00,01,   \n10,11,   "), false, CsvReader.DefaultDelimiter, CsvReader.DefaultQuote, CsvReader.DefaultEscape, CsvReader.DefaultComment, ValueTrimmingOptions.UnquotedOnly, 1))
            {
                var fieldCount = csv.FieldCount;

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("00", csv[0]);
                Assert.AreEqual("01", csv[1]);
                Assert.AreEqual("", csv[2]);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("10", csv[0]);
                Assert.AreEqual("11", csv[1]);
                Assert.AreEqual("", csv[2]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest40()
        {
            using (var csv = new CsvReader(new StringReader("\"00\",\n\"10\","), false))
            {
                Assert.AreEqual(2, csv.FieldCount);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("00", csv[0]);
                Assert.AreEqual(string.Empty, csv[1]);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("10", csv[0]);
                Assert.AreEqual(string.Empty, csv[1]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest41()
        {
            using (var csv = new CsvReader(new StringReader("First record          ,Second record"), false, CsvReader.DefaultDelimiter, CsvReader.DefaultQuote, CsvReader.DefaultEscape, CsvReader.DefaultComment, ValueTrimmingOptions.UnquotedOnly, 16))
            {
                Assert.AreEqual(2, csv.FieldCount);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("First record", csv[0]);
                Assert.AreEqual("Second record", csv[1]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest42()
        {
            using (var csv = new CsvReader(new StringReader(" "), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(1, csv.FieldCount);
                Assert.AreEqual(string.Empty, csv[0]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void MissingFieldNull()
        {
            using (var csv = new CsvReader(new StringReader("a,b,\nc,d"), false))
            {
                csv.SkipEmptyLines = true;
                csv.MissingFieldAction = MissingFieldAction.ReplaceByNull;

                RowCheck(csv, "a", "b", string.Empty);
                RowCheck(csv, "c", "d", null);

                Assert.That(csv.ReadNextRecord, Is.False);
            }
        }

        [Test]
        public void MissingFieldEmpty()
        {
            using (var csv = new CsvReader(new StringReader("a,b,\nc,d"), false))
            {
                csv.SkipEmptyLines = true;
                csv.MissingFieldAction = MissingFieldAction.ReplaceByEmpty;

                RowCheck(csv, "a", "b", string.Empty);
                RowCheck(csv, "c", "d", string.Empty);

                Assert.That(csv.ReadNextRecord, Is.False);
            }
        }

        [Test]
        public void MissingFieldException()
        {
            using (var csv = new CsvReader(new StringReader("a,b,\nc,d"), false))
            {
                csv.SkipEmptyLines = true;
                csv.MissingFieldAction = MissingFieldAction.ParseError;

                RowCheck(csv, "a", "b", string.Empty);
                RowCheck(csv, "c", "d");

                Assert.Throws<MissingFieldCsvException>(() => { var x = csv[2]; });
            }
        }

        [Test]
        public void ParsingTest44()
        {
            const string data = "\"01234567891\"\r\ntest";

            Assert.Throws<MalformedCsvException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(data), false))
                {
                    csv.MaxQuotedFieldLength = 10;
                    csv.ReadNextRecord();
                }
            });
        }

        [Test]
        public void ParsingTest45()
        {
            const string data = "\"01234567891\"\r\ntest";

            using (var csv = new CsvReader(new StringReader(data), false))
            {
                csv.MaxQuotedFieldLength = 11;
                csv.ReadNextRecord();
                Assert.AreEqual("01234567891", csv[0]);
            }
        }

        [Test]
        public void UnicodeParsingTest1()
        {
            // control characters and comma are skipped

            var raw = new char[65536 - 13];

            for (var i = 0; i < raw.Length; i++)
                raw[i] = (char)(i + 14);

            raw[44 - 14] = ' '; // skip comma

            var data = new string(raw);

            using (var csv = new CsvReader(new StringReader(data), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(data, csv[0]);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void UnicodeParsingTest2()
        {
            byte[] buffer;

            var test = "M�nchen";

            using (var stream = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(stream, Encoding.Unicode))
                {
                    writer.WriteLine(test);
                }

                buffer = stream.ToArray();
            }

            using (var csv = new CsvReader(new StreamReader(new MemoryStream(buffer), Encoding.Unicode, false), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(test, csv[0]);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void UnicodeParsingTest3()
        {
            byte[] buffer;

            var test = "M�nchen";

            using (var stream = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(stream, Encoding.Unicode))
                {
                    writer.Write(test);
                }

                buffer = stream.ToArray();
            }

            using (var csv = new CsvReader(new StreamReader(new MemoryStream(buffer), Encoding.Unicode, false), false))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(test, csv[0]);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void FieldCountTest1()
        {
            using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
            {
                CsvReaderSampleData.CheckSampleData1(csv, true);
            }
        }

        [Test]
        public void GetFieldHeadersTest1()
        {
            using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
            {
                var headers = csv.GetFieldHeaders();

                Assert.IsNotNull(headers);
                Assert.AreEqual(0, headers.Length);
            }
        }

        [Test]
        public void GetFieldHeadersTest2()
        {
            using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
            {
                var headers = csv.GetFieldHeaders();

                Assert.IsNotNull(headers);
                Assert.AreEqual(CsvReaderSampleData.SampleData1RecordCount, headers.Length);

                Assert.AreEqual(CsvReaderSampleData.SampleData1Header0, headers[0]);
                Assert.AreEqual(CsvReaderSampleData.SampleData1Header1, headers[1]);
                Assert.AreEqual(CsvReaderSampleData.SampleData1Header2, headers[2]);
                Assert.AreEqual(CsvReaderSampleData.SampleData1Header3, headers[3]);
                Assert.AreEqual(CsvReaderSampleData.SampleData1Header4, headers[4]);
                Assert.AreEqual(CsvReaderSampleData.SampleData1Header5, headers[5]);

                Assert.AreEqual(0, csv.GetFieldIndex(CsvReaderSampleData.SampleData1Header0));
                Assert.AreEqual(1, csv.GetFieldIndex(CsvReaderSampleData.SampleData1Header1));
                Assert.AreEqual(2, csv.GetFieldIndex(CsvReaderSampleData.SampleData1Header2));
                Assert.AreEqual(3, csv.GetFieldIndex(CsvReaderSampleData.SampleData1Header3));
                Assert.AreEqual(4, csv.GetFieldIndex(CsvReaderSampleData.SampleData1Header4));
                Assert.AreEqual(5, csv.GetFieldIndex(CsvReaderSampleData.SampleData1Header5));
            }
        }

        [Test]
        public void GetFieldHeadersTest_EmptyCsv()
        {
            using (var csv = new CsvReader(new StringReader("#asdf\n\n#asdf,asdf"), true))
            {
                var headers = csv.GetFieldHeaders();

                Assert.IsNotNull(headers);
                Assert.AreEqual(0, headers.Length);
            }
        }

        [TestCase((string)null)]
        [TestCase("")]
        [TestCase("AnotherName")]
        public void GetFieldHeaders_WithEmptyHeaderNames(string defaultHeaderName)
        {
            if (defaultHeaderName == null)
                defaultHeaderName = "Column";

            using (var csv = new CsvReader(new StringReader(",  ,,aaa,\"   \",,,"), true))
            {
                csv.DefaultHeaderName = defaultHeaderName;

                Assert.IsFalse(csv.ReadNextRecord());
                Assert.AreEqual(8, csv.FieldCount);

                var headers = csv.GetFieldHeaders();
                Assert.AreEqual(csv.FieldCount, headers.Length);

                Assert.AreEqual("aaa", headers[3]);
                foreach (var index in new int[] { 0, 1, 2, 4, 5, 6, 7 })
                    Assert.AreEqual(defaultHeaderName + index.ToString(), headers[index]);
            }
        }

        [Test]
        public void OverrideColumnValueTest()
        {
            using (var csv = new CsvReader(new StringReader("Column1,Column2,Column3\nabc,def,ghi\n"), true))
            {
                csv.Columns[csv.GetFieldIndex("Column2")].OverrideValue = "xyz";

                Assert.IsTrue(csv.ReadNextRecord());

                Assert.AreEqual("abc", csv[0]);
                Assert.AreEqual("xyz", csv[1]);
                Assert.AreNotEqual("def", csv[1]);
                Assert.AreEqual("ghi", csv[2]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }


        [Test]
        public void HasHeader_NullHeader()
        {
            string header = null;

            using (var csvReader = new CsvReader(new StringReader("Header1,Header2\r\nValue1,Value2"), true))
            {
                Assert.Throws<ArgumentNullException>(delegate
                {
                    csvReader.HasHeader(header);
                });
            }
        }

        [Test]
        public void HasHeader_DuplicateHeader()
        {
          try
          {
            using (CsvReader csvReader = new CsvReader(new StringReader("Header,Header\r\nValue1,Value2"), true))
            {
              csvReader.ReadNextRecord();
            }
            Assert.Fail("Expected DuplicateHeaderException");
          }
          catch (DuplicateHeaderException ex)
          {
            Assert.AreEqual("Header", ex.HeaderName);
            Assert.AreEqual(1,ex.ColumnIndex);
          }
          catch (Exception)
          {
            Assert.Fail("Expected DuplicateHeaderException");
          }
        }

        [Test]
        public void HasHeader_DuplicateHeader_Override()
        {
            using (CsvReader csvReader = new CsvReader(new StringReader("Header,Header\r\nValue1,Value2"), true))
            {
              csvReader.DuplicateHeaderEncountered += (s, e) => e.HeaderName = $"{e.HeaderName}_{e.Index}";
              csvReader.ReadNextRecord();
              Assert.AreEqual(csvReader.Columns[0].Name,"Header");
              Assert.AreEqual(csvReader.Columns[1].Name,"Header_1");
            }
        }

        [Test]
        public void HasHeader_HeaderExists()
        {
            var header = "First Name";

            using (var csvReader = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
            {
                Assert.IsTrue(csvReader.HasHeader(header));
            }
        }

        [Test]
        public void HasHeader_HeaderDoesNotExist()
        {
            var header = "Phone Number";

            using (var csvReader = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
            {
                Assert.IsFalse(csvReader.HasHeader(header));
            }
        }

        [Test]
        public void HasHeader_NullFieldHeaders()
        {
            var header = "NonExistingHeader";

            using (var csvReader = new CsvReader(new StringReader("Value1,Value2"), false))
            {
                Assert.IsFalse(csvReader.HasHeader(header));
            }
        }

        [Test]
        public void CopyCurrentRecordToTest1()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
                {
                    csv.CopyCurrentRecordTo(new string[CsvReaderSampleData.SampleData1RecordCount]);
                }
            });
        }

        [Test]
        public void MoveToTest1()
        {
            using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
            {
                for (var i = 0; i < CsvReaderSampleData.SampleData1RecordCount; i++)
                {
                    Assert.IsTrue(csv.MoveTo(i));
                    CsvReaderSampleData.CheckSampleData1(i, csv);
                }
            }
        }

        [Test]
        public void MoveToTest2()
        {
            using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
            {
                Assert.IsTrue(csv.MoveTo(1));
                Assert.IsFalse(csv.MoveTo(0));
            }
        }

        [Test]
        public void MoveToTest3()
        {
            using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
            {
                Assert.IsFalse(csv.MoveTo(CsvReaderSampleData.SampleData1RecordCount));
            }
        }

        [Test]
        public void MoveToTest4()
        {
            using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
            {
                csv.SupportsMultiline = false;

                var headers = csv.GetFieldHeaders();

                Assert.IsTrue(csv.MoveTo(2));
                Assert.AreEqual(2, csv.CurrentRecordIndex);
                CsvReaderSampleData.CheckSampleData1(csv, false);
            }
        }

        [Test]
        public void MoveToTest5()
        {
            using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), false))
            {
                Assert.IsTrue(csv.MoveTo(-1));
                csv.MoveTo(0);
                Assert.IsFalse(csv.MoveTo(-1));
            }
        }

        [Test]
        public void IterationTest1()
        {
            using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
            {
                var index = 0;

                foreach (var record in csv)
                {
                    CsvReaderSampleData.CheckSampleData1(csv.HasHeaders, index, record);
                    index++;
                }
            }
        }

        [Test]
        public void IterationTest2()
        {
            using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
            {
                string[] previous = null;

                foreach (var record in csv)
                {
                    Assert.IsFalse(object.ReferenceEquals(previous, record));

                    previous = record;
                }
            }
        }

        [Test]
        public void IndexerTest1()
        {
            using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
            {
                for (var i = 0; i < CsvReaderSampleData.SampleData1RecordCount; i++)
                {
                    var s = csv[i, 0];
                    CsvReaderSampleData.CheckSampleData1(i, csv);
                }
            }
        }

        [Test]
        public void IndexerTest2()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
                {
                    var s = csv[1, 0];
                    s = csv[0, 0];
                }
            });
        }

        [Test]
        public void IndexerTest3()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
                {
                    var s = csv[CsvReaderSampleData.SampleData1RecordCount, 0];
                }
            });
        }

        [Test]
        public void SkipEmptyLinesTest1()
        {
            using (var csv = new CsvReader(new StringReader("00\n\n10"), false))
            {
                csv.SkipEmptyLines = false;

                Assert.AreEqual(1, csv.FieldCount);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("00", csv[0]);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(string.Empty, csv[0]);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("10", csv[0]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void SkipEmptyLinesTest2()
        {
            using (var csv = new CsvReader(new StringReader("00\n\n10"), false))
            {
                csv.SkipEmptyLines = true;

                Assert.AreEqual(1, csv.FieldCount);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("00", csv[0]);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("10", csv[0]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [TestCase("", ValueTrimmingOptions.None, new string[] { })]
        [TestCase("", ValueTrimmingOptions.QuotedOnly, new string[] { })]
        [TestCase("", ValueTrimmingOptions.UnquotedOnly, new string[] { })]
        [TestCase(" aaa , bbb , ccc ", ValueTrimmingOptions.None, new string[] { " aaa ", " bbb ", " ccc " })]
        [TestCase(" aaa , bbb , ccc ", ValueTrimmingOptions.QuotedOnly, new string[] { " aaa ", " bbb ", " ccc " })]
        [TestCase(" aaa , bbb , ccc ", ValueTrimmingOptions.UnquotedOnly, new string[] { "aaa", "bbb", "ccc" })]
        [TestCase("\" aaa \",\" bbb \",\" ccc \"", ValueTrimmingOptions.None, new string[] { " aaa ", " bbb ", " ccc " })]
        [TestCase("\" aaa \",\" bbb \",\" ccc \"", ValueTrimmingOptions.QuotedOnly, new string[] { "aaa", "bbb", "ccc" })]
        [TestCase("\" aaa \",\" bbb \",\" ccc \"", ValueTrimmingOptions.UnquotedOnly, new string[] { " aaa ", " bbb ", " ccc " })]
        [TestCase(" aaa , bbb ,\" ccc \"", ValueTrimmingOptions.None, new string[] { " aaa ", " bbb ", " ccc " })]
        [TestCase(" aaa , bbb ,\" ccc \"", ValueTrimmingOptions.QuotedOnly, new string[] { " aaa ", " bbb ", "ccc" })]
        [TestCase(" aaa , bbb ,\" ccc \"", ValueTrimmingOptions.UnquotedOnly, new string[] { "aaa", "bbb", " ccc " })]
        public void TrimFieldValuesTest(string data, ValueTrimmingOptions trimmingOptions, params string[] expected)
        {
            using (var csv = new CsvReader(new StringReader(data), false, CsvReader.DefaultDelimiter, CsvReader.DefaultQuote, CsvReader.DefaultEscape, CsvReader.DefaultComment, trimmingOptions))
            {
                while (csv.ReadNextRecord())
                {
                    var actual = new string[csv.FieldCount];
                    csv.CopyCurrentRecordTo(actual);

                    CollectionAssert.AreEqual(expected, actual);
                }
            }
        }

        private void RowCheck(CsvReader csv, params object[] values)
        {            
            Assert.That(csv.ReadNextRecord(), Is.True);
            for (var i = 0; i < values.Length; i++)
            {
                Assert.That(csv[i], Is.EqualTo(values[i]), $"Field {i} differs");
            }
        }

        private void ProcessData(CsvReader csv)
        {
            while (csv.ReadNextRecord())
            {
                for (var i = 0; i < csv.FieldCount; i++)
                {
                    var s = csv[i];
                }
            }
        }
    }
}