using DocumentGenerator.Lib.Helpers;
using NUnit.Framework;

namespace DocumentGenerator.Lib.Test
{
    public class NumToWordsPlnConvTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ZeroTest()
        {
            Assert.AreEqual("zero z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("0"));
        }

        [Test]
        public void OneDigitNumTest()
        {
            Assert.AreEqual("jeden z�oty 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("1"));
            Assert.AreEqual("pi�� z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("5"));
            Assert.AreEqual("dziewi�� z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("9"));
            Assert.AreEqual("dziewi�� z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("9,00"));
            Assert.AreEqual("trzy z�ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("3"));
            Assert.AreEqual("trzy z�ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("3.00"));
        }

        [Test]
        public void TeenstNumTest()
        {
            Assert.AreEqual("dziesi�� z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("10"));
            Assert.AreEqual("jedena�cie z�otych 89/100", NumberToWordsConverter.ConvertNumberToAmountPln("11,89"));
            Assert.AreEqual("dwana�cie z�otych 87/100", NumberToWordsConverter.ConvertNumberToAmountPln("12.87"));
            Assert.AreEqual("trzyna�cie z�otych 76/100", NumberToWordsConverter.ConvertNumberToAmountPln("13.76"));
            Assert.AreEqual("dziewi�tna�cie z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("19"));
           
        }
    
        [Test]
        public void TwoDigitNumTest()
        {
            Assert.AreEqual("dwadzie�cia jeden z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("21"));
            Assert.AreEqual("trzydzie�ci dwa z�ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("32"));
            Assert.AreEqual("czterdzie�ci cztery z�ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("44"));
            Assert.AreEqual("pi��dziesi�t pi�� z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("55"));
            Assert.AreEqual("sze��dziesi�t sze�� z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("66"));
            Assert.AreEqual("sze��dziesi�t z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("60"));
            Assert.AreEqual("siedemdziesi�t trzy z�ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("73"));
            Assert.AreEqual("osiemdziesi�t siedem z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("87"));
            Assert.AreEqual("dziewi��dziesi�t osiem z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("98"));
            Assert.AreEqual("dwadzie�cia dwa z�ote 34/100", NumberToWordsConverter.ConvertNumberToAmountPln("22.34"));
        }

        [Test]
        public void ThreeNumTest()
        {
            Assert.AreEqual("sto dwadzie�cia dziewi�� z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("129"));
            Assert.AreEqual("trzysta pi�tna�cie z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("315"));
            Assert.AreEqual("osiemset pi��dziesi�t cztery z�ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("854"));
            Assert.AreEqual("dziewi��set dziewi��dziesi�t dziewi�� z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("999"));
        }

        [Test]
        public void FourDigitNumTest()
        {
            Assert.AreEqual("jeden tysi�c dwie�cie trzydzie�ci trzy z�ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("1233"));
            Assert.AreEqual("dwa tysi�ce trzysta trzy z�ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("2303"));
            Assert.AreEqual("sze�� tysi�cy trzyna�cie z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("6013"));
            Assert.AreEqual("pi�� tysi�cy dwie�cie dwadzie�cia osiem z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("5228"));
            Assert.AreEqual("trzy tysi�ce sto z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("3100"));
        }

        [Test]
        public void FiveDigitNumTest()
        {
            Assert.AreEqual("jedena�cie tysi�cy dwie�cie czterdzie�ci cztery z�ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("11244"));
            Assert.AreEqual("dwadzie�cia dwa tysi�ce czterysta jeden z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("22401"));
            Assert.AreEqual("pi��dziesi�t sze�� tysi�cy czterna�cie z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("56014"));
            Assert.AreEqual("trzydzie�ci jeden tysi�cy z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("31000"));
            Assert.AreEqual("dwadzie�cia tysi�cy trzy z�ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("20003"));
            Assert.AreEqual("dziesi�� tysi�cy czterysta pi��dziesi�t sze�� z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("10456"));

        }

        [Test]
        public void SiXDigitNumTest()
        {
            Assert.AreEqual("sto dwadzie�cia jeden tysi�cy dwie�cie czterdzie�ci pi�� z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("121245"));
            Assert.AreEqual("trzysta dwa tysi�ce trzy z�ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("302003"));
            Assert.AreEqual("pi��set tysi�cy jedena�cie z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("500011"));
            Assert.AreEqual("dziewi��set jedena�cie tysi�cy trzyna�cie z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("911013"));
            Assert.AreEqual("sto tysi�cy z�otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("100000"));
            Assert.AreEqual("sto dwa tysi�ce dwie�cie dwa z�ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("102202"));

        }

    }
}