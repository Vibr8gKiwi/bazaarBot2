using System;
using EconomySimDataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EconomySimTests
{
    [TestClass]
    public class EconomySimDataAccessTests
    {
        [TestMethod]
        public void TestInsert()
        {
            //Arrange
            EconomySimDbContext.DoInsert();

            //Act

            //Assert
        }
    }
}
