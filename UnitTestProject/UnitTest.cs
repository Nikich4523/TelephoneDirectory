using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

using TelephoneDirectoryF;
using TelephoneDirectoryF.Models;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest
    {
        TelephoneDirectoryEntities _context;
        RecordList _records;
        MainWindow _wnd;

        // Initializing a database connection
        public UnitTest()
        {
            _wnd = new MainWindow();
            _context = _wnd._context;
            _records = new RecordList(_context.Records.ToList());
        }

        [TestMethod]
        public void AddTest()
        {
            // declare variables for test
            string name, phoneNumber, address, schedule, fieldActivity;

            // 1 - true
            name = "ТТИТ";
            phoneNumber = "+79641166987";
            address = "г. Томск, ул. Герцена, 18";
            schedule = "24/7";
            fieldActivity = "Образование";

            Assert.IsTrue(_wnd.AddRecord(name, phoneNumber, address, schedule, fieldActivity));

            // 2 - true
            name = "Спутник";
            phoneNumber = "79641166988";
            address = "г. Томск, ул. Красноармейская";
            schedule = "24/7";
            fieldActivity = "IT";

            Assert.IsTrue(_wnd.AddRecord(name, phoneNumber, address, schedule, fieldActivity));

            // 3 - true
            name = "Rubius";
            phoneNumber = "79641166989";
            address = "г. Томск, ул. Нахимова";
            schedule = "ПН, ВТ, СР, ЧТ, ПТ";
            fieldActivity = "IT";

            Assert.IsTrue(_wnd.AddRecord(name, phoneNumber, address, schedule, fieldActivity));

            // 4 - false (uncorrect phone number)
            name = "CryptonStudio";
            phoneNumber = "+79641166983423";
            address = "г. Томск, ул. Разбитых сердец";
            schedule = "24/7";
            fieldActivity = "Blockchain";

            Assert.IsFalse(_wnd.AddRecord(name, phoneNumber, address, schedule, fieldActivity));
        }

        [TestMethod]
        public void UpdateTest()
        {
            // declare variables for test
            string name, phoneNumber, address, schedule, fieldActivity;
            Records record;

            // 1 - true
            record = new Records() { Name = "", PhoneNumber = "", Address = "", Schedule = "", FieldActivity = ""};
            name = "ТТИТ++";
            phoneNumber = "+79641166987";
            address = "г. Томск, ул. Герцена, 18";
            schedule = "24/7";
            fieldActivity = "Образование";

            Assert.IsTrue(_wnd.UpdateRecord(ref record, name, phoneNumber, address, schedule, fieldActivity));

            // 2 - false (uncorrect phone number)
            name = "CryptonStudio";
            phoneNumber = "+79641166983423";
            address = "г. Томск, ул. Разбитых сердец";
            schedule = "24/7";
            fieldActivity = "Blockchain";

            Assert.IsFalse(_wnd.UpdateRecord(ref record, name, phoneNumber, address, schedule, fieldActivity));
        }

        [TestMethod]
        public void DeleteTest()
        {
            // declare variables for test
            Records record;
            record = new Records() { Name = "", PhoneNumber = "", Address = "", Schedule = "", FieldActivity = "" };

            _context.Records.Add(record);
            Assert.IsTrue(_wnd.DeleteRecord(ref record));
        }

        [TestMethod]
        public void SearchTest()
        {
            // declare variables for test
            string searchStr;

            // 1 - true
            searchStr = "ттит";
            Records expectedRecord = _context.Records.Where(r => r.Name == "ТТИТ").First();
            Records actualRecord = _records.Filter(searchStr)[0];
            Assert.AreEqual(expectedRecord, actualRecord);

            // 2 - true
            searchStr = "hj,jrf";
            expectedRecord = null; ;
            actualRecord = _records.Filter(searchStr).FirstOrDefault();
            Assert.AreEqual(expectedRecord, actualRecord);
        }

        [TestMethod]
        public void SortTest()
        {
            // declare variables for test
            RecordList unsortedRecords = new RecordList(_context.Records.ToList());

            RecordList sortedRecords = new RecordList(unsortedRecords.SortByFieldAcitivity());

            List<string> actualRecordsName = new List<string>();
            foreach (Records record in sortedRecords)
                actualRecordsName.Add(record.Name);

            List<string> expectedRecordsName = new List<string>() { "Спутник", "Rubius", "ТТИТ"};

            CollectionAssert.AreEqual(expectedRecordsName, actualRecordsName);
        }
    }
}
