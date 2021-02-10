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
            Assert.AreEqual("zero z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("0"));
        }

        [Test]
        public void OneDigitNumTest()
        {
            Assert.AreEqual("jeden z³oty 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("1"));
            Assert.AreEqual("piêæ z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("5"));
            Assert.AreEqual("dziewiêæ z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("9"));
            Assert.AreEqual("dziewiêæ z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("9,00"));
            Assert.AreEqual("trzy z³ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("3"));
            Assert.AreEqual("trzy z³ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("3.00"));
        }

        [Test]
        public void TeenstNumTest()
        {
            Assert.AreEqual("dziesiêæ z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("10"));
            Assert.AreEqual("jedenaœcie z³otych 89/100", NumberToWordsConverter.ConvertNumberToAmountPln("11,89"));
            Assert.AreEqual("dwanaœcie z³otych 87/100", NumberToWordsConverter.ConvertNumberToAmountPln("12.87"));
            Assert.AreEqual("trzynaœcie z³otych 76/100", NumberToWordsConverter.ConvertNumberToAmountPln("13.76"));
            Assert.AreEqual("dziewiêtnaœcie z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("19"));
           
        }
    
        [Test]
        public void TwoDigitNumTest()
        {
            Assert.AreEqual("dwadzieœcia jeden z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("21"));
            Assert.AreEqual("trzydzieœci dwa z³ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("32"));
            Assert.AreEqual("czterdzieœci cztery z³ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("44"));
            Assert.AreEqual("piêædziesi¹t piêæ z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("55"));
            Assert.AreEqual("szeœædziesi¹t szeœæ z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("66"));
            Assert.AreEqual("szeœædziesi¹t z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("60"));
            Assert.AreEqual("siedemdziesi¹t trzy z³ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("73"));
            Assert.AreEqual("osiemdziesi¹t siedem z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("87"));
            Assert.AreEqual("dziewiêædziesi¹t osiem z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("98"));
            Assert.AreEqual("dwadzieœcia dwa z³ote 34/100", NumberToWordsConverter.ConvertNumberToAmountPln("22.34"));
        }

        [Test]
        public void ThreeNumTest()
        {
            Assert.AreEqual("sto dwadzieœcia dziewiêæ z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("129"));
            Assert.AreEqual("trzysta piêtnaœcie z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("315"));
            Assert.AreEqual("osiemset piêædziesi¹t cztery z³ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("854"));
            Assert.AreEqual("dziewiêæset dziewiêædziesi¹t dziewiêæ z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("999"));
        }

        [Test]
        public void FourDigitNumTest()
        {
            Assert.AreEqual("jeden tysi¹c dwieœcie trzydzieœci trzy z³ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("1233"));
            Assert.AreEqual("dwa tysi¹ce trzysta trzy z³ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("2303"));
            Assert.AreEqual("szeœæ tysiêcy trzynaœcie z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("6013"));
            Assert.AreEqual("piêæ tysiêcy dwieœcie dwadzieœcia osiem z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("5228"));
            Assert.AreEqual("trzy tysi¹ce sto z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("3100"));
        }

        [Test]
        public void FiveDigitNumTest()
        {
            Assert.AreEqual("jedenaœcie tysiêcy dwieœcie czterdzieœci cztery z³ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("11244"));
            Assert.AreEqual("dwadzieœcia dwa tysi¹ce czterysta jeden z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("22401"));
            Assert.AreEqual("piêædziesi¹t szeœæ tysiêcy czternaœcie z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("56014"));
            Assert.AreEqual("trzydzieœci jeden tysiêcy z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("31000"));
            Assert.AreEqual("dwadzieœcia tysiêcy trzy z³ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("20003"));
            Assert.AreEqual("dziesiêæ tysiêcy czterysta piêædziesi¹t szeœæ z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("10456"));

        }

        [Test]
        public void SiXDigitNumTest()
        {
            Assert.AreEqual("sto dwadzieœcia jeden tysiêcy dwieœcie czterdzieœci piêæ z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("121245"));
            Assert.AreEqual("trzysta dwa tysi¹ce trzy z³ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("302003"));
            Assert.AreEqual("piêæset tysiêcy jedenaœcie z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("500011"));
            Assert.AreEqual("dziewiêæset jedenaœcie tysiêcy trzynaœcie z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("911013"));
            Assert.AreEqual("sto tysiêcy z³otych 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("100000"));
            Assert.AreEqual("sto dwa tysi¹ce dwieœcie dwa z³ote 00/100", NumberToWordsConverter.ConvertNumberToAmountPln("102202"));

        }

    }
}