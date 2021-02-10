using DocumentGenerator.Lib.Helpers;
using NUnit.Framework;

namespace DocumentGenerator.Lib.Test
{
    public class NumToWordsConvTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ZeroTest()
        {
            Assert.AreEqual("zero", NumberToWordsConverter.ConvertToWords(0));
        }

        [Test]
        public void OneDigitNumTest()
        {
            Assert.AreEqual("jeden", NumberToWordsConverter.ConvertToWords(1));
            Assert.AreEqual("pi��", NumberToWordsConverter.ConvertToWords(5));
            Assert.AreEqual("dziewi��", NumberToWordsConverter.ConvertToWords(9));
        }

        [Test]
        public void TeenstNumTest()
        {
            Assert.AreEqual("dziesi��", NumberToWordsConverter.ConvertToWords(10));
            Assert.AreEqual("jedena�cie", NumberToWordsConverter.ConvertToWords(11));
            Assert.AreEqual("dwana�cie", NumberToWordsConverter.ConvertToWords(12));
            Assert.AreEqual("czterna�cie", NumberToWordsConverter.ConvertToWords(14));
            Assert.AreEqual("dziewi�tna�cie", NumberToWordsConverter.ConvertToWords(19));
        }
    
        [Test]
        public void TwoDigitNumTest()
        {
            Assert.AreEqual("dwadzie�cia jeden", NumberToWordsConverter.ConvertToWords(21));
            Assert.AreEqual("trzydzie�ci dwa", NumberToWordsConverter.ConvertToWords(32));
            Assert.AreEqual("czterdzie�ci cztery", NumberToWordsConverter.ConvertToWords(44));
            Assert.AreEqual("pi��dziesi�t pi��", NumberToWordsConverter.ConvertToWords(55));
            Assert.AreEqual("sze��dziesi�t sze��", NumberToWordsConverter.ConvertToWords(66));
            Assert.AreEqual("sze��dziesi�t", NumberToWordsConverter.ConvertToWords(60));
            Assert.AreEqual("siedemdziesi�t trzy", NumberToWordsConverter.ConvertToWords(73));
            Assert.AreEqual("osiemdziesi�t siedem", NumberToWordsConverter.ConvertToWords(87));
            Assert.AreEqual("dziewi��dziesi�t osiem", NumberToWordsConverter.ConvertToWords(98));
        }

        [Test]
        public void ThreeNumTest()
        {
            Assert.AreEqual("sto dwadzie�cia dziewi��", NumberToWordsConverter.ConvertToWords(129));
            Assert.AreEqual("trzysta pi�tna�cie", NumberToWordsConverter.ConvertToWords(315));
            Assert.AreEqual("osiemset pi��dziesi�t cztery", NumberToWordsConverter.ConvertToWords(854));
            Assert.AreEqual("dziewi��set dziewi��dziesi�t dziewi��", NumberToWordsConverter.ConvertToWords(999));
        }

        [Test]
        public void FourDigitNumTest()
        {
            Assert.AreEqual("jeden tysi�c dwie�cie trzydzie�ci trzy", NumberToWordsConverter.ConvertToWords(1233));
            Assert.AreEqual("dwa tysi�ce trzysta trzy", NumberToWordsConverter.ConvertToWords(2303));
            Assert.AreEqual("sze�� tysi�cy trzyna�cie", NumberToWordsConverter.ConvertToWords(6013));
            Assert.AreEqual("pi�� tysi�cy dwie�cie dwadzie�cia osiem", NumberToWordsConverter.ConvertToWords(5228));
            Assert.AreEqual("trzy tysi�ce sto", NumberToWordsConverter.ConvertToWords(3100));
        }

        [Test]
        public void FiveDigitNumTest()
        {
            Assert.AreEqual("jedena�cie tysi�cy dwie�cie czterdzie�ci cztery", NumberToWordsConverter.ConvertToWords(11244));
            Assert.AreEqual("dwadzie�cia dwa tysi�ce czterysta jeden", NumberToWordsConverter.ConvertToWords(22401));
            Assert.AreEqual("pi��dziesi�t sze�� tysi�cy czterna�cie", NumberToWordsConverter.ConvertToWords(56014));
            Assert.AreEqual("trzydzie�ci jeden tysi�cy", NumberToWordsConverter.ConvertToWords(31000));
            Assert.AreEqual("dwadzie�cia tysi�cy trzy", NumberToWordsConverter.ConvertToWords(20003));
            Assert.AreEqual("dziesi�� tysi�cy czterysta pi��dziesi�t sze��", NumberToWordsConverter.ConvertToWords(10456));

        }

        [Test]
        public void SiXDigitNumTest()
        {
            Assert.AreEqual("sto dwadzie�cia jeden tysi�cy dwie�cie czterdzie�ci pi��", NumberToWordsConverter.ConvertToWords(121245));
            Assert.AreEqual("trzysta dwa tysi�ce trzy", NumberToWordsConverter.ConvertToWords(302003));
            Assert.AreEqual("pi��set tysi�cy jedena�cie", NumberToWordsConverter.ConvertToWords(500011));
            Assert.AreEqual("dziewi��set jedena�cie tysi�cy trzyna�cie", NumberToWordsConverter.ConvertToWords(911013));
            Assert.AreEqual("sto tysi�cy", NumberToWordsConverter.ConvertToWords(100000));
            Assert.AreEqual("sto dwa tysi�ce dwie�cie dwa", NumberToWordsConverter.ConvertToWords(102202));

        }

    }
}