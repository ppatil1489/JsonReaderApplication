using JsonMonitorService;
using System.Collections.ObjectModel;
using System.Text.Json.Nodes;

namespace JsonMonitorServiceTest
{
    [TestClass]
    public class JsonReaderTest
    {

        private JsonFileCreatorHelper _jsonFileCreator = new JsonFileCreatorHelper();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private JsonReader _jsonReader;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private int timerCount = 0;
        AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        private int _refreshJSONContentTimeRateInSeconds = 5000 + 3000;

        List<Book> Books = new List<Book>();

        [TestInitialize]
        public void BeforeEachTest()
        {
            timerCount = 0;
            _jsonFileCreator = new JsonFileCreatorHelper();
            autoResetEvent = new AutoResetEvent(false);
            _jsonReader = new JsonReader(UpdateBooksCollection, DisplayProgressBar);
            _jsonReader.FilePath = _jsonFileCreator.FilePath;

        }


        [TestCleanup]
        public void AfterEachTest() 
        {
            _jsonReader?.CancelUpdateJSON();
            _jsonFileCreator?.DeleteTemporaryFile();
            Books = new List<Book>();
            autoResetEvent.Reset();
        }


        [TestMethod]
        public async Task VerifyReadJSonFileContent()
        {
            List<Book> expectedResult = BooksCollectionsetup();
            _jsonFileCreator?.WriteContentToFile(expectedResult);
            await Task.Run(() =>
            {
                _jsonReader?.UpdateJSONContentOnRefreshTimeRate();
            });

            Assert.IsTrue(autoResetEvent.WaitOne(_refreshJSONContentTimeRateInSeconds));
            _jsonReader.CancelUpdateJSON();

            Assert.AreEqual(expectedResult.Count, Books.Count());
            int i = 0;
            foreach(var book in expectedResult)
            {
                Assert.AreEqual(book.Name, Books[i].Name);
                Assert.AreEqual(book.Description, Books[i].Description);
                Assert.AreEqual(book.Rack, Books[i].Rack);
                i++;
            }
        }


        [TestMethod]
        public async Task VerifyTimerCountOnReadJSonFileContent()
        {
            List<Book> expectedResult = BooksCollectionsetup();
            int expectedTimerCount = 2;
            int _refreshJsonContentTimerRateInSeconds = 6000;
            _jsonFileCreator.WriteContentToFile(expectedResult);
            await Task.Run(() =>
            {
                _jsonReader?.UpdateJSONContentOnRefreshTimeRate();
            });

           await Task.Delay(_refreshJsonContentTimerRateInSeconds * expectedTimerCount);
            _jsonReader.CancelUpdateJSON();

            Assert.AreEqual(expectedResult.Count, Books.Count());
            Assert.AreEqual(expectedTimerCount, timerCount);
            int i = 0;
            foreach (var book in expectedResult)
            {
                Assert.AreEqual(book.Name, Books[i].Name);
                Assert.AreEqual(book.Description, Books[i].Description);
                Assert.AreEqual(book.Rack, Books[i].Rack);
                i++;
            }
        }

        [TestMethod]
        public async Task VerifyTimerStoppedWhenExceptionFoundReadingJsonFile()
        {
            List<Book> expectedResult = BooksCollectionsetup();
            _jsonFileCreator.WriteContentToFile(expectedResult);
            _jsonFileCreator.DeleteTemporaryFile();
            await Task.Run(() =>
            {
                _jsonReader?.UpdateJSONContentOnRefreshTimeRate();
            });
            
            Assert.IsFalse(autoResetEvent.WaitOne(_refreshJSONContentTimeRateInSeconds));

            Assert.AreEqual(0, Books.Count());
        }

        [TestMethod]
        public async Task VerifyUpdatedJSonFileContent()
        {
            List<Book> expectedResult = BooksCollectionsetup();
            _jsonFileCreator.WriteContentToFile(expectedResult);
            await Task.Run(() => {
                _jsonReader?.UpdateJSONContentOnRefreshTimeRate();
            });

            Assert.IsTrue(autoResetEvent.WaitOne(_refreshJSONContentTimeRateInSeconds));

            var jsonObjects = _jsonFileCreator.ReadContentFromFile();

            jsonObjects[0].Name = "updated_Book";

            _jsonFileCreator.WriteContentToFile(jsonObjects);

            autoResetEvent = new AutoResetEvent(false);
            Assert.IsTrue(autoResetEvent.WaitOne(_refreshJSONContentTimeRateInSeconds));

            _jsonReader.CancelUpdateJSON();
            Assert.AreEqual(expectedResult.Count, Books.Count());
            int i = 0;
            foreach (var book in expectedResult)
            {
                if (i == 0)
                {
                    Assert.AreNotEqual(book.Name, Books[i].Name);
                }
                else
                {
                    Assert.AreEqual(book.Name, Books[i].Name);
                }
                Assert.AreEqual(book.Description, Books[i].Description);
                Assert.AreEqual(book.Rack, Books[i].Rack);
                Assert.IsTrue(Books[0].IsDataUpdated = true);
                i++;
            }
        }

