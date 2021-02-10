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
            Assert.AreEqual("piêæ", NumberToWordsConverter.ConvertToWords(5));
            Assert.AreEqual("dziewiêæ", NumberToWordsConverter.ConvertToWords(9));
        }

        [Test]
        public void TeenstNumTest()
        {
            Assert.AreEqual("dziesiêæ", NumberToWordsConverter.ConvertToWords(10));
            Assert.AreEqual("jedenaœcie", NumberToWordsConverter.ConvertToWords(11));
            Assert.AreEqual("dwanaœcie", NumberToWordsConverter.ConvertToWords(12));
            Assert.AreEqual("czternaœcie", NumberToWordsConverter.ConvertToWords(14));
            Assert.AreEqual("dziewiêtnaœcie", NumberToWordsConverter.ConvertToWords(19));
        }
    
        [Test]
        public void TwoDigitNumTest()
        {
            Assert.AreEqual("dwadzieœcia jeden", NumberToWordsConverter.ConvertToWords(21));
            Assert.AreEqual("trzydzieœci dwa", NumberToWordsConverter.ConvertToWords(32));
            Assert.AreEqual("czterdzieœci cztery", NumberToWordsConverter.ConvertToWords(44));
            Assert.AreEqual("piêædziesi¹t piêæ", NumberToWordsConverter.ConvertToWords(55));
            Assert.AreEqual("szeœædziesi¹t szeœæ", NumberToWordsConverter.ConvertToWords(66));
            Assert.AreEqual("szeœædziesi¹t", NumberToWordsConverter.ConvertToWords(60));
            Assert.AreEqual("siedemdziesi¹t trzy", NumberToWordsConverter.ConvertToWords(73));
            Assert.AreEqual("osiemdziesi¹t siedem", NumberToWordsConverter.ConvertToWords(87));
            Assert.AreEqual("dziewiêædziesi¹t osiem", NumberToWordsConverter.ConvertToWords(98));
        }

        [Test]
        public void ThreeNumTest()
        {
            Assert.AreEqual("sto dwadzieœcia dziewiêæ", NumberToWordsConverter.ConvertToWords(129));
            Assert.AreEqual("trzysta piêtnaœcie", NumberToWordsConverter.ConvertToWords(315));
            Assert.AreEqual("osiemset piêædziesi¹t cztery", NumberToWordsConverter.ConvertToWords(854));
            Assert.AreEqual("dziewiêæset dziewiêædziesi¹t dziewiêæ", NumberToWordsConverter.ConvertToWords(999));
        }

        [Test]
        public void FourDigitNumTest()
        {
            Assert.AreEqual("jeden tysi¹c dwieœcie trzydzieœci trzy", NumberToWordsConverter.ConvertToWords(1233));
            Assert.AreEqual("dwa tysi¹ce trzysta trzy", NumberToWordsConverter.ConvertToWords(2303));
            Assert.AreEqual("szeœæ tysiêcy trzynaœcie", NumberToWordsConverter.ConvertToWords(6013));
            Assert.AreEqual("piêæ tysiêcy dwieœcie dwadzieœcia osiem", NumberToWordsConverter.ConvertToWords(5228));
            Assert.AreEqual("trzy tysi¹ce sto", NumberToWordsConverter.ConvertToWords(3100));
        }

        [Test]
        public void FiveDigitNumTest()
        {
            Assert.AreEqual("jedenaœcie tysiêcy dwieœcie czterdzieœci cztery", NumberToWordsConverter.ConvertToWords(11244));
            Assert.AreEqual("dwadzieœcia dwa tysi¹ce czterysta jeden", NumberToWordsConverter.ConvertToWords(22401));
            Assert.AreEqual("piêædziesi¹t szeœæ tysiêcy czternaœcie", NumberToWordsConverter.ConvertToWords(56014));
            Assert.AreEqual("trzydzieœci jeden tysiêcy", NumberToWordsConverter.ConvertToWords(31000));
            Assert.AreEqual("dwadzieœcia tysiêcy trzy", NumberToWordsConverter.ConvertToWords(20003));
            Assert.AreEqual("dziesiêæ tysiêcy czterysta piêædziesi¹t szeœæ", NumberToWordsConverter.ConvertToWords(10456));

        }

        [Test]
        public void SiXDigitNumTest()
        {
            Assert.AreEqual("sto dwadzieœcia jeden tysiêcy dwieœcie czterdzieœci piêæ", NumberToWordsConverter.ConvertToWords(121245));
            Assert.AreEqual("trzysta dwa tysi¹ce trzy", NumberToWordsConverter.ConvertToWords(302003));
            Assert.AreEqual("piêæset tysiêcy jedenaœcie", NumberToWordsConverter.ConvertToWords(500011));
            Assert.AreEqual("dziewiêæset jedenaœcie tysiêcy trzynaœcie", NumberToWordsConverter.ConvertToWords(911013));
            Assert.AreEqual("sto tysiêcy", NumberToWordsConverter.ConvertToWords(100000));
            Assert.AreEqual("sto dwa tysi¹ce dwieœcie dwa", NumberToWordsConverter.ConvertToWords(102202));

        }

    }
}