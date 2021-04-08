using System;
using System.Collections.Generic;
using System.Text;
using DocumentGenerator.Lib.Helpers;
using NUnit.Framework;

namespace DocumentGenerator.Lib.Test
{
    public class AddressCheckerTests
    {

        [Test]
        public void FirstTest()
        {
            AddressChecker ac = new AddressChecker();
            ac.Init();
        }
        
    }
}
