using System;
using System.Collections.Generic;
using System.Text;
using DocumentGenerator.Lib.Helpers;
using NUnit.Framework;

namespace DocumentGenerator.Lib.Test
{
    public class AddressCheckerTests
    {
        AddressChecker _addressChecker;

        [SetUp]
        public void SetUp()
        {
            _addressChecker = new AddressChecker();
            _addressChecker.Init();
        }

        [Test]
        public void UlTypeStreetTest()
        {
         
            Assert.AreEqual("ul.", _addressChecker.GetStart("Szafirowa 2, 70-236 Bezrzecze"));

            Assert.AreEqual("ul.", _addressChecker.GetStart("Rutki Laskier 2, 42-500 Będzin"));
        }

        [Test]
        public void AlTypeStreetTest()
        {

            Assert.AreEqual(string.Empty, _addressChecker.GetStart("Aleje Adama Mickiewicza 2, 85-071 Bydgoszcz"));

            Assert.AreEqual(string.Empty, _addressChecker.GetStart("Aleja Józefa Piłsudskiego 2, 41-300 Dąbrowa Górnicza"));

            Assert.AreEqual(string.Empty, _addressChecker.GetStart("Al. Adama Mickiewicza 2, 85-071 Bydgoszcz"));

            Assert.AreEqual(string.Empty, _addressChecker.GetStart("al. Józefa Piłsudskiego 2, 41-300 Dąbrowa Górnicza"));
        }

        [Test]
        public void OsTypeStreetTest()
        {

            Assert.AreEqual(string.Empty, _addressChecker.GetStart("os. Osiedle Albertyńskie 2, 31-851 Kraków"));

            Assert.AreEqual(string.Empty, _addressChecker.GetStart("Osiedle Leśne 2, 71-238 Bezrzecze"));

            Assert.AreEqual("ul.", _addressChecker.GetStart("Osiedle 2, 80-725 Gdańsk"));
            
            Assert.AreEqual(string.Empty, _addressChecker.GetStart("Osiedle Sady 2, 45-840 Opole"));

            Assert.AreEqual(string.Empty, _addressChecker.GetStart("Osiedle Sady, 45-840 Opole"));


        }

        [Test]
        public void OnlyTownNoStreetTest()
        {

            Assert.AreEqual(string.Empty, _addressChecker.GetStart("Łopuchowo 2, 62-095 Murowana Goślina"));

            Assert.AreEqual(string.Empty, _addressChecker.GetStart("Łopuchowo, 62-095 Murowana Goślina"));

            Assert.AreEqual(string.Empty, _addressChecker.GetStart("Biel, 07-300 Ostrów Mazowiecka"));
        }

    }
}
