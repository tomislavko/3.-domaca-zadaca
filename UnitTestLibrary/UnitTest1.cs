using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ZadatakPrvi;


namespace UnitTestLibrary
{
    [TestClass]
    public class TodoSqlRepositoraTest
    {
        [TestMethod]
        [ExpectedExceptoion(typeof(DuplicateTodoItemException))]
        public void AddingDuplicateTodoItem()
        {

        }
    }
}
