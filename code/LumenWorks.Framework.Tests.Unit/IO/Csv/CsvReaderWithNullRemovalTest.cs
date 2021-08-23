using System;
using System.IO;
using System.Text;

using LumenWorks.Framework.IO.Csv;

using NUnit.Framework;

namespace LumenWorks.Framework.Tests.Unit.IO.Csv
{
    [TestFixture()]
    public class CsvReaderWithNullRemovalTest
    {
        private void Checkdata5(int bufferSize)
        {
            const string data = CsvReaderSampleData.SampleData1;

            try
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), true, Encoding.ASCII, bufferSize))
                {
                    CsvReaderSampleData.CheckSampleData1(csv, true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("BufferSize={0}", bufferSize), ex);
            }
        }

        [TestCase((string)null)]
        [TestCase("")]
        [TestCase("AnotherName")]
        public void GetFieldHeaders_WithEmptyHeaderNamesWithNullRemovalStreamReader(string defaultHeaderName)
        {
            if (defaultHeaderName == null)
            {
                defaultHeaderName = "Column";
            }

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(",  ,,aaa,\"   \",,,")), true, Encoding.ASCII))
            {
                csv.DefaultHeaderName = defaultHeaderName;

                Assert.IsFalse(csv.ReadNextRecord());
                Assert.AreEqual(8, csv.FieldCount);

                var headers = csv.GetFieldHeaders();
                Assert.AreEqual(csv.FieldCount, headers.Length);

                Assert.AreEqual("aaa", headers[3]);
                foreach (var index in new[] { 0, 1, 2, 4, 5, 6, 7 })
                {
                    Assert.AreEqual(defaultHeaderName + index, headers[index]);
                }
            }
        }

        [TestCase("", ValueTrimmingOptions.None, new string[] { })]
        [TestCase("", ValueTrimmingOptions.QuotedOnly, new string[] { })]
        [TestCase("", ValueTrimmingOptions.UnquotedOnly, new string[] { })]
        [TestCase(" aaa , bbb , ccc ", ValueTrimmingOptions.None, new[] { " aaa ", " bbb ", " ccc " })]
        [TestCase(" aaa , bbb , ccc ", ValueTrimmingOptions.QuotedOnly, new[] { " aaa ", " bbb ", " ccc " })]
        [TestCase(" aaa , bbb , ccc ", ValueTrimmingOptions.UnquotedOnly, new[] { "aaa", "bbb", "ccc" })]
        [TestCase("\" aaa \",\" bbb \",\" ccc \"", ValueTrimmingOptions.None, new[] { " aaa ", " bbb ", " ccc " })]
        [TestCase("\" aaa \",\" bbb \",\" ccc \"", ValueTrimmingOptions.QuotedOnly, new[] { "aaa", "bbb", "ccc" })]
        [TestCase("\" aaa \",\" bbb \",\" ccc \"", ValueTrimmingOptions.UnquotedOnly, new[] { " aaa ", " bbb ", " ccc " })]
        [TestCase(" aaa , bbb ,\" ccc \"", ValueTrimmingOptions.None, new[] { " aaa ", " bbb ", " ccc " })]
        [TestCase(" aaa , bbb ,\" ccc \"", ValueTrimmingOptions.QuotedOnly, new[] { " aaa ", " bbb ", "ccc" })]
        [TestCase(" aaa , bbb ,\" ccc \"", ValueTrimmingOptions.UnquotedOnly, new[] { "aaa", "bbb", " ccc " })]
        public void TrimFieldValuesTestWithNullRemovalStreamReader(string data, ValueTrimmingOptions trimmingOptions, params string[] expected)
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)),
                                                 false,
                                                 Encoding.ASCII,
                                                 CsvReader.DefaultDelimiter,
                                                 CsvReader.DefaultQuote,
                                                 CsvReader.DefaultEscape,
                                                 CsvReader.DefaultComment,
                                                 trimmingOptions))
            {
                while (csv.ReadNextRecord())
                {
                    var actual = new string[csv.FieldCount];
                    csv.CopyCurrentRecordTo(actual);

                    CollectionAssert.AreEqual(expected, actual);
                }
            }
        }

        [Test]
        public void ArgumentTestCopyCurrentRecordTo1WithNullRemovalStreamReader()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
                {
                    csv.CopyCurrentRecordTo(null);
                }
            });
        }

        [Test]
        public void ArgumentTestCopyCurrentRecordTo2WithNullRemovalStreamReader()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
                {
                    csv.CopyCurrentRecordTo(new string[1], -1);
                }
            });
        }

        [Test]
        public void ArgumentTestCopyCurrentRecordTo3WithNullRemovalStreamReader()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
                {
                    csv.CopyCurrentRecordTo(new string[1], 1);
                }
            });
        }

        [Test]
        public void ArgumentTestCopyCurrentRecordTo4WithNullRemovalStreamReader()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
                {
                    csv.ReadNextRecord();
                    csv.CopyCurrentRecordTo(new string[CsvReaderSampleData.SampleData1RecordCount - 1], 0);
                }
            });
        }

        [Test]
        public void ArgumentTestCopyCurrentRecordTo5WithNullRemovalStreamReader()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
                {
                    csv.ReadNextRecord();
                    csv.CopyCurrentRecordTo(new string[CsvReaderSampleData.SampleData1RecordCount], 1);
                }
            });
        }

        [Test]
        public void ArgumentTestCtor1WithNullRemovalStreamReader()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (var csv = new CsvReader(null, false, Encoding.ASCII))
                {
                }
            });
        }

        [Test]
        public void ArgumentTestCtor2WithNullRemovalStreamReader()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("hello world!")), false, Encoding.ASCII, 0))
                {
                }
            });
        }

        [Test]
        public void ArgumentTestCtor3WithNullRemovalStreamReader()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("hello world!")), false, Encoding.ASCII, -1))
                {
                }
            });
        }

        [Test]
        public void ArgumentTestCtor4WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("hello world!")), false, Encoding.ASCII, 123))
            {
                Assert.AreEqual("hello world!".Length, csv.BufferSize);
            }
        }

        [Test]
        public void ArgumentTestIndexer10WithNullRemovalStreamReader()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
                {
                    var s = csv[-1, 0];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer1WithNullRemovalStreamReader()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
                {
                    var s = csv[-1];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer2WithNullRemovalStreamReader()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
                {
                    var s = csv[CsvReaderSampleData.SampleData1RecordCount];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer3WithNullRemovalStreamReader()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
                {
                    var s = csv["asdf"];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer4WithNullRemovalStreamReader()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
                {
                    var s = csv[CsvReaderSampleData.SampleData1Header0];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer5WithNullRemovalStreamReader()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
                {
                    var s = csv[null];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer6WithNullRemovalStreamReader()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
                {
                    var s = csv[string.Empty];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer7WithNullRemovalStreamReader()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
                {
                    var s = csv[null];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer8WithNullRemovalStreamReader()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
                {
                    var s = csv[string.Empty];
                }
            });
        }

        [Test]
        public void ArgumentTestIndexer9WithNullRemovalStreamReader()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
                {
                    var s = csv["asdf"];
                }
            });
        }


        [Test]
        public void CopyCurrentRecordToTest1WithNullRemovalStreamReader()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
                {
                    csv.CopyCurrentRecordTo(new string[CsvReaderSampleData.SampleData1RecordCount]);
                }
            });
        }

        [Test]
        public void FieldCountTest1WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                CsvReaderSampleData.CheckSampleData1(csv, true);
            }
        }

        [Test]
        public void GetFieldHeadersTest_EmptyCsvWithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("#asdf\n\n#asdf,asdf")), true, Encoding.ASCII))
            {
                var headers = csv.GetFieldHeaders();

                Assert.IsNotNull(headers);
                Assert.AreEqual(0, headers.Length);
            }
        }

        [Test]
        public void GetFieldHeadersTest1WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                var headers = csv.GetFieldHeaders();

                Assert.IsNotNull(headers);
                Assert.AreEqual(0, headers.Length);
            }
        }

        [Test]
        public void GetFieldHeadersTest2WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
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
        public void HasHeader_HeaderDoesNotExistWithNullRemovalStreamReader()
        {
            var header = "Phone Number";

            using (var csvReader = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                Assert.IsFalse(csvReader.HasHeader(header));
            }
        }

        [Test]
        public void HasHeader_HeaderExistsWithNullRemovalStreamReader()
        {
            var header = "First Name";

            using (var csvReader = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                Assert.IsTrue(csvReader.HasHeader(header));
            }
        }

        [Test]
        public void HasHeader_NullFieldHeadersWithNullRemovalStreamReader()
        {
            var header = "NonExistingHeader";

            using (var csvReader = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("Value1,Value2")), false, Encoding.ASCII))
            {
                Assert.IsFalse(csvReader.HasHeader(header));
            }
        }

        [Test]
        public void HasHeader_NullHeaderWithNullRemovalStreamReader()
        {
            using (var csvReader = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("Header1,Header2\r\nValue1,Value2")), true, Encoding.ASCII))
            {
                Assert.Throws<ArgumentNullException>(delegate
                {
                    csvReader.HasHeader(null);
                });
            }
        }

        [Test]
        public void IndexerTest1WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                for (var i = 0; i < CsvReaderSampleData.SampleData1RecordCount; i++)
                {
                    var s = csv[i, 0];
                    CsvReaderSampleData.CheckSampleData1(i, csv);
                }
            }
        }

        [Test]
        public void IndexerTest2WithNullRemovalStreamReader()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
                {
                    var s = csv[1, 0];
                    s = csv[0, 0];
                }
            });
        }

        [Test]
        public void IndexerTest3WithNullRemovalStreamReader()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
                {
                    var s = csv[CsvReaderSampleData.SampleData1RecordCount, 0];
                }
            });
        }

        [Test]
        public void IterationTest1WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
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
        public void IterationTest2WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                string[] previous = null;

                foreach (var record in csv)
                {
                    Assert.IsFalse(ReferenceEquals(previous, record));

                    previous = record;
                }
            }
        }

        [Test]
        public void MoveToTest1WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                for (var i = 0; i < CsvReaderSampleData.SampleData1RecordCount; i++)
                {
                    Assert.IsTrue(csv.MoveTo(i));
                    CsvReaderSampleData.CheckSampleData1(i, csv);
                }
            }
        }

        [Test]
        public void MoveToTest2WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                Assert.IsTrue(csv.MoveTo(1));
                Assert.IsFalse(csv.MoveTo(0));
            }
        }

        [Test]
        public void MoveToTest3WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                Assert.IsFalse(csv.MoveTo(CsvReaderSampleData.SampleData1RecordCount));
            }
        }

        [Test]
        public void MoveToTest4WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                csv.SupportsMultiline = false;

                var headers = csv.GetFieldHeaders();

                Assert.IsTrue(csv.MoveTo(2));
                Assert.AreEqual(2, csv.CurrentRecordIndex);
                CsvReaderSampleData.CheckSampleData1(csv, false);
            }
        }

        [Test]
        public void MoveToTest5WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.MoveTo(-1));
                csv.MoveTo(0);
                Assert.IsFalse(csv.MoveTo(-1));
            }
        }

        [Test]
        public void ParsingTest10WithNullRemovalStreamReader()
        {
            const string data = "1\r2";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
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
        public void ParsingTest11WithNullRemovalStreamReader()
        {
            const string data = "1\n2";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
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
        public void ParsingTest12WithNullRemovalStreamReader()
        {
            const string data = "1\r\n2";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
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
        public void ParsingTest13WithNullRemovalStreamReader()
        {
            const string data = "1\r";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest14WithNullRemovalStreamReader()
        {
            const string data = "1\n";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest15WithNullRemovalStreamReader()
        {
            const string data = "1\r\n";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest16WithNullRemovalStreamReader()
        {
            const string data = "1\r2\n";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, '\r', '"', '\"', '#', ValueTrimmingOptions.UnquotedOnly))
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
        public void ParsingTest17WithNullRemovalStreamReader()
        {
            const string data = "\"July 4th, 2005\"";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("July 4th, 2005", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest18WithNullRemovalStreamReader()
        {
            const string data = " 1";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\"', '\"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(" 1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest19WithNullRemovalStreamReader()
        {
            var data = string.Empty;

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest1WithNullRemovalStreamReader()
        {
            const string data = "1\r\n\r\n1";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest20WithNullRemovalStreamReader()
        {
            const string data = "user_id,name\r\n1,Bruce";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), true, Encoding.ASCII))
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
        public void ParsingTest21WithNullRemovalStreamReader()
        {
            const string data = "\"data \r\n here\"";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\"', '\"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("data \r\n here", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest22WithNullRemovalStreamReader()
        {
            const string data = "\r\r\n1\r";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, '\r', '\"', '\"', '#', ValueTrimmingOptions.None))
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
        public void ParsingTest23WithNullRemovalStreamReader()
        {
            const string data = "\"double\"\"\"\"double quotes\"";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\"', '\"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("double\"\"double quotes", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest24WithNullRemovalStreamReader()
        {
            const string data = "1\r";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest25WithNullRemovalStreamReader()
        {
            const string data = "1\r\n";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest26WithNullRemovalStreamReader()
        {
            const string data = "1\n";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest27WithNullRemovalStreamReader()
        {
            const string data = "'bob said, ''Hey!''',2, 3 ";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\'', '\'', '#', ValueTrimmingOptions.UnquotedOnly))
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
        public void ParsingTest28WithNullRemovalStreamReader()
        {
            const string data = "\"data \"\" here\"";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\0', '\\', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("\"data \"\" here\"", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest29WithNullRemovalStreamReader()
        {
            var data = new string('a', 75) + "," + new string('b', 75);

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
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
        public void ParsingTest2WithNullRemovalStreamReader()
        {
            // ["Bob said, ""Hey!""",2, 3 ]
            const string data = "\"Bob said, \"\"Hey!\"\"\",2, 3 ";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(@"Bob said, ""Hey!""", csv[0]);
                Assert.AreEqual("2", csv[1]);
                Assert.AreEqual("3", csv[2]);

                Assert.IsFalse(csv.ReadNextRecord());
            }

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '"', '"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(@"Bob said, ""Hey!""", csv[0]);
                Assert.AreEqual("2", csv[1]);
                Assert.AreEqual(" 3 ", csv[2]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest30WithNullRemovalStreamReader()
        {
            const string data = "1\r\n\r\n1";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
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
        public void ParsingTest31WithNullRemovalStreamReader()
        {
            const string data = "1\r\n# bunch of crazy stuff here\r\n1";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\"', '\"', '#', ValueTrimmingOptions.None))
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
        public void ParsingTest32WithNullRemovalStreamReader()
        {
            const string data = "\"1\",Bruce\r\n\"2\n\",Toni\r\n\"3\",Brian\r\n";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\"', '\"', '#', ValueTrimmingOptions.None))
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
        public void ParsingTest33WithNullRemovalStreamReader()
        {
            const string data = "\"double\\\\\\\\double backslash\"";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\"', '\\', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("double\\\\double backslash", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest34WithNullRemovalStreamReader()
        {
            const string data = "\"Chicane\", \"Love on the Run\", \"Knight Rider\", \"This field contains a comma, but it doesn't matter as the field is quoted\"\r\n" +
                                "\"Samuel Barber\", \"Adagio for Strings\", \"Classical\", \"This field contains a double quote character, \"\", but it doesn't matter as it is escaped\"";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\"', '\"', '#', ValueTrimmingOptions.UnquotedOnly))
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
        public void ParsingTest35WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("\t")), false, Encoding.ASCII, '\t'))
            {
                Assert.AreEqual(2, csv.FieldCount);

                Assert.IsTrue(csv.ReadNextRecord());

                Assert.AreEqual(string.Empty, csv[0]);
                Assert.AreEqual(string.Empty, csv[1]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest36WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                csv.SupportsMultiline = false;
                CsvReaderSampleData.CheckSampleData1(csv, true);
            }
        }

        [Test]
        public void ParsingTest37WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                csv.SupportsMultiline = false;
                CsvReaderSampleData.CheckSampleData1(csv, true);
            }
        }

        [Test]
        public void ParsingTest38WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("abc,def,ghi\n")), false, Encoding.ASCII))
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
        public void ParsingTest39WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("00,01,   \n10,11,   ")),
                                                 false,
                                                 Encoding.ASCII,
                                                 CsvReader.DefaultDelimiter,
                                                 CsvReader.DefaultQuote,
                                                 CsvReader.DefaultEscape,
                                                 CsvReader.DefaultComment,
                                                 ValueTrimmingOptions.UnquotedOnly,
                                                 1))
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
        public void ParsingTest3WithNullRemovalStreamReader()
        {
            const string data = "1\r2\n";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("2", csv[0]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest40WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("\"00\",\n\"10\",")), false, Encoding.ASCII))
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
        public void ParsingTest41WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("First record          ,Second record")),
                                                 false,
                                                 Encoding.ASCII,
                                                 CsvReader.DefaultDelimiter,
                                                 CsvReader.DefaultQuote,
                                                 CsvReader.DefaultEscape,
                                                 CsvReader.DefaultComment,
                                                 ValueTrimmingOptions.UnquotedOnly,
                                                 16))
            {
                Assert.AreEqual(2, csv.FieldCount);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("First record", csv[0]);
                Assert.AreEqual("Second record", csv[1]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest42WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(" ")), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(1, csv.FieldCount);
                Assert.AreEqual(string.Empty, csv[0]);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest43WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("a,b\n   ")), false, Encoding.ASCII))
            {
                csv.SkipEmptyLines = true;
                csv.MissingFieldAction = MissingFieldAction.ReplaceByNull;

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(2, csv.FieldCount);
                Assert.AreEqual("a", csv[0]);
                Assert.AreEqual("b", csv[1]);

                csv.ReadNextRecord();
                Assert.AreEqual(string.Empty, csv[0]);
                Assert.AreEqual(null, csv[1]);
            }
        }

        [Test]
        public void ParsingTest44WithNullRemovalStreamReader()
        {
            const string data = "\"01234567891\"\r\ntest";

            Assert.Throws<MalformedCsvException>(() =>
            {
                using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
                {
                    csv.MaxQuotedFieldLength = 10;
                    csv.ReadNextRecord();
                }
            });
        }

        [Test]
        public void ParsingTest45WithNullRemovalStreamReader()
        {
            const string data = "\"01234567891\"\r\ntest";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                csv.MaxQuotedFieldLength = 11;
                csv.ReadNextRecord();
                Assert.AreEqual("01234567891", csv[0]);
            }
        }

        [Test]
        public void ParsingTest4WithNullRemovalStreamReader()
        {
            const string data = "\"\n\r\n\n\r\r\",,\t,\n";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
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
        public void ParsingTest5_RandomBufferSizesWithNullRemovalStreamReader()
        {
            var random = new Random();

            for (var i = 0; i < 1000; i++)
            {
                Checkdata5(random.Next(1, 512));
            }
        }

        [Test]
        public void ParsingTest5WithNullRemovalStreamReader()
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
        public void ParsingTest6WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("1,2")), false, Encoding.ASCII))
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
        public void ParsingTest7WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("\r\n1\r\n")), false, Encoding.ASCII))
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
        public void ParsingTest8WithNullRemovalStreamReader()
        {
            const string data = "\"bob said, \"\"Hey!\"\"\",2, 3 ";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\"', '\"', '#', ValueTrimmingOptions.UnquotedOnly))
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
        public void ParsingTest9WithNullRemovalStreamReader()
        {
            const string data = ",";

            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
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
        public void SkipEmptyLinesTest1WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("00\n\n10")), false, Encoding.ASCII))
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
        public void SkipEmptyLinesTest2WithNullRemovalStreamReader()
        {
            using (var csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("00\n\n10")), false, Encoding.ASCII))
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

        [Test]
        public void UnicodeParsingTest1WithNullRemovalStreamReader()
        {
            // control characters and comma are skipped

            var raw = new char[65536 - 13];

            for (var i = 0; i < raw.Length; i++)
            {
                raw[i] = (char)(i + 14);
            }

            raw[44 - 14] = ' '; // skip comma

            var data = new string(raw);

            var dataBytes = Encoding.Unicode.GetBytes(data);
            var dataBack = Encoding.Unicode.GetString(dataBytes);

            using (var csv = new CsvReader(new MemoryStream(dataBytes), false, Encoding.Unicode))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(dataBack, csv[0]);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void UnicodeParsingTest2WithNullRemovalStreamReader()
        {
            byte[] buffer;

            var test = "M�nchen";

            using (var stream = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.WriteLine(test);
                }

                buffer = stream.ToArray();
            }

            using (var csv = new CsvReader(new MemoryStream(buffer), false, Encoding.UTF8))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(test, csv[0]);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void UnicodeParsingTest3WithNullRemovalStreamReader()
        {
            byte[] buffer;

            var test = "M�nchen";

            using (var stream = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(stream, Encoding.UTF32))
                {
                    writer.Write(test);
                }

                buffer = stream.ToArray();
            }

            using (var csv = new CsvReader(new MemoryStream(buffer), false, Encoding.UTF32))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(test, csv[0]);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }
    }
}