        [TestMethod]
        public async Task VerifyDeleteJSonFileContent()
        {
            List<Book> expectedResult = BooksCollectionsetup();
            _jsonFileCreator.WriteContentToFile(expectedResult);

            await Task.Run(() => { _jsonReader?.UpdateJSONContentOnRefreshTimeRate(); });

            Assert.IsTrue(autoResetEvent.WaitOne(_refreshJSONContentTimeRateInSeconds));

            var jsonObjects = _jsonFileCreator.ReadContentFromFile();

            jsonObjects = new List<Book>();

            _jsonFileCreator.WriteContentToFile(jsonObjects);

            autoResetEvent = new AutoResetEvent(false);
            Assert.IsTrue(autoResetEvent.WaitOne(_refreshJSONContentTimeRateInSeconds));

            _jsonReader.CancelUpdateJSON();
            Assert.AreEqual(0, Books.Count());
        }

        [TestMethod]
        public async Task VerifyMonitorServiceProcessGetCancelled()
        {
            List<Book> expectedResult = BooksCollectionsetup();
            _jsonFileCreator.WriteContentToFile(expectedResult);
            await Task.Run(() => { _jsonReader?.UpdateJSONContentOnRefreshTimeRate(); });

            Assert.IsTrue(autoResetEvent.WaitOne(_refreshJSONContentTimeRateInSeconds));

            _jsonReader.CancelUpdateJSON();

            var jsonObjects = _jsonFileCreator.ReadContentFromFile();

            jsonObjects[0].Name = "updated_Book";

            _jsonFileCreator.WriteContentToFile(jsonObjects);
            autoResetEvent = new AutoResetEvent(false);
            Assert.IsTrue(!autoResetEvent.WaitOne(_refreshJSONContentTimeRateInSeconds));

            Assert.AreEqual(expectedResult[0].Name, Books[0].Name);
        }

        [TestMethod]
        public async Task VerifyMonitorServiceProcessGetCancelledAndStartedAgain()
        {
            List<Book> expectedResult = BooksCollectionsetup();
            _jsonFileCreator.WriteContentToFile(expectedResult);
            await Task.Run(() => { _jsonReader?.UpdateJSONContentOnRefreshTimeRate(); });

            Assert.IsTrue(autoResetEvent.WaitOne(_refreshJSONContentTimeRateInSeconds));

            _jsonReader.CancelUpdateJSON();
            var jsonObjects = _jsonFileCreator.ReadContentFromFile();

            jsonObjects[0].Name = "updated_Book";

            _jsonFileCreator.WriteContentToFile(jsonObjects);
            Assert.IsTrue(!autoResetEvent.WaitOne(_refreshJSONContentTimeRateInSeconds));

            Assert.AreEqual(expectedResult[0].Name, Books[0].Name);

            autoResetEvent = new AutoResetEvent(false);
            await Task.Run(() => { _jsonReader?.UpdateJSONContentOnRefreshTimeRate(); });
            Assert.IsTrue(autoResetEvent.WaitOne(_refreshJSONContentTimeRateInSeconds));

            Assert.AreNotEqual(expectedResult[0].Name, Books[0].Name);
        }

        public void UpdateBooksCollection(List<Book> books)
        {
            autoResetEvent.Set();
            Books= books;
        }

        public void DisplayProgressBar(bool isProgressStarted)
        {
            timerCount++;
        }

        private List<Book> BooksCollectionsetup()
        {
            List<Book> BooksCollection = new List<Book>();
            BooksCollection.Add(new Book() { Name = "Book1", Description = "Description1", Rack = "10" });
            BooksCollection.Add(new Book() { Name = "Book2", Description = "Description2", Rack = "11" });
            BooksCollection.Add(new Book() { Name = "Book3", Description = "Description3", Rack = "12" });
            BooksCollection.Add(new Book() { Name = "Book4", Description = "Description4", Rack = "13" });
            return BooksCollection;
        }
    }
}