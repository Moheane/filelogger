using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using UpdatedVersionFilelogger;




namespace UpdatedVersionTest
{
    [TestClass]
    public class UpdatedVersionTest
    {

        //IMPORTS AND SETUP STATIC VARIABLES
        private FileLogger Logger { get; }
        private Mock<IDateProvider> Date_Provider_Mock { get; }
        private Mock<IFileSystem> File_System_Mock { get; }

        private const string Test_message = "Ramahadi_log_message";

        private static readonly DateTime Saturday = new DateTime(2020, 2, 8);
        private static readonly DateTime Sunday = new DateTime(2020, 2, 9);
        private DateTime week_day => new DateTime(2020, 2, 13);

        private string default_log_filename => $"log{week_day:yyyyMMdd}.txt";
        private string default_log_weeekend_filename => "weekend.txt";

        public UpdatedVersionTest()
        {

            //SETUP MOCK VARIABLES
            File_System_Mock = new Mock<IFileSystem>(MockBehavior.Strict);
            File_System_Mock.Setup(fs => fs.Append(It.IsNotNull<string>(), It.IsNotNull<string>()));
            File_System_Mock.Setup(fs => fs.Create(It.IsNotNull<string>()));
            File_System_Mock.Setup(fs => fs.Exists(It.IsNotNull<string>())).Returns(true);
            File_System_Mock.Setup(fs => fs.GetLastWriteTime(It.IsNotNull<string>())).Returns(DateTime.Now);
            File_System_Mock.Setup(fs => fs.Rename(It.IsNotNull<string>(), It.IsNotNull<string>()));

            Date_Provider_Mock = new Mock<IDateProvider>(MockBehavior.Strict);
            Date_Provider_Mock.Setup(dp => dp.Today).Returns(week_day);

            Logger = new FileLogger(File_System_Mock.Object, Date_Provider_Mock.Object);
        }


        [TestMethod]
        public void log_creates_file_and_append_message_if_file_not_exists()
        {
            File_System_Mock.Setup(fs => fs.Exists(default_log_filename)).Returns(false);

            Logger.Log(Test_message);

            File_System_Mock.Verify(fs => fs.Exists(default_log_filename), Times.Once);
            File_System_Mock.Verify(fs => fs.Create(default_log_filename), Times.Once);
            File_System_Mock.Verify(fs => fs.Append(default_log_filename, Test_message), Times.Once);
        }

        [TestMethod]
        public void log_append_message_to_file_if_file_exists()
        {
            Logger.Log(Test_message);

            File_System_Mock.Verify(fs => fs.Exists(default_log_filename), Times.Once);
            File_System_Mock.Verify(fs => fs.Create(default_log_filename), Times.Never);
            File_System_Mock.Verify(fs => fs.Append(default_log_filename, Test_message), Times.Once);
        }



        [TestMethod]
        public void use_IdateProvider_as_default()
        {
            Logger.Log(Test_message);

            Date_Provider_Mock.VerifyGet(dp => dp.Today, Times.AtLeastOnce);
            File_System_Mock.Verify(fs => fs.Append(default_log_filename, Test_message), Times.Once);
        }

        [TestMethod]
        public void logs_to_weekend_on_saturdays()
        {
            
            Date_Provider_Mock.Setup(dp => dp.Today).Returns(Saturday);

            Logger.Log(Test_message);

            Date_Provider_Mock.VerifyGet(dp => dp.Today, Times.AtLeastOnce);
            File_System_Mock.Verify(fs => fs.Append(default_log_weeekend_filename, Test_message), Times.Once);
        }

        [TestMethod]
        public void logs_to_weekend_on_sunday()
        {
            
            Date_Provider_Mock.Setup(dp => dp.Today).Returns(Sunday);

            Logger.Log(Test_message);

            Date_Provider_Mock.VerifyGet(dp => dp.Today, Times.AtLeastOnce);
            File_System_Mock.Verify(fs => fs.Append(default_log_weeekend_filename, Test_message), Times.Once);
        }



    }
}